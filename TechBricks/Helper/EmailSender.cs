using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using TechBricks.Models;

namespace TechBricks.Helper
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                // 1. Create the MailMessage
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true // Set to true if the body contains HTML (which is safer)
                };
                mailMessage.To.Add(new MailAddress(toEmail)); // The recipient (your company email)

                // 2. Configure the SmtpClient for Gmail
                using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;

                    // Use your Gmail address and App Password for credentials
                    client.Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.Password);

                    // 3. Send the email
                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                // In a real application, you would log this error.
                // For now, we wrap it in a Task to match the interface.
                throw ex;
            }
        }
    }
}
