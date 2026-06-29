using Npgsql;
using System;
using System.Collections.Generic;
using VendorHub.Models.Classes;

namespace VendorHub.Models.Repositories
{
    public class ActivityLogRepository
    {
        private readonly string _connectionString = "Host=ep-plain-waterfall-at1aeg2h-pooler.c-9.us-east-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_HSh1KC9zfnkl;SSL Mode=Require;Trust Server Certificate=true;";

        public void LogActivity(int userId, string action, string description)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"INSERT INTO ""ActivityLogs"" (""UserId"", ""Action"", ""Description"", ""CreatedAt"") 
                                VALUES (@UserId, @Action, @Description, @CreatedAt);";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Action", action);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<ActivityLog> GetRecentActivities(int companyId, int count = 5)
        {
            var list = new List<ActivityLog>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM ""ActivityLogs"" ORDER BY ""CreatedAt"" DESC LIMIT @Count;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Count", count);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ActivityLog
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                UserId = Convert.ToInt32(reader["UserId"]),
                                Action = reader["Action"].ToString(),
                                Description = reader["Description"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}
