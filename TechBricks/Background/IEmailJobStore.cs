using TechBricks.Models;

namespace TechBricks.Background
{
    public interface IEmailJobStore
    {
        EmailJob CreateJob(string fileName, int totalRecipients);
        void MarkRunning(Guid jobId);
        void MarkCompleted(Guid jobId, int sent);
        void MarkFailed(Guid jobId, string errorMessage);
        IEnumerable<EmailJob> GetAllJobs();
        EmailJob? GetJob(Guid jobId);
    }
}