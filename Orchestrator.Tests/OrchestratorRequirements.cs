using System;
using System.Linq;
using Bogus;
using DistributedWorker.Core.Domain;
using DistributedWorker.Core.Exception;
using FluentAssertions;
using Xunit;

namespace DistributedWorker.Core.Tests;

public class OrchestratorRequirements
{
    private readonly Orchestrator _orchestrator;
    private readonly Work _work;
    private readonly Worker _worker;

    public OrchestratorRequirements()
    {
        _orchestrator = new Faker<Orchestrator>().RuleFor(p => p.Id, Guid.NewGuid())
                                                 .Generate();
        _work = new Faker<Work>().RuleFor(p => p.Id, Guid.NewGuid())
                                 .RuleFor(p => p.Name, $"Work-{_orchestrator.Workers.Count + 1}")
                                 .Generate();
        _worker = new Faker<Worker>().RuleFor(p => p.Id, Guid.NewGuid())
                                     .Generate();

        _orchestrator.Id.Should()
                     .NotBeEmpty();
        _worker.Id.Should()
               .NotBeEmpty();
        _work.Id.Should()
             .NotBeEmpty();
    }


    [Fact]
    public void OrchestratorCanAssignWorkToWorker()
    {
        _orchestrator.Workers.Add(_worker);
        _orchestrator.AssignWorkToWorker(_worker, _work);
        _worker.Work.Should()
               .BeSameAs(_work);

        _worker.Work.WorkStartedAt.Should()
               .Be(DateTime.MinValue);
        _worker.Work.TimeLimit.Should()
               .Be(DateTime.MinValue);
        _worker.Work.Id.Should()
               .NotBeEmpty();
        _worker.Work.Name.Should()
               .Be("Work-1");
    }

    [Theory]
    [InlineData(3)]
    [InlineData(0)]
    [InlineData(263)]
    [InlineData(62)]
    public void OrchestratorAddRangeOfWorkers(int numberOfWorkers)
    {
        var fakeWorker = new Faker<Worker>().RuleFor(p => p.Id, Guid.NewGuid());
        var workerList = fakeWorker.Generate(numberOfWorkers);
        _orchestrator.Workers.AddRange(workerList);
        _orchestrator.Workers.Should()
                     .HaveCount(numberOfWorkers);
    }

    [Fact]
    public void OrchestratorAddRemoveGetWorker()
    {
        _orchestrator.Workers.Add(_worker);

        _orchestrator.Workers.Should()
                     .HaveCount(1);

        var retrievedWorker = _orchestrator.Workers.First();
        retrievedWorker.Should()
                       .BeSameAs(_worker);

        _orchestrator.Workers.Remove(retrievedWorker);
        _orchestrator.Workers.Should()
                     .BeEmpty();
    }

    [Fact]
    public void OrchestratorCanStartStopWorker()
    {
        Action action = () => _orchestrator.StartWork(_worker);
        // Throw exception because the worker is not added to list of workers known by orchestrator
        action.Should()
              .Throw<WorkerException>();

        _orchestrator.Workers.Add(_worker);

        action = () => _orchestrator.StartWork(_worker);
        // Throw exception because the worker have not had any work assigned to it
        action.Should()
              .Throw<WorkException>();

        _orchestrator.AssignWorkToWorker(_worker, _work);
        _worker.Work.Should()
               .BeSameAs(_work);

        _orchestrator.StartWork(_worker);

        _orchestrator.GetStatus(_worker)
                     .Should()
                     .Be(WorkerStatus.Working);

        _orchestrator.StopWork(_worker);
        _orchestrator.GetStatus(_worker)
                     .Should()
                     .Be(WorkerStatus.Stopped);
    }

    [Fact]
    public void WorkerCanSetWorkTimeButOnlyInFuture()
    {
        _orchestrator.Workers.Add(_worker);
        Action action = () => _work.TimeLimit = DateTime.Now.AddHours(-1);
        action.Should()
              .Throw<WorkException>();

        var timeLimit = DateTime.Now.AddHours(1);
        _work.TimeLimit = timeLimit;

        _orchestrator.AssignWorkToWorker(_worker, _work);
        _orchestrator.StartWork(_worker);

        _work.TimeLimit.Should()
             .Be(timeLimit);
        _work.WorkStartedAt.Should()
             .BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }
}
