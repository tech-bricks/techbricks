namespace TechBricks.Helper
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }

    // Updated to return number of successfully sent emails
    public interface IBulkEmailSender
    {
        Task<int> SendBulkZohoEmailsAsync(List<TechBricks.Models.EmailRecipient> recipients, string subject, string bodyHtmlTemplate, string? attachmentPath = null);
    }
}
