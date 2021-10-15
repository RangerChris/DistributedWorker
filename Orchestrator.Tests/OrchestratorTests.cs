using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using DistributedWorker.Core.Domain;
using DistributedWorker.Core.Exception;
using DistributedWorker.Core.Factory;
using FluentAssertions;
using Xunit;

namespace DistributedWorker.Core.Tests;

public class OrchestratorTests
{
    private readonly Orchestrator _orchestrator;
    private readonly WorkBuilder _workBuilder;

    public OrchestratorTests()
    {
        _orchestrator = new Orchestrator();
        _workBuilder = new WorkBuilder();
    }

    [Fact]
    public void OrchestratorCanAssignWorkToWorker()
    {
        var worker = _orchestrator.AddWorker();
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
        try
        {
            _orchestrator.AddWorker(numberOfWorkers);
            _orchestrator.NumberOfWorkers.Should()
                         .Be(numberOfWorkers);
        }
        catch (ArgumentException e)
        {
            e.Should()
             .BeOfType<ArgumentException>();
            e.Message.Should()
             .Be("Required input numberOfWorkers cannot be zero or negative. (Parameter 'numberOfWorkers')");
        }
    }

    [Fact]
    public void OrchestratorAddRemoveGetWorker()
    {
        _orchestrator.AddWorker();
        var retrievedWorker = _orchestrator.GetNextAvailableWorker();
        var removeSuccess = _orchestrator.RemoveWorker(retrievedWorker);
        removeSuccess.Should()
                     .BeTrue();

        retrievedWorker = _orchestrator.GetNextAvailableWorker();
        retrievedWorker.Should()
                       .BeNull();
    }

    [Fact]
    public void CanAddWorkToOrchestrator()
    {
        var faker = new Faker();
        var numberOfWork = faker.Random.Number(1, 20);
        var workList = _workBuilder.CreateWork(numberOfWork, true)
                                   .Build();
        _orchestrator.AddWork(workList);
        _orchestrator.WorkQueueSize.Should()
                     .Be(numberOfWork);
        _orchestrator.GetWorkList()
                     .Count()
                     .Should()
                     .Be(numberOfWork);
    }

    [Fact]
    public void OrchestratorCanCreateWorker()
    {
        var worker = _orchestrator.AddWorker();
        worker.Id.Should()
              .NotBeEmpty();
        worker.Name.Should()
              .Be("Worker-1");
    }

    [Fact]
    public async Task OrchestratorCanStartStopWorker()
    {
        var worker = _orchestrator.AddWorker();
        var work = _workBuilder.CreateWork(1, false)
                               .Build()
                               .First();
        Func<Task> asyncCall = async () =>
        {
            await _orchestrator.StartWorker(worker);
        };

        // Throw exception because the worker have not had any work assigned to it
        await asyncCall.Should()
                       .ThrowAsync<WorkException>();

        _orchestrator.AssignWorkToWorker(worker, work);
        worker.Work.Should()
              .BeSameAs(work);

        work.WorkDuration = 5;
        work.TimeLimit = new TimeSpan(0, 0, 0, 10);

        _orchestrator.StartWorker(worker)
                     .FireAndForget();

        _orchestrator.GetWorkerStatus(worker)
                     .Should()
                     .Be(WorkStatus.Working);

        _orchestrator.StopWorker(worker);
        _orchestrator.GetWorkerStatus(worker)
                     .Should()
                     .Be(WorkStatus.Stopped);
    }
}
