using Ardalis.GuardClauses;
using DistributedWorker.Core.Exception;

namespace DistributedWorker.Core.Domain;

/// <summary>
///     Keeps track of the <see cref="Work" />
/// </summary>
public class Worker
{
    public Worker()
    {
        Status = WorkerStatus.Ready;
        Id = Guid.NewGuid();
    }

    public Work Work { get; set; }

    public WorkerStatus Status
    {
        get;
        private set;
    }

    public Guid Id
    {
        get;
    }

    public bool SetWork(Work work)
    {
        Guard.Against.Null(work, nameof(work));

        Work = work;
        return true;
    }

    public void StartWork()
    {
        if (Work == null)
        {
            throw new WorkException($"No work assigned to worker {Id}");
        }

        Status = WorkerStatus.Working;
        Work.DoWork();
    }

    public void StopWork()
    {
        Status = WorkerStatus.Stopped;
    }
}
