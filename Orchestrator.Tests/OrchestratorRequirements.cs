using System.Linq;
using Bogus;
using DistributedWorker.Core.Domain;
using FluentAssertions;
using Xunit;

namespace DistributedWorker.Core.Tests;

public class OrchestratorRequirements
{
    private readonly Faker<Orchestrator> _fakeOrchestrator = new();
    private readonly Faker<Work> _fakeWork = new();
    private readonly Faker<Worker> _fakeWorker = new();

    [Fact]
    public void OrchestratorCanAssignWorkToWorker()
    {
        var orchestrator = _fakeOrchestrator.Generate();
        var worker = _fakeWorker.Generate();
        var work = _fakeWork.Generate();

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
}
