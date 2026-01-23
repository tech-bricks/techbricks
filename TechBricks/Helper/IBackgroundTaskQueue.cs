using System;
using System.Threading;
using System.Threading.Tasks;

namespace TechBricks.Background
{
    // Lightweight background task queue contract
    public interface IBackgroundTaskQueue
    {
        ValueTask QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
        ValueTask<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}