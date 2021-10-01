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
    private readonly Faker<Orchestrator> _fakeOrchestrator = new();
    private readonly Faker<Work> _fakeWork = new();
    private readonly Faker<Worker> _fakeWorker = new();

    public OrchestratorRequirements()
    {
        _fakeOrchestrator.RuleFor(p => p.Id, p => Guid.NewGuid());
        _fakeWorker.RuleFor(p => p.Id, p => Guid.NewGuid());
        _fakeWork.RuleFor(p => p.Id, p => Guid.NewGuid());
    }

    [Fact]
    public void OrchestratorCanAssignWorkToWorker()
    {
        var orchestrator = _fakeOrchestrator.Generate();
        var worker = _fakeWorker.Generate();
        var work = _fakeWork.Generate();

        orchestrator.Workers.Add(worker);
        var assignmentSuccess = orchestrator.AssignWorkToWorker(worker, work);
        assignmentSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(3)]
    [InlineData(0)]
    [InlineData(263)]
    [InlineData(62)]
    public void OrchestratorAddRangeOfWorkers(int numberOfWorkers)
    {
        var orchestrator = _fakeOrchestrator.Generate();
        var workerList = _fakeWorker.Generate(numberOfWorkers);
        orchestrator.Workers.AddRange(workerList);
        orchestrator.Workers.Should().HaveCount(numberOfWorkers);
    }

    [Fact]
    public void OrchestratorAddRemoveGetWorker()
    {
        var orchestrator = _fakeOrchestrator.Generate();
        var worker = _fakeWorker.Generate();
        orchestrator.Workers.Add(worker);

        orchestrator.Workers.Should().HaveCount(1);

        var retrievedWorker = orchestrator.Workers.First();
        retrievedWorker.Should().BeSameAs(worker);

        orchestrator.Workers.Remove(retrievedWorker);
        orchestrator.Workers.Should().BeEmpty();
    }

    [Fact]
    public void OrchestratorCanStartStopWorker()
    {
        var orchestrator = _fakeOrchestrator.Generate();
        var worker = _fakeWorker.Generate();
        var work = _fakeWork.Generate();

        Action action = () => orchestrator.StartWork(worker);
        // Throw exception because the worker is not added to list of workers known by orchestrator
        action.Should().Throw<WorkerException>();

        orchestrator.Workers.Add(worker);

        action = () => orchestrator.StartWork(worker);
        // Throw exception because the worker have not had any work assigned to it
        action.Should().Throw<WorkException>();

        var result = orchestrator.AssignWorkToWorker(worker, work);
        result.Should().BeTrue();

        orchestrator.StartWork(worker);

        worker.Status.Should().Be(WorkerStatus.Working);

        orchestrator.StopWork(worker);
        worker.Status.Should().Be(WorkerStatus.Stopped);
    }
}
