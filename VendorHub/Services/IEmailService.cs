using System.Threading.Tasks;

namespace VendorHub.Services
{
    public interface IEmailService
    {
        Task SendInvitationEmailAsync(string toEmail, string vendorName, string quotationTitle, string invitationLink, string senderEmail);
        Task SendGenericEmailAsync(string toEmail, string subject, string body, string? fromDisplayName = null, string? replyToEmail = null);
    }
}
