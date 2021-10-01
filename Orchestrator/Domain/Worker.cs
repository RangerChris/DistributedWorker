using Ardalis.GuardClauses;

namespace DistributedWorker.Core.Domain;

public class Worker
{
    private Work? _work;

    public bool SetWork(Work work)
    {
        Guard.Against.Null(work, nameof(work));

        _work = work;
        return true;
    }
}
