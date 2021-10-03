using System;
using Bogus;
using DistributedWorker.Core.Domain;
using FluentAssertions;
using Xunit;

namespace DistributedWorker.Core.Tests
{
    public class WorkTests
    {
        private readonly Faker _faker = new();

        [Fact]
        public async void FailingWork()
        {
            var worker = new Worker();
            var work = new Work
            {
                Id = Guid.NewGuid(),
                Name = "Work that will fail because it time out",
                TimeLimit = DateTime.Now.AddSeconds(1),
                WorkDuration = 3
            };
            worker.Work = work;
            await worker.StartWork();
            work.Status.Should()
                .Be(WorkStatus.Failed);
        }
    }
}
