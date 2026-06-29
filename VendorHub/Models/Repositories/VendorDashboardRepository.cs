using Npgsql;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using VendorHub.Models.Classes;

namespace VendorHub.Models.Repositories
{
    public class VendorDashboardRepository
    {
        private readonly string _connectionString;

        public VendorDashboardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public VendorDashboardStats GetVendorDashboardStats(int vendorId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        (SELECT COUNT(*) FROM ""QuotationAssignments"" WHERE ""VendorId"" = @VendorId) as TotalRequests,
                        (SELECT COUNT(*) FROM ""VendorResponses"" WHERE ""VendorId"" = @VendorId) as SubmittedQuotes,
                        (SELECT COUNT(*) FROM ""VendorResponses"" WHERE ""VendorId"" = @VendorId AND ""Status"" = 'Submitted') as PendingApproval,
                        (SELECT COUNT(*) FROM ""VendorResponses"" WHERE ""VendorId"" = @VendorId AND ""Status"" = 'Approved') as WonOrders,
                        (SELECT COUNT(*) FROM ""QuotationAssignments"" qa2
                         LEFT JOIN ""VendorResponses"" vr2 ON vr2.""QuotationAssignmentId"" = qa2.""Id"" AND vr2.""VendorId"" = qa2.""VendorId""
                         WHERE qa2.""VendorId"" = @VendorId AND vr2.""Id"" IS NULL) as PendingRequests,
                        (SELECT COUNT(*) FROM ""VendorResponses"" WHERE ""VendorId"" = @VendorId AND ""Status"" = 'Responded') as SubmittedResponses,
                        (SELECT COUNT(*) FROM ""VendorResponses"" WHERE ""VendorId"" = @VendorId AND ""Status"" = 'Rejected') as RejectedResponses,
                        (SELECT COALESCE(ROUND(AVG(vr3.""Price""), 2), 0) FROM ""VendorResponses"" vr3 WHERE vr3.""VendorId"" = @VendorId) as AvgBidPrice,
                        (SELECT COALESCE(ROUND(AVG(qr3.""Budget""), 2), 0) FROM ""QuotationAssignments"" qa3
                         INNER JOIN ""QuotationRequests"" qr3 ON qa3.""QuotationRequestId"" = qr3.""QuotationRequestId""
                         WHERE qa3.""VendorId"" = @VendorId) as AvgBudget;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VendorId", vendorId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var total = Convert.ToInt32(reader["TotalRequests"]);
                            return new VendorDashboardStats
                            {
                                TotalRequestsSent = total,
                                TotalRequestsReceived = total,
                                SubmittedQuotes = Convert.ToInt32(reader["SubmittedQuotes"]),
                                PendingApproval = Convert.ToInt32(reader["PendingApproval"]),
                                WonOrders = Convert.ToInt32(reader["WonOrders"]),
                                PendingRequests = Convert.ToInt32(reader["PendingRequests"]),
                                SubmittedResponses = Convert.ToInt32(reader["SubmittedResponses"]),
                                RejectedResponses = Convert.ToInt32(reader["RejectedResponses"]),
                                AvgBidPrice = Convert.ToDecimal(reader["AvgBidPrice"]),
                                AvgBudget = Convert.ToDecimal(reader["AvgBudget"])
                            };
                        }
                    }
                }
            }
            return new VendorDashboardStats { TotalRequestsReceived = 0, SubmittedQuotes = 0, PendingApproval = 0, WonOrders = 0, PendingRequests = 0, SubmittedResponses = 0, RejectedResponses = 0, AvgBidPrice = 0, AvgBudget = 0 };
        }

        public List<VendorDashboardActivity> GetRecentVendorActivity(int vendorId, int count = 5)
        {
            var list = new List<VendorDashboardActivity>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"
                    SELECT qa.""Id"" as AssignmentId, qr.""Title"" as QuotationTitle, qa.""Status"" as AssignmentStatus, qa.""AssignedAt"",
                           vr.""Status"" as ResponseStatus, vr.""SubmittedAt""
                    FROM ""QuotationAssignments"" qa
                    INNER JOIN ""QuotationRequests"" qr ON qa.""QuotationRequestId"" = qr.""QuotationRequestId""
                    LEFT JOIN ""VendorResponses"" vr ON vr.""QuotationAssignmentId"" = qa.""Id"" AND vr.""VendorId"" = qa.""VendorId""
                    WHERE qa.""VendorId"" = @VendorId
                    ORDER BY COALESCE(vr.""SubmittedAt"", qa.""AssignedAt"") DESC
                    LIMIT @Count;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VendorId", vendorId);
                    cmd.Parameters.AddWithValue("@Count", count);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var status = reader["AssignmentStatus"].ToString();
                            var respStatus = reader["ResponseStatus"] != DBNull.Value ? reader["ResponseStatus"].ToString() : null;
                            if (respStatus != null)
                                status = respStatus;
                            list.Add(new VendorDashboardActivity
                            {
                                AssignmentId = Convert.ToInt32(reader["AssignmentId"]),
                                QuotationTitle = reader["QuotationTitle"].ToString(),
                                Status = status,
                                Date = reader["SubmittedAt"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["SubmittedAt"])
                                    : Convert.ToDateTime(reader["AssignedAt"])
                            });
                        }
                    }
                }
            }
            return list;
        }
    }

    public class VendorDashboardActivity
    {
        public int AssignmentId { get; set; }
        public string QuotationTitle { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime Date { get; set; }
    }
}
