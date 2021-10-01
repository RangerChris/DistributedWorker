using Ardalis.GuardClauses;
using DistributedWorker.Core.Exception;

namespace DistributedWorker.Core.Domain;

public class Worker
{
    private Work? _work;

    public Worker()
    {
        Status = WorkerStatus.Ready;
    }

    public WorkerStatus Status
    {
        get;
        private set;
    }

    public Guid Id
    {
        get;
        set;
    }

    public bool SetWork(Work work)
    {
        Guard.Against.Null(work, nameof(work));

        _work = work;
        return true;
    }

    public void StartWork()
    {
        if (_work == null)
        {
            throw new WorkException($"No work assigned to worker {Id}");
        }

        DoWork();
    }

    private void DoWork()
    {
        Status = WorkerStatus.Working;
    }

    public void StopWork()
    {
        Status = WorkerStatus.Stopped;
    }
}
