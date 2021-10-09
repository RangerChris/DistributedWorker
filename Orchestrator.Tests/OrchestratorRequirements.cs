using System;
using System.Linq;
using System.Threading.Tasks;
using DistributedWorker.Core.Domain;
using DistributedWorker.Core.Exception;
using FluentAssertions;
using Xunit;

namespace DistributedWorker.Core.Tests;

public class OrchestratorRequirements
{
    private readonly Orchestrator _orchestrator;
    private readonly WorkBuilder _workBuilder;

    public OrchestratorRequirements()
    {
        _orchestrator = new Orchestrator();
        _workBuilder = new WorkBuilder();
    }

    [Fact]
    public void OrchestratorCanAssignWorkToWorker()
    {
        var worker = _orchestrator.CreateWorker();
        var work = _workBuilder.CreateWork(1, false)
                               .Build()
                               .First();
        _orchestrator.AssignWorkToWorker(worker, work);
        worker.Work.Should()
              .BeSameAs(work);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(0)]
    [InlineData(263)]
    [InlineData(62)]
    public void OrchestratorAddRangeOfWorkers(int numberOfWorkers)
    {
        var workerList = _workBuilder.CreateWork(numberOfWorkers, false)
                                     .Build();
        workerList.Should()
                  .HaveCount(numberOfWorkers);
    }

    [Fact]
    public void OrchestratorAddRemoveGetWorker()
    {
        _orchestrator.CreateWorker();
        var retrievedWorker = _orchestrator.GetNextAvailableWorker();
        var removeSuccess = _orchestrator.RemoveWorker(retrievedWorker);
        removeSuccess.Should()
                     .BeTrue();
    }

    [Fact]
    public void OrchestratorCanCreateWorker()
    {
        var worker = _orchestrator.CreateWorker();
        worker.Id.Should()
              .NotBeEmpty();
        worker.Name.Should()
              .Be("Worker-1");
    }

    [Fact]
    public async Task OrchestratorCanStartStopWorker()
    {
        var worker = _orchestrator.CreateWorker();
        var work = _workBuilder.CreateWork(1, false)
                               .Build()
                               .First();
        Func<Task> asyncCall = async () =>
        {
            await _orchestrator.StartWork(worker);
        };

        // Throw exception because the worker have not had any work assigned to it
        await asyncCall.Should()
                       .ThrowAsync<WorkException>();

        _orchestrator.AssignWorkToWorker(worker, work);
        worker.Work.Should()
              .BeSameAs(work);

        work.WorkDuration = 5;
        work.TimeLimit = new TimeSpan(0, 0, 0, 10);

        _orchestrator.StartWork(worker)
                     .FireAndForget();

        _orchestrator.GetStatus(worker)
                     .Should()
                     .Be(WorkStatus.Working);

        _orchestrator.StopWork(worker);
        _orchestrator.GetStatus(worker)
                     .Should()
                     .Be(WorkStatus.Stopped);
    }
}
