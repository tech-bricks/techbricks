namespace TechBricks.Helper
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }

    public interface IBulkEmailSender
    {
        Task SendBulkZohoEmailsAsync(List<string> recipientEmails, string subject, string bodyHtml);
    }
}
