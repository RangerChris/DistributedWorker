using Ardalis.GuardClauses;
using DistributedWorker.Core.Exception;

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

    public Guid Id
    {
        get;
        set;
    }

    public bool AssignWorkToWorker(Worker worker, Work work)
    {
        Guard.Against.Null(worker, nameof(worker));
        CheckWorkerIsKnownByOrchestrator(worker);
        Guard.Against.Null(work, nameof(work));

        var result = worker.SetWork(work);
        return result;
    }

    public void StartWork(Worker worker)
    {
        Guard.Against.Null(worker, nameof(worker));
        CheckWorkerIsKnownByOrchestrator(worker);

        worker.StartWork();
    }

    private void CheckWorkerIsKnownByOrchestrator(Worker worker)
    {
        if (!Workers.Contains(worker))
        {
            throw new WorkerException($"Worker {worker.Id} not known by orchestrator, please add to worker list");
        }
    }

    public void StopWork(Worker worker)
    {
        Guard.Against.Null(worker, nameof(worker));
        CheckWorkerIsKnownByOrchestrator(worker);

        worker.StopWork();
    }
}
