using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendorHub.Models.Classes
{
    public class QuotationAssignment
    {
        public int Id { get; set; }
        public int QuotationRequestId { get; set; }
        public int VendorId { get; set; }
        public string Status { get; set; } = "Invited";
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? InvitationSentAt { get; set; }

        [ForeignKey("QuotationRequestId")]
        public QuotationRequest? QuotationRequest { get; set; }

        [ForeignKey("VendorId")]
        public Vendor? Vendor { get; set; }
    }
}
