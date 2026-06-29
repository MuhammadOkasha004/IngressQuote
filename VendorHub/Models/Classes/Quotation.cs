#nullable disable
using System.ComponentModel.DataAnnotations;

namespace VendorHub.Models.Classes
{
    public class Quotation
    {
        public int QuotationId { get; set; }

        [Required(ErrorMessage = "Quotation title is required.")]
        [StringLength(200, ErrorMessage = "Quotation title cannot exceed 200 characters.")]
        [Display(Name = "Quotation Title")]
        public string QuotationTitle { get; set; }

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vendor is required.")]
        [Display(Name = "Vendor")]
        public int VendorId { get; set; }

        [Required(ErrorMessage = "Quotation request is required.")]
        [Display(Name = "Quotation Request")]
        public int QuotationRequestId { get; set; }

        [Required(ErrorMessage = "Quotation amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quotation amount must be greater than zero.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Quotation Amount")]
        public decimal QuotationAmount { get; set; }

        [Required(ErrorMessage = "Submission date is required.")]
        [Display(Name = "Submission Date")]
        [DataType(DataType.Date)]
        public DateTime SubmissionDate { get; set; }= DateTime.Now;

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; }="Sent";

    }
}
