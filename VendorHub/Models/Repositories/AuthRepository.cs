using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VendorHub.Data;
using VendorHub.Models.Classes;
using System.Diagnostics.CodeAnalysis;

namespace VendorHub.Models.Repositories
{
    public class AuthRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthRepository(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> RegisterAsync(string username, string email, string password, string role)
        {
            try
            {
                bool exists = await _context.Users.AnyAsync(u => u.Email == email);
                if (exists)
                {
                    Console.WriteLine($"REGISTER: User with email {email} already exists");
                    return false;
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                var user = new User
                {
                    Username = username,
                    Email = email,
                    Password = hashedPassword,
                    Role = role,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Users.AddAsync(user);
                int rows = await _context.SaveChangesAsync();

                Console.WriteLine($"Rows saved: {rows}");
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"REGISTER DB ERROR: {ex.Message}");
                Console.WriteLine($"INNER: {ex.InnerException?.Message}");
                throw;
            }
        }

        public bool RegisterAdmin(string username, string email, string password, int companyId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string checkQuery = @"SELECT COUNT(*) FROM ""Users"" WHERE ""Email"" = @Email;";
                using (var checkCmd = new NpgsqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Email", email);
                    conn.Open();
                    long count = Convert.ToInt64(checkCmd.ExecuteScalar());
                    if (count > 0) return false;
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                string insertQuery = @"INSERT INTO ""Users"" (""Username"", ""Email"", ""Password"", ""Role"", ""CreatedAt"") 
                                      VALUES (@Username, @Email, @Password, 'Admin', @CreatedAt);";
                using (var cmd = new NpgsqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    Console.WriteLine("LOGIN FAILED: No user found with given email");
                    return null;
                }

                bool passwordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
                if (!passwordValid)
                {
                    Console.WriteLine("LOGIN FAILED: Invalid password for " + email);
                    return null;
                }

                Console.WriteLine($"LOGIN SUCCESS: {user.Email} Role={user.Role}");
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LOGIN DB ERROR: {ex.Message}");
                Console.WriteLine($"INNER: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task<(string? SmtpEmail, string? SmtpAppPassword)> GetSmtpCredentialsAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            return (user?.SmtpEmail, user?.SmtpAppPassword);
        }

        public async Task<bool> UpdateSmtpCredentialsAsync(int userId, string smtpEmail, string smtpAppPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return false;
            user.SmtpEmail = smtpEmail;
            user.SmtpAppPassword = smtpAppPassword;
            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateJwtToken(int userId, string username, string email, string role, int companyId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim("CompanyId", companyId.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateInvitationToken(string email, int vendorId)
        {
            string token = Guid.NewGuid().ToString("N");
            DateTime expiryDate = DateTime.UtcNow.AddDays(3);
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"INSERT INTO ""VendorInvitations"" (""Token"", ""Email"", ""VendorId"", ""ExpiryDate"", ""IsUsed"") 
                                VALUES (@Token, @Email, @VendorId, @ExpiryDate, FALSE);";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Token", token);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@VendorId", vendorId);
                    cmd.Parameters.AddWithValue("@ExpiryDate", expiryDate);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return token;
        }

        public bool VerifyInvitationToken(string token)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"SELECT COUNT(*) FROM ""VendorInvitations"" 
                                WHERE ""Token"" = @Token AND ""IsUsed"" = FALSE AND ""ExpiryDate"" > @Now;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Token", token);
                    cmd.Parameters.AddWithValue("@Now", DateTime.UtcNow);
                    conn.Open();
                    long count = (long)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public bool RegisterVendor(string token, string password)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string selectQuery = @"SELECT ""Email"", ""VendorId"" FROM ""VendorInvitations"" 
                                      WHERE ""Token"" = @Token AND ""IsUsed"" = FALSE AND ""ExpiryDate"" > @Now;";
                string email = "";
                int vendorId = 0;
                using (var cmd = new NpgsqlCommand(selectQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Token", token);
                    cmd.Parameters.AddWithValue("@Now", DateTime.UtcNow);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return false;
                        email = reader["Email"].ToString();
                        vendorId = Convert.ToInt32(reader["VendorId"]);
                    }
                }
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                string insertUserQuery = @"INSERT INTO ""Users"" (""Username"", ""Email"", ""Password"", ""Role"", ""CreatedAt"") 
                                          VALUES (@Username, @Email, @Password, 'Vendor', @CreatedAt);";
                using (var cmd = new NpgsqlCommand(insertUserQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", email);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                    cmd.ExecuteNonQuery();
                }
                string updateTokenQuery = @"UPDATE ""VendorInvitations"" SET ""IsUsed"" = TRUE WHERE ""Token"" = @Token;";
                using (var cmd = new NpgsqlCommand(updateTokenQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Token", token);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
        }
    }
}
