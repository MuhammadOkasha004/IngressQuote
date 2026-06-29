using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendorHub.Models.Classes
{
    public class VendorResponse
    {
        public int Id { get; set; }
        public int QuotationAssignmentId { get; set; }
        public int VendorId { get; set; }
        public int QuotationRequestId { get; set; }
        public decimal Price { get; set; }
        public string? Notes { get; set; }
        public int DeliveryDays { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Submitted";

        [ForeignKey("QuotationAssignmentId")]
        public QuotationAssignment? QuotationAssignment { get; set; }

        [ForeignKey("VendorId")]
        public Vendor? Vendor { get; set; }

        [ForeignKey("QuotationRequestId")]
        public QuotationRequest? QuotationRequest { get; set; }
    }
}
