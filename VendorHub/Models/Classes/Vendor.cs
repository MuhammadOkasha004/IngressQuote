#nullable disable
using System.ComponentModel.DataAnnotations;

namespace VendorHub.Models.Classes
{
    public class Vendor
    {
        public int VendorId { get; set; }

        [Required(ErrorMessage = "Vendor name is required.")]
        [StringLength(100, ErrorMessage = "Vendor name cannot exceed 100 characters.")]
        public string VendorName { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters.")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [StringLength(256, ErrorMessage = "Email address cannot exceed 256 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Contact number is required.")]
        [StringLength(20, ErrorMessage = "Contact number cannot exceed 20 characters.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "Business address is required.")]
        [StringLength(500, ErrorMessage = "Business address cannot exceed 500 characters.")]
        public string BusinessAddress { get; set; }
        public int CompanyId { get; set; }//FK

    }
}
