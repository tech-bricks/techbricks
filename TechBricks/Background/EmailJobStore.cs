using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TechBricks.Models;

namespace TechBricks.Background
{
    public class EmailJobStore : IEmailJobStore
    {
        private readonly ConcurrentDictionary<Guid, EmailJob> _store = new();

        public EmailJob CreateJob(string fileName, int totalRecipients)
        {
            var job = new EmailJob
            {
                FileName = fileName,
                TotalRecipients = totalRecipients,
                SentCount = 0,
                FailedCount = 0,
                Status = EmailJobStatus.Queued,
                CreatedAt = DateTime.UtcNow
            };

            _store[job.Id] = job;
            return job;
        }

        public void MarkRunning(Guid jobId)
        {
            if (_store.TryGetValue(jobId, out var job))
            {
                job.Status = EmailJobStatus.Running;
            }
        }

        public void MarkCompleted(Guid jobId, int sent)
        {
            if (_store.TryGetValue(jobId, out var job))
            {
                job.SentCount = sent;
                job.FailedCount = Math.Max(0, job.TotalRecipients - sent);
                job.Status = EmailJobStatus.Completed;
                job.CompletedAt = DateTime.UtcNow;
            }
        }

        public void MarkFailed(Guid jobId, string errorMessage)
        {
            if (_store.TryGetValue(jobId, out var job))
            {
                job.Status = EmailJobStatus.Failed;
                job.ErrorMessage = errorMessage;
                job.CompletedAt = DateTime.UtcNow;
            }
        }

        public IEnumerable<EmailJob> GetAllJobs() => _store.Values;

        public EmailJob? GetJob(Guid jobId) => _store.TryGetValue(jobId, out var job) ? job : null;
    }
}