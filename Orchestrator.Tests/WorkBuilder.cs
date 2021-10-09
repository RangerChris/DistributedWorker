using System.Collections.Generic;
using DistributedWorker.Core.Domain;

namespace DistributedWorker.Core.Tests;

public class WorkBuilder
{
    private readonly List<Work> _workList = new List<Work>();

    public WorkBuilder CreateWork(int numberOfWorkers, bool canFail)
    {
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
