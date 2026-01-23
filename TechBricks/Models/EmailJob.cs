using System;

namespace TechBricks.Models
{
    public enum EmailJobStatus
    {
        Queued,
        Running,
        Completed,
        Failed
    }

    public class EmailJob
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FileName { get; set; } = string.Empty;
        public int TotalRecipients { get; set; }
        public int SentCount { get; set; }
        public int FailedCount { get; set; }
        public EmailJobStatus Status { get; set; } = EmailJobStatus.Queued;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public string? ErrorMessage { get; set; }
    }
}