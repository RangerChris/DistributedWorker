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
}
