using Ardalis.GuardClauses;
using DistributedWorker.Core.Exception;

namespace DistributedWorker.Core.Domain;

/// <summary>
///     Keeps track of <see cref="Workers" /> and can issue <see cref="Work" /> to the <see cref="Worker" />
/// </summary>
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

    public void AssignWorkToWorker(Worker worker, Work work)
    {
        Guard.Against.Null(worker, nameof(worker));
        CheckWorkerIsKnownByOrchestrator(worker);
        Guard.Against.Null(work, nameof(work));

        worker.SetWork(work);
    }

    public void StartWork(Worker worker)
    {
        Guard.Against.Null(worker, nameof(worker));
        CheckWorkerIsKnownByOrchestrator(worker);

        worker.StartWork();
    }

    public WorkerStatus GetStatus(Worker worker)
    {
        Guard.Against.Null(worker, nameof(worker));
        CheckWorkerIsKnownByOrchestrator(worker);

        return worker.Status;
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
