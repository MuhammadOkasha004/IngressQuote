using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VendorHub.Data;
using VendorHub.Models.Classes;

namespace VendorHub.Models.Repositories
{
    public class InvitationRepository
    {
        private readonly ApplicationDbContext _context;

        public InvitationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<VendorInvitation?> GetByTokenAsync(string token)
        {
            return await _context.VendorInvitations
                .FirstOrDefaultAsync(i => i.Token == token);
        }

        public async Task CreateInvitationAsync(VendorInvitation invitation)
        {
            _context.VendorInvitations.Add(invitation);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsUsedAsync(string token)
        {
            var invitation = await _context.VendorInvitations.FindAsync(token);
            if (invitation != null)
            {
                invitation.IsUsed = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<QuotationAssignment>> GetAssignmentsByQuotationAsync(int quotationId)
        {
            return await _context.QuotationAssignments
                .Include(a => a.Vendor)
                .Where(a => a.QuotationRequestId == quotationId)
                .OrderByDescending(a => a.AssignedAt)
                .ToListAsync();
        }

        public async Task<List<QuotationAssignment>> GetAssignmentsByVendorAsync(int vendorId)
        {
            return await _context.QuotationAssignments
                .Include(a => a.QuotationRequest)
                .Where(a => a.VendorId == vendorId)
                .OrderByDescending(a => a.AssignedAt)
                .ToListAsync();
        }

        public async Task<QuotationAssignment?> GetAssignmentByIdAsync(int id)
        {
            return await _context.QuotationAssignments
                .Include(a => a.QuotationRequest)
                .Include(a => a.Vendor)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task CreateAssignmentAsync(QuotationAssignment assignment)
        {
            _context.QuotationAssignments.Add(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAssignmentStatusAsync(int assignmentId, string status)
        {
            var assignment = await _context.QuotationAssignments.FindAsync(assignmentId);
            if (assignment != null)
            {
                assignment.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateResponseAsync(VendorResponse response)
        {
            _context.VendorResponses.Add(response);
            await _context.SaveChangesAsync();
        }

        public async Task<List<VendorResponse>> GetAllResponsesAsync()
        {
            return await _context.VendorResponses
                .Include(r => r.Vendor)
                .Include(r => r.QuotationRequest)
                .OrderByDescending(r => r.SubmittedAt)
                .ToListAsync();
        }

        public async Task<List<VendorResponse>> GetResponsesByVendorAsync(int vendorId)
        {
            return await _context.VendorResponses
                .Include(r => r.QuotationRequest)
                .Where(r => r.VendorId == vendorId)
                .OrderByDescending(r => r.SubmittedAt)
                .ToListAsync();
        }

        public async Task<VendorResponse?> GetResponseByAssignmentAsync(int assignmentId)
        {
            return await _context.VendorResponses
                .Include(r => r.Vendor)
                .Include(r => r.QuotationRequest)
                .FirstOrDefaultAsync(r => r.QuotationAssignmentId == assignmentId);
        }

        public async Task<Vendor?> GetVendorByEmailAsync(string email)
        {
            return await _context.Vendors.FirstOrDefaultAsync(v => v.EmailAddress == email);
        }
    }
}
