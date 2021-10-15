using System.Collections.ObjectModel;
using Ardalis.GuardClauses;

namespace DistributedWorker.Core.Domain;

/// <summary>
///     Keeps track of <see cref="Workers" /> and can issue <see cref="Work" /> to the <see cref="Worker" />
/// </summary>
public class Orchestrator
{
    public Orchestrator()
    {
        Id = Guid.NewGuid();
        Workers = new List<Worker?>();
        WorkQueue = new PriorityQueue<Work, int>();
    }

    private List<Worker?> Workers
    {
        get;
    }

    private PriorityQueue<Work, int> WorkQueue
    {
        get;
    }

    public Guid Id
    {
        get;
        set;
    }

    public int NumberOfWorkers
    {
        get
        {
            var result = Workers.Count;
            return result;
        }
    }

    public int WorkQueueSize =>
        WorkQueue.Count;

    public ReadOnlyCollection<Worker?> GetWorkerList()
    {
        return Workers.AsReadOnly();
    }

    public void AssignWorkToWorker(Worker? worker, Work work)
    {
        Guard.Against.Null(worker, nameof(worker));
        Guard.Against.Null(work, nameof(work));

        worker.SetWork(work);
    }

    public async Task StartWorker(Worker? worker)
    {
        Guard.Against.Null(worker, nameof(worker));

        await worker.StartWork();
    }

    public WorkStatus GetWorkerStatus(Worker? worker)
    {
        Guard.Against.Null(worker, nameof(worker));

        if (worker.Work == null)
        {
            return WorkStatus.NotReady;
        }

        return worker.Work.Status;
    }

    public void StopWorker(Worker? worker)
    {
        Guard.Against.Null(worker, nameof(worker));
        worker.StopWork();
    }

    public void AddWorker(int numberOfWorkers)
    {
        Guard.Against.NegativeOrZero(numberOfWorkers, nameof(numberOfWorkers));

        for (var i = 0; i < numberOfWorkers; i++)
        {
            var newWorker = new Worker
            {
                Name = $"Worker-{Workers.Count + 1}"
            };
            Workers.Add(newWorker);
        }
    }

    public Worker AddWorker()
    {
        var newWorker = new Worker
        {
            Name = $"Worker-{Workers.Count + 1}"
        };
        Workers.Add(newWorker);
        return newWorker;
    }

    public Worker? GetNextAvailableWorker()
    {
        foreach (var worker in Workers)
        {
            if (worker != null && worker.IsReadyForWork())
            {
                return worker;
            }
        }

        return null;
    }

    public bool RemoveWorker(Worker? worker)
    {
        return Workers.Remove(worker);
    }

    public void AddWork(List<Work> workList)
    {
        Guard.Against.Null(workList, nameof(workList));
        Guard.Against.NegativeOrZero(workList.Count, nameof(workList));

        foreach (var currentWork in workList)
        {
            WorkQueue.Enqueue(currentWork, (int)currentWork.Priority);
        }
    }

    public IEnumerable<Work> GetWorkList()
    {
        var result = new List<Work>(WorkQueueSize);
        while (WorkQueue.TryDequeue(out var item, out var priority))
        {
            result.Add(item);
        }

        return result;
    }
}
