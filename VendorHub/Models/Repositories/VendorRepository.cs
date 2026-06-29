using Npgsql;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using VendorHub.Models.Classes;

namespace VendorHub.Models.Repositories
{
    public class VendorRepository
    {
        private readonly string _connectionString;

        public VendorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void AddVendor(Vendor vendor)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"INSERT INTO ""Vendors"" (""VendorName"", ""CompanyName"", ""EmailAddress"", ""ContactNumber"", ""BusinessAddress"", ""CompanyId"") 
                                VALUES (@VendorName, @CompanyName, @EmailAddress, @ContactNumber, @BusinessAddress, @CompanyId);";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VendorName", vendor.VendorName);
                    cmd.Parameters.AddWithValue("@CompanyName", vendor.CompanyName);
                    cmd.Parameters.AddWithValue("@EmailAddress", vendor.EmailAddress);
                    cmd.Parameters.AddWithValue("@ContactNumber", vendor.ContactNumber);
                    cmd.Parameters.AddWithValue("@BusinessAddress", vendor.BusinessAddress);
                    cmd.Parameters.AddWithValue("@CompanyId", vendor.CompanyId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Vendor GetVendorById(int id)
        {
            Vendor vendor = null;
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM ""Vendors"" WHERE ""VendorId"" = @VendorId;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VendorId", id);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            vendor = new Vendor
                            {
                                VendorId = Convert.ToInt32(reader["VendorId"]),
                                VendorName = reader["VendorName"].ToString(),
                                CompanyName = reader["CompanyName"].ToString(),
                                EmailAddress = reader["EmailAddress"].ToString(),
                                ContactNumber = reader["ContactNumber"].ToString(),
                                BusinessAddress = reader["BusinessAddress"].ToString(),
                                CompanyId = reader["CompanyId"] != DBNull.Value ? Convert.ToInt32(reader["CompanyId"]) : 0
                            };
                        }
                    }
                }
            }
            return vendor;
        }

        public List<Vendor> GetAllVendors()
        {
            var list = new List<Vendor>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM ""Vendors"";";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Vendor
                            {
                                VendorId = Convert.ToInt32(reader["VendorId"]),
                                VendorName = reader["VendorName"].ToString(),
                                CompanyName = reader["CompanyName"].ToString(),
                                EmailAddress = reader["EmailAddress"].ToString(),
                                ContactNumber = reader["ContactNumber"].ToString(),
                                BusinessAddress = reader["BusinessAddress"].ToString(),
                                CompanyId = reader["CompanyId"] != DBNull.Value ? Convert.ToInt32(reader["CompanyId"]) : 0
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void UpdateVendor(Vendor vendor)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"UPDATE ""Vendors"" 
                                SET ""VendorName"" = @VendorName, ""CompanyName"" = @CompanyName, 
                                    ""EmailAddress"" = @EmailAddress, ""ContactNumber"" = @ContactNumber, 
                                    ""BusinessAddress"" = @BusinessAddress,
                                    ""CompanyId"" = @CompanyId
                                WHERE ""VendorId"" = @VendorId;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VendorId", vendor.VendorId);
                    cmd.Parameters.AddWithValue("@VendorName", vendor.VendorName);
                    cmd.Parameters.AddWithValue("@CompanyName", vendor.CompanyName);
                    cmd.Parameters.AddWithValue("@EmailAddress", vendor.EmailAddress);
                    cmd.Parameters.AddWithValue("@ContactNumber", vendor.ContactNumber);
                    cmd.Parameters.AddWithValue("@BusinessAddress", vendor.BusinessAddress);
                    cmd.Parameters.AddWithValue("@CompanyId", vendor.CompanyId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteVendor(int id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"DELETE FROM ""Vendors"" WHERE ""VendorId"" = @VendorId;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VendorId", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Vendor> SearchAndFilterVendors(string searchTerm)
{
    var list = new List<Vendor>();
    using (var conn = new NpgsqlConnection(_connectionString))
    {
        string query = @"SELECT * FROM ""Vendors"" 
                        WHERE ""VendorName"" ILIKE @SearchTerm 
                           OR ""CompanyName"" ILIKE @SearchTerm 
                           OR ""BusinessAddress"" ILIKE @SearchTerm;";

        using (var cmd = new NpgsqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
            conn.Open();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new Vendor
                    {
                        VendorId = Convert.ToInt32(reader["VendorId"]),
                        VendorName = reader["VendorName"].ToString(),
                        CompanyName = reader["CompanyName"].ToString(),
                        EmailAddress = reader["EmailAddress"].ToString(),
                        ContactNumber = reader["ContactNumber"].ToString(),
                        BusinessAddress = reader["BusinessAddress"].ToString(),
                        CompanyId = reader["CompanyId"] != DBNull.Value ? Convert.ToInt32(reader["CompanyId"]) : 0
                    });
                }
            }
        }
    }
    return list;
}
    }
}