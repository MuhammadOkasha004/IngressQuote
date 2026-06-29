using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace VendorHub.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderPassword;
        private readonly string _senderName;
        private readonly bool _enableSsl;

        public EmailService()
        {
            _smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "";
            _smtpPort = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var port) ? port : 587;
            _senderEmail = Environment.GetEnvironmentVariable("SMTP_SENDER_EMAIL") ?? "";
            _senderPassword = Environment.GetEnvironmentVariable("SMTP_SENDER_PASSWORD") ?? "";
            _senderName = Environment.GetEnvironmentVariable("SMTP_SENDER_NAME") ?? "IngressQuote System";
            _enableSsl = true;
        }

        public async Task SendInvitationEmailAsync(string toEmail, string vendorName, string quotationTitle, string invitationLink, string senderEmail)
        {
            string subject = "IngressQuote — You have been invited to quote";
            string body = $@"<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'></head>
<body style='margin:0;padding:0;background-color:#0A192F;font-family:Inter,system-ui,sans-serif;'>
<table width='100%' cellpadding='0' cellspacing='0' style='background-color:#0A192F;padding:40px 20px;'>
<tr><td align='center'>
<table width='600' cellpadding='0' cellspacing='0' style='background-color:#1E293B;border-radius:16px;overflow:hidden;border:1px solid #1E3A5F;'>
<tr><td style='padding:40px 30px;text-align:center;background-color:#112240;'>
<h1 style='color:#14B8A6;font-size:28px;margin:0;'>IngressQuote</h1>
<p style='color:#94A3B8;font-size:14px;margin:8px 0 0;'>Your Gateway to Smart Procurement</p>
</td></tr>
<tr><td style='padding:30px;'>
<h2 style='color:#E2E8F0;font-size:22px;margin:0 0 20px;'>You're Invited to Quote!</h2>
<p style='color:#94A3B8;font-size:14px;margin:0 0 16px;'>Invited by: <strong style='color:#E2E8F0;'>{senderEmail}</strong></p>
<p style='color:#E2E8F0;font-size:16px;line-height:1.6;'>Hello <strong style='color:#14B8A6;'>{vendorName}</strong>,</p>
<p style='color:#94A3B8;font-size:15px;line-height:1.6;'>You have been invited to submit a quotation for the following request:</p>
<div style='background:#0A192F;border:1px solid #1E3A5F;border-radius:8px;padding:16px;margin:20px 0;'>
<p style='color:#E2E8F0;font-size:16px;margin:0;'><strong>{quotationTitle}</strong></p>
</div>
<p style='color:#94A3B8;font-size:15px;line-height:1.6;margin-bottom:24px;'>Click the button below to create your account and submit your quotation.</p>
<a href='{invitationLink}' style='display:inline-block;background-color:#14B8A6;color:#ffffff;text-decoration:none;padding:14px 32px;border-radius:8px;font-size:16px;font-weight:600;'>Accept Invitation &amp; Submit Quote</a>
<table cellpadding='0' cellspacing='0' style='margin:16px auto 0;'><tr><td style='background:#1E293B;border:1px solid #1E3A5F;border-radius:8px;padding:12px 20px;'><p style='color:#94A3B8;font-size:13px;margin:0 0 4px;'>If the button doesn't work, copy and paste this link:</p><p style='color:#14B8A6;font-size:13px;margin:0;word-break:break-all;'>{invitationLink}</p></td></tr></table>
<p style='color:#64748B;font-size:13px;margin-top:24px;'>This invitation link will expire in 3 days.</p>
</td></tr>
<tr><td style='padding:20px 30px;text-align:center;border-top:1px solid #1E3A5F;'>
<p style='color:#64748B;font-size:12px;margin:0;'>IngressQuote — Your Gateway to Smart Procurement</p>
</td></tr>
</table>
</td></tr>
</table>
</body>
</html>";
            Console.WriteLine($"[EmailService] Sending invitation to {toEmail} | SenderEmail: {senderEmail} | ReplyTo: {senderEmail} | Link: {invitationLink}");
            await SendGenericEmailAsync(toEmail, subject, body, fromDisplayName: senderEmail, replyToEmail: senderEmail);
        }

        public async Task SendGenericEmailAsync(string toEmail, string subject, string body, string? fromDisplayName = null, string? replyToEmail = null)
        {
            if (string.IsNullOrEmpty(_smtpHost) || string.IsNullOrEmpty(_senderEmail))
            {
                Console.WriteLine($"[EmailService] FAILED: SMTP environment variables not found or empty.");
                throw new InvalidOperationException("SMTP environment variables not found or empty. Make sure .env file exists at project root, has real values filled in for SMTP_SENDER_EMAIL and SMTP_SENDER_PASSWORD, and DotNetEnv.Env.Load() runs before this service is used.");
            }

            string displayName = fromDisplayName ?? _senderName;
            Console.WriteLine($"[EmailService] Host={_smtpHost}:{_smtpPort} | Auth={_senderEmail} | PasswordLength={_senderPassword.Length} | EnableSsl={_enableSsl} | FromDisplayName={displayName} | ReplyTo={replyToEmail ?? "(none)"}");

            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(_senderEmail, _senderPassword),
                EnableSsl = _enableSsl
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_senderEmail, displayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);
            if (!string.IsNullOrEmpty(replyToEmail))
                message.ReplyToList.Add(new MailAddress(replyToEmail));

            try
            {
                await client.SendMailAsync(message);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"[EmailService] SMTP FAILED: {ex.Message}");
                throw new InvalidOperationException(
                    "Gmail SMTP authentication failed. Make sure SMTP_SENDER_EMAIL and SMTP_SENDER_PASSWORD in the .env file are filled with a real Gmail address and a 16-character Gmail App Password, and that 2-Step Verification is enabled on that Gmail account.", ex);
            }
        }
    }
}
