using Ardalis.GuardClauses;
using DistributedWorker.Core.Domain;

namespace DistributedWorker.Core.Factory;

public class WorkBuilder
{
    private readonly List<Work> _workList = new List<Work>();

    public WorkBuilder CreateWork(int numberOfWorkers, bool canFail)
    {
        Guard.Against.NegativeOrZero(numberOfWorkers, nameof(numberOfWorkers));

        for (var i = 0; i < numberOfWorkers; i++)
        {
            var work = new Work();
            _workList.Add(work);
        }

        return this;
    }

    public List<Work> Build()
    {
        return _workList;
    }
}
