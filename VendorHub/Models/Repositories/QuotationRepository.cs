using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VendorHub.Data;
using VendorHub.Models.Classes;

namespace VendorHub.Models.Repositories
{
    public class QuotationRepository
    {
        private readonly string _connectionString;
        private readonly ApplicationDbContext _context;

        public QuotationRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<QuotationRequest>> GetAllRequestsAsync()
        {
            try
            {
                var result = await _context.QuotationRequests
                    .OrderByDescending(q => q.CreatedDate)
                    .ToListAsync();

                Console.WriteLine($"DB returned {result.Count} quotations");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB GETALL ERROR: {ex.Message}");
                Console.WriteLine($"INNER: {ex.InnerException?.Message}");
                throw;
            }
        }

        public int CreateQuotationRequest(QuotationRequest request)
        {
            _context.QuotationRequests.Add(request);
            _context.SaveChanges();
            return request.QuotationRequestId;
        }

        public void DeleteQuotationRequest(int id)
        {
            var entity = _context.QuotationRequests.Find(id);
            if (entity != null)
            {
                _context.QuotationRequests.Remove(entity);
                _context.SaveChanges();
            }
        }

        public void AssignQuotationRequestToVendors(QuotationRequest request, List<Vendor> selectedVendors)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                foreach (var vendor in selectedVendors)
                {
                    string query = @"INSERT INTO ""Quotations"" (""QuotationTitle"", ""Description"", ""VendorId"", ""QuotationRequestId"", ""QuotationAmount"", ""SubmissionDate"", ""Status"") 
                                    VALUES (@QuotationTitle, @Description, @VendorId, @QuotationRequestId, @QuotationAmount, @SubmissionDate, @Status);";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@QuotationTitle", request.Title);
                        cmd.Parameters.AddWithValue("@Description", request.Description);
                        cmd.Parameters.AddWithValue("@VendorId", vendor.VendorId);
                        cmd.Parameters.AddWithValue("@QuotationRequestId", request.QuotationRequestId);
                        cmd.Parameters.AddWithValue("@QuotationAmount", 0.00m);
                        cmd.Parameters.AddWithValue("@SubmissionDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Status", "Pending");

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void SubmitQuotationResponse(int quotationId, decimal amount)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"UPDATE ""Quotations"" 
                                SET ""QuotationAmount"" = @QuotationAmount, 
                                    ""Status"" = 'Submitted', 
                                    ""SubmissionDate"" = @SubmissionDate 
                                WHERE ""QuotationId"" = @QuotationId;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@QuotationId", quotationId);
                    cmd.Parameters.AddWithValue("@QuotationAmount", amount);
                    cmd.Parameters.AddWithValue("@SubmissionDate", DateTime.Now);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateQuotationStatus(int quotationId, string status)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"
                    UPDATE ""VendorResponses"" SET ""Status"" = @Status WHERE ""Id"" = @Id;
                    UPDATE ""QuotationAssignments"" qa SET ""Status"" = @Status
                    FROM ""VendorResponses"" vr
                    WHERE vr.""Id"" = @Id AND qa.""Id"" = vr.""QuotationAssignmentId"";";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", quotationId);
                    cmd.Parameters.AddWithValue("@Status", status);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Quotation> GetQuotationHistoryByCompany(int companyId)
        {
            var list = new List<Quotation>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"SELECT q.* FROM ""Quotations"" q
                                INNER JOIN ""QuotationRequests"" qr ON q.""QuotationRequestId"" = qr.""QuotationRequestId""
                                WHERE qr.""CompanyId"" = @CompanyId
                                ORDER BY q.""SubmissionDate"" DESC;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Quotation
                            {
                                QuotationId = Convert.ToInt32(reader["QuotationId"]),
                                QuotationTitle = reader["QuotationTitle"].ToString(),
                                Description = reader["Description"].ToString(),
                                VendorId = Convert.ToInt32(reader["VendorId"]),
                                QuotationRequestId = Convert.ToInt32(reader["QuotationRequestId"]),
                                QuotationAmount = Convert.ToDecimal(reader["QuotationAmount"]),
                                SubmissionDate = Convert.ToDateTime(reader["SubmissionDate"]),
                                Status = reader["Status"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<Quotation> GetQuotationHistoryByVendor(int vendorId)
        {
            var list = new List<Quotation>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM ""Quotations"" WHERE ""VendorId"" = @VendorId ORDER BY ""SubmissionDate"" DESC;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VendorId", vendorId);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Quotation
                            {
                                QuotationId = Convert.ToInt32(reader["QuotationId"]),
                                QuotationTitle = reader["QuotationTitle"].ToString(),
                                Description = reader["Description"].ToString(),
                                VendorId = Convert.ToInt32(reader["VendorId"]),
                                QuotationRequestId = Convert.ToInt32(reader["QuotationRequestId"]),
                                QuotationAmount = Convert.ToDecimal(reader["QuotationAmount"]),
                                SubmissionDate = Convert.ToDateTime(reader["SubmissionDate"]),
                                Status = reader["Status"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

                public List<QuotationComparisonViewModel> CompareVendorsByRequestId(int quotationRequestId)
        {
            var comparisonList = new List<QuotationComparisonViewModel>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"SELECT vr.""Id"" as QuotationId, vr.""Price"" as QuotationAmount, vr.""Status"", v.""VendorName"", v.""CompanyName"" as VendorCompanyName
                                FROM ""VendorResponses"" vr
                                INNER JOIN ""Vendors"" v ON vr.""VendorId"" = v.""VendorId""
                                WHERE vr.""QuotationRequestId"" = @QuotationRequestId AND vr.""Status"" IN ('Submitted', 'Approved')
                                ORDER BY vr.""Price"" ASC;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@QuotationRequestId", quotationRequestId);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comparisonList.Add(new QuotationComparisonViewModel
                            {
                                QuotationId = Convert.ToInt32(reader["QuotationId"]),
                                VendorName = reader["VendorName"].ToString(),
                                VendorCompanyName = reader["VendorCompanyName"].ToString(),
                                QuotationAmount = Convert.ToDecimal(reader["QuotationAmount"]),
                                Status = reader["Status"].ToString()
                            });
                        }
                    }
                }
            }
            return comparisonList;
        }

        
        public List<SimpleQuotationSummaryViewModel> GetSimpleQuotationSummaries(int companyId)
        {
            var list = new List<SimpleQuotationSummaryViewModel>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"SELECT 
                                    qr.""QuotationRequestId"",
                                    qr.""Title"" as RequestTitle,
                                    qr.""CreatedDate"" as RequestDate
                                FROM ""QuotationRequests"" qr
                                WHERE qr.""CompanyId"" = @CompanyId
                                ORDER BY qr.""CreatedDate"" DESC;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new SimpleQuotationSummaryViewModel
                            {
                                QuotationRequestId = Convert.ToInt32(reader["QuotationRequestId"]),
                                RequestTitle = reader["RequestTitle"].ToString(),
                                RequestDate = Convert.ToDateTime(reader["RequestDate"]),
                                QuotationTitle = "",
                                QuotationStatus = "",
                                QuotationAmount = 0
                            });
                        }
                    }
                }
            }
            return list;
        }

        
        public LowestQuotationViewModel GetCheapestQuotation(int quotationRequestId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"SELECT vr.""Id"" as QuotationId, qr.""Title"" as RequestTitle, v.""VendorName"", v.""CompanyName"" as VendorCompanyName, vr.""Price"" as QuotationAmount, vr.""Status""
                                FROM ""VendorResponses"" vr
                                INNER JOIN ""QuotationRequests"" qr ON vr.""QuotationRequestId"" = qr.""QuotationRequestId""
                                INNER JOIN ""Vendors"" v ON vr.""VendorId"" = v.""VendorId""
                                WHERE vr.""QuotationRequestId"" = @QuotationRequestId AND vr.""Status"" IN ('Submitted', 'Approved')
                                ORDER BY vr.""Price"" ASC
                                LIMIT 1;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@QuotationRequestId", quotationRequestId);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new LowestQuotationViewModel
                            {
                                QuotationId = Convert.ToInt32(reader["QuotationId"]),
                                RequestTitle = reader["RequestTitle"].ToString(),
                                VendorName = reader["VendorName"].ToString(),
                                VendorCompanyName = reader["VendorCompanyName"].ToString(),
                                QuotationAmount = Convert.ToDecimal(reader["QuotationAmount"]),
                                Status = reader["Status"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        
        public List<TrackQuotationViewModel> TrackQuotationsByCompany(int companyId)
        {
            var trackList = new List<TrackQuotationViewModel>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"SELECT q.""QuotationId"", qr.""Title"" as RequestTitle, v.""VendorName"", q.""Status"", q.""QuotationAmount""
                                FROM ""Quotations"" q
                                INNER JOIN ""QuotationRequests"" qr ON q.""QuotationRequestId"" = qr.""QuotationRequestId""
                                INNER JOIN ""Vendors"" v ON q.""VendorId"" = v.""VendorId""
                                WHERE qr.""CompanyId"" = @CompanyId
                                ORDER BY qr.""CreatedDate"" DESC;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            trackList.Add(new TrackQuotationViewModel
                            {
                                QuotationId = Convert.ToInt32(reader["QuotationId"]),
                                RequestTitle = reader["RequestTitle"].ToString(),
                                VendorName = reader["VendorName"].ToString(),
                                Status = reader["Status"].ToString(),
                                QuotationAmount = Convert.ToDecimal(reader["QuotationAmount"])
                            });
                        }
                    }
                }
            }
            return trackList;
        }
    }
}