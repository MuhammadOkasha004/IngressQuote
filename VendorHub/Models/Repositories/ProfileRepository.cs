using Npgsql;
using System;

namespace VendorHub.Models.Repositories
{
    public class ProfileRepository
    {
        private readonly string _connectionString = "Host=ep-plain-waterfall-at1aeg2h-pooler.c-9.us-east-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_HSh1KC9zfnkl;SSL Mode=Require;Trust Server Certificate=true;";

        public object GetUserProfile(string email)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"SELECT ""Email"", ""Role"", ""VendorId"" FROM ""Users"" WHERE ""Email"" = @Email;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new
                            {
                                Email = reader["Email"].ToString(),
                                Role = reader["Role"].ToString(),
                                VendorId = reader["VendorId"] != DBNull.Value ? Convert.ToInt32(reader["VendorId"]) : (int?)null
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool UpdatePassword(string email, string newHashedPassword)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"UPDATE ""Users"" SET ""Password"" = @Password WHERE ""Email"" = @Email;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Password", newHashedPassword);
                    cmd.Parameters.AddWithValue("@Email", email);
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }
    }
}