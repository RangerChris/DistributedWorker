using Ardalis.GuardClauses;
using DistributedWorker.Core.Exception;

namespace DistributedWorker.Core.Domain;

/// <summary>
///     Keeps track of the <see cref="Work" /> to be done
///     Can only work on one thing at a time, but can spawn many <see cref="Worker" />
/// </summary>
public class Worker
{
    public Worker()
    {
        Id = Guid.NewGuid();
        Name = "";
    }

    public Work? Work { get; set; }

    public Guid Id
    {
        get;
    }

    public string Name
    {
        get;
        set;
    }

    public void SetWork(Work work)
    {
        Guard.Against.Null(work, nameof(work));
        Work = work;
        work.Status = WorkStatus.Ready;
    }

    public async Task StartWork()
    {
        if (Work == null)
        {
            throw new WorkException($"No work assigned to worker {Id}");
        }

        Work.CheckIfValid();

        await Work.DoWork(new CancellationToken());
    }

    public void StopWork()
    {
        if (Work == null)
        {
            throw new WorkException($"No work assigned to worker {Id}");
        }

        Work.StopWork();
    }

    public bool IsReadyForWork()
    {
        if (Work == null || Work.Status == WorkStatus.Finished || Work.Status == WorkStatus.Ready)
        {
            return true;
        }

        return false;
    }
}
