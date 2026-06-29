using Npgsql;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace VendorHub.Models.Repositories
{
    public class AdminDashboardRepository
    {
        private readonly string _connectionString;

        public AdminDashboardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<TopVendorStat> GetTopVendors(int companyId, int count = 5)
        {
            var list = new List<TopVendorStat>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"
                    SELECT v.""VendorName"", COUNT(vr.""Id"") as ResponseCount
                    FROM ""Vendors"" v
                    LEFT JOIN ""VendorResponses"" vr ON v.""VendorId"" = vr.""VendorId""
                    WHERE v.""CompanyId"" = @CompanyId
                    GROUP BY v.""VendorId"", v.""VendorName""
                    ORDER BY ResponseCount DESC
                    LIMIT @Count;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    cmd.Parameters.AddWithValue("@Count", count);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new TopVendorStat
                            {
                                VendorName = reader["VendorName"].ToString(),
                                ResponseCount = Convert.ToInt32(reader["ResponseCount"])
                            });
                        }
                    }
                }
            }
            return list;
        }

        public AdminDashboardStats GetAdminDashboardStats(int companyId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        (SELECT COUNT(*) FROM ""Vendors"" WHERE ""CompanyId"" = @CompanyId) as TotalVendors,
                        (SELECT COUNT(*) FROM ""Quotations"" q
                         INNER JOIN ""QuotationRequests"" qr ON q.""QuotationRequestId"" = qr.""QuotationRequestId""
                         WHERE q.""Status"" = 'Active' AND qr.""CompanyId"" = @CompanyId) as ActiveQuotes,
                        (SELECT COUNT(*) FROM ""Quotations"" q
                         INNER JOIN ""QuotationRequests"" qr ON q.""QuotationRequestId"" = qr.""QuotationRequestId""
                         WHERE q.""Status"" = 'Pending' AND qr.""CompanyId"" = @CompanyId) as PendingQuotes,
                        (SELECT COUNT(*) FROM ""Quotations"" q
                         INNER JOIN ""QuotationRequests"" qr ON q.""QuotationRequestId"" = qr.""QuotationRequestId""
                         WHERE q.""Status"" = 'Approved' AND qr.""CompanyId"" = @CompanyId) as ApprovedQuotes;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new AdminDashboardStats
                            {
                                TotalVendors = Convert.ToInt32(reader["TotalVendors"]),
                                ActiveQuotations = Convert.ToInt32(reader["ActiveQuotes"]),
                                PendingQuotations = Convert.ToInt32(reader["PendingQuotes"]),
                                ApprovedQuotations = Convert.ToInt32(reader["ApprovedQuotes"])
                            };
                        }
                    }
                }
            }
            return new AdminDashboardStats { TotalVendors = 0, ActiveQuotations = 0, PendingQuotations = 0, ApprovedQuotations = 0 };
        }
    }
}