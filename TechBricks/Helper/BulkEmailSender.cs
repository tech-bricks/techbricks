using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace TechBricks.Helper
{
    public class BulkEmailSender : IBulkEmailSender
    {
        public async Task SendBulkZohoEmailsAsync(List<string> recipientEmails, string subject, string bodyHtml)
        {
            // Zoho SMTP Configuration
            string smtpHost = "smtp.zoho.com";
            int smtpPort = 465;
            string zohoUsername = "info@tech-bricks.com";
            string zohoPassword = "mzgubTCfd8cg"; // Use an App Password for security

            using (var client = new SmtpClient())
            {
                try
                {
                    // 1. Connect and Authenticate ONCE
                    await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(zohoUsername, zohoPassword);

                    foreach (var email in recipientEmails)
                    {
                        try
                        {
                            var message = new MimeMessage();
                            message.From.Add(new MailboxAddress("Tech Bricks", zohoUsername));
                            message.To.Add(new MailboxAddress(null, email));
                            message.Subject = subject;

                            var bodyBuilder = new BodyBuilder { HtmlBody = bodyHtml };
                            message.Body = bodyBuilder.ToMessageBody();

                            // 2. Send within the existing connection
                            await client.SendAsync(message);
                            Console.WriteLine($"Email sent to {email}");

                            // 3. Optional: Small delay to stay within Zoho's rate limits
                            // await Task.Delay(200); 
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to send to {email}: {ex.Message}");
                            // Continue to next recipient even if one fails
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SMTP Connection Error: {ex.Message}");
                }
                finally
                {
                    // 4. Disconnect once after all emails are processed
                    if (client.IsConnected)
                        await client.DisconnectAsync(true);
                }
            }
        }
    }
}
