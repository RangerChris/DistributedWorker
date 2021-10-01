using Ardalis.GuardClauses;

namespace DistributedWorker.Core.Domain;

public class Orchestrator
{
    public Orchestrator()
    {
        Workers = new List<Worker>();
    }

    public List<Worker> Workers
    {
        get;
    }

    public bool AssignWorkToWorker(Worker worker, Work work)
    {
        Guard.Against.Null(worker, nameof(worker));
        Guard.Against.Null(work, nameof(work));

        var result = worker.SetWork(work);
        return result;
    }
}
