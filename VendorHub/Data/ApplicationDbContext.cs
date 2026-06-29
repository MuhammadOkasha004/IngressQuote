using Microsoft.EntityFrameworkCore;
using VendorHub.Models.Classes;

namespace VendorHub.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<QuotationRequest> QuotationRequests { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<VendorInvitation> VendorInvitations { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<VendorResponse> VendorResponses { get; set; }
        public DbSet<QuotationAssignment> QuotationAssignments { get; set; }
    }
}
