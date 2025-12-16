namespace TechBricks.Models
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587; // Or 465 for SMTPS
        public string SenderName { get; set; } = "Tech Bricks Contact Form";
        public string SenderEmail { get; set; } = "your_gmail_address@gmail.com"; // Your Gmail address
        public string Password { get; set; } // The App Password you generated
    }
}
