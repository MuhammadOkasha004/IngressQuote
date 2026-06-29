using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendorHub.Models.Classes
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Column("UserId")]
        public int UserId { get; set; }

        [Column("Username")]
        public string Username { get; set; } = "";

        [Column("Email")]
        public string Email { get; set; } = "";

        [Column("Password")]
        public string Password { get; set; } = "";

        [Column("Role")]
        public string Role { get; set; } = "";

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("SmtpEmail")]
        public string? SmtpEmail { get; set; }

        [Column("SmtpAppPassword")]
        public string? SmtpAppPassword { get; set; }
    }
}