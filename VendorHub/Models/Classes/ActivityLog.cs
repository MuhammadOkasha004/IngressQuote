using System;

namespace VendorHub.Models.Classes
{
    public class ActivityLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
