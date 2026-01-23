using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TechBricks.Models;
using System.IO;

namespace TechBricks.Helper
{
    public class BulkEmailSender : IBulkEmailSender
    {
        public async Task<int> SendBulkZohoEmailsAsync(List<EmailRecipient> recipients, string subject, string bodyHtmlTemplate, string? attachmentPath = null)
        {
            int successCount = 0;

            // Zoho SMTP Configuration - please keep secrets out of code in production (use configuration/secret store)
            string smtpHost = "smtp.zoho.com";
            int smtpPort = 465;
            string zohoUsername = "info@tech-bricks.com";
            string zohoPassword = "mzgubTCfd8cg"; // Use an App Password for security

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(zohoUsername, zohoPassword);

                foreach (var recipient in recipients)
                {
                    try
                    {
                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress("Tech Bricks", zohoUsername));
                        message.To.Add(new MailboxAddress(recipient.Name, recipient.Email));
                        message.Subject = subject ?? "";

                        // Replace common placeholder formats for name in the template.
                        string personalizedHtml = bodyHtmlTemplate ?? "";
                        if (!string.IsNullOrWhiteSpace(recipient.Name))
                        {
                            personalizedHtml = personalizedHtml
                                .Replace("{{Name}}", recipient.Name, StringComparison.OrdinalIgnoreCase)
                                .Replace("{Name}", recipient.Name, StringComparison.OrdinalIgnoreCase)
                                .Replace("%%Name%%", recipient.Name, StringComparison.OrdinalIgnoreCase)
                                .Replace("%%=v(@Name)=%%", recipient.Name, StringComparison.OrdinalIgnoreCase);
                        }

                        var bodyBuilder = new BodyBuilder { HtmlBody = personalizedHtml };

                        if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                        {
                            bodyBuilder.Attachments.Add(attachmentPath);
                        }

                        message.Body = bodyBuilder.ToMessageBody();

                        await client.SendAsync(message);

                        // increment only on successful send
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        // log or handle per-recipient failure as needed
                        Console.WriteLine($"Failed to send to {recipient.Email}: {ex.Message}");
                        // continue sending to other recipients
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SMTP Connection Error: {ex.Message}");
            }
            finally
            {
                if (client.IsConnected)
                    await client.DisconnectAsync(true);
            }

            return successCount;
        }
    }
}