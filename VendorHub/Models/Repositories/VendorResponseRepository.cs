using Npgsql;
using System;
using System.Collections.Generic;
using VendorHub.Models.Classes;

namespace VendorHub.Models.Repositories
{
    public class VendorResponseRepository
    {
        private readonly string _connectionString = "Host=ep-plain-waterfall-at1aeg2h-pooler.c-9.us-east-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_HSh1KC9zfnkl;SSL Mode=Require;Trust Server Certificate=true;";

        public List<VendorResponse> GetAssignedRequests(int vendorId)
        {
            var list = new List<VendorResponse>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM ""VendorResponses"" WHERE ""VendorId"" = @VendorId ORDER BY ""SubmittedAt"" DESC;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VendorId", vendorId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(MapReaderToVendorResponse(reader));
                        }
                    }
                }
            }
            return list;
        }

        public int SubmitResponse(VendorResponse response)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"INSERT INTO ""VendorResponses"" 
                                (""QuotationAssignmentId"", ""VendorId"", ""Price"", ""Notes"", ""DeliveryDays"", ""SubmittedAt"", ""Status"") 
                                VALUES (@QaId, @VendorId, @Price, @Notes, @DeliveryDays, @SubmittedAt, @Status)
                                RETURNING ""Id"";";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@QaId", response.QuotationAssignmentId);
                    cmd.Parameters.AddWithValue("@VendorId", response.VendorId);
                    cmd.Parameters.AddWithValue("@Price", response.Price);
                    cmd.Parameters.AddWithValue("@Notes", (object?)response.Notes ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DeliveryDays", response.DeliveryDays);
                    cmd.Parameters.AddWithValue("@SubmittedAt", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@Status", "Responded");
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public VendorResponse GetResponseById(int id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM ""VendorResponses"" WHERE ""Id"" = @Id;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return MapReaderToVendorResponse(reader);
                    }
                }
            }
            return null;
        }

        public List<VendorResponse> GetResponsesByQuotationAssignment(int assignmentId)
        {
            var list = new List<VendorResponse>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM ""VendorResponses"" WHERE ""QuotationAssignmentId"" = @Id ORDER BY ""Price"" ASC;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", assignmentId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            list.Add(MapReaderToVendorResponse(reader));
                    }
                }
            }
            return list;
        }

        public void UpdateStatus(int id, string status)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"UPDATE ""VendorResponses"" SET ""Status"" = @Status WHERE ""Id"" = @Id;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Status", status);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RejectOtherResponses(int quotationRequestId, int excludeResponseId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"UPDATE ""VendorResponses"" vr SET ""Status"" = 'Rejected'
                                FROM ""QuotationAssignments"" qa
                                WHERE vr.""QuotationAssignmentId"" = qa.""Id""
                                AND qa.""QuotationRequestId"" = @QrId
                                AND vr.""Id"" != @ExcludeId;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@QrId", quotationRequestId);
                    cmd.Parameters.AddWithValue("@ExcludeId", excludeResponseId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private VendorResponse MapReaderToVendorResponse(NpgsqlDataReader reader)
        {
            return new VendorResponse
            {
                Id = Convert.ToInt32(reader["Id"]),
                QuotationAssignmentId = Convert.ToInt32(reader["QuotationAssignmentId"]),
                VendorId = Convert.ToInt32(reader["VendorId"]),
                Price = Convert.ToDecimal(reader["Price"]),
                Notes = reader["Notes"] != DBNull.Value ? reader["Notes"].ToString() : null,
                DeliveryDays = Convert.ToInt32(reader["DeliveryDays"]),
                SubmittedAt = Convert.ToDateTime(reader["SubmittedAt"]),
                Status = reader["Status"].ToString()
            };
        }
    }
}
