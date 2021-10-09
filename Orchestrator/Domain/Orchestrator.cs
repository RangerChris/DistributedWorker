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
        Id = Guid.NewGuid();
        Workers = new List<Worker>();
    }

    protected List<Worker> Workers
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

    public async Task StartWork(Worker worker)
    {
        Guard.Against.Null(worker, nameof(worker));
        CheckWorkerIsKnownByOrchestrator(worker);

        await worker.StartWork();
    }

    public WorkStatus GetStatus(Worker worker)
    {
        Guard.Against.Null(worker, nameof(worker));
        CheckWorkerIsKnownByOrchestrator(worker);

        if (worker.Work == null)
        {
            return WorkStatus.Failed;
        }

        return worker.Work.Status;
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

    public Worker CreateWorker()
    {
        var newWorker = new Worker
        {
            Name = $"Worker-{Workers.Count + 1}"
        };
        Workers.Add(newWorker);
        return newWorker;
    }

    public Worker GetNextAvailableWorker()
    {
        foreach (var worker in Workers)
        {
            if (worker.IsReadyForWork())
            {
                return worker;
            }
        }

        return null;
    }

    public bool RemoveWorker(Worker worker)
    {
        return Workers.Remove(worker);
    }
}
