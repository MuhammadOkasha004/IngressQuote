using System;
using System.ComponentModel.DataAnnotations;

namespace VendorHub.Models.Classes
{
    public class VendorInvitation
    {
        [Key]
        public string Token { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string VendorName { get; set; } = null!;
        public int VendorId { get; set; }
        public int QuotationRequestId { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(48);
    }
}
