#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendorHub.Models.Classes
{
    [Table("QuotationRequests")]
    public class QuotationRequest
    {
        [Key]
        [Column("QuotationRequestId")]
        public int QuotationRequestId { get; set; }

        [NotMapped]
        public int Id => QuotationRequestId;

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        [Column("Title")]
        public string Title { get; set; }

        [Column("CompanyId")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        [Column("Description")]
        public string Description { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("Deadline")]
        public DateTime? Deadline { get; set; }

        [Column("Budget")]
        public decimal? Budget { get; set; }

        [Column("Status")]
        public string Status { get; set; } = "Pending";
    }
}
