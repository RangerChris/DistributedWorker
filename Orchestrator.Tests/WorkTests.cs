using System;
using DistributedWorker.Core.Domain;
using DistributedWorker.Core.Exception;
using FluentAssertions;
using Xunit;

namespace DistributedWorker.Core.Tests
{
    public class WorkTests
    {
        [Fact]
        public void WorkBasic()
        {
            var work = new Work();
            work.CheckIfValid();

            work.Id = Guid.Empty;

            Assert.Throws<WorkException>(() =>
            {
                work.CheckIfValid();
            });

            work.Id = Guid.NewGuid();
            work.Status = WorkStatus.Failed;

            Assert.Throws<WorkException>(() =>
            {
                work.CheckIfValid();
            });
        }

        [Fact]
        public async void WorkThatFails()
        {
            var worker = new Worker();
            var work = new Work
            {
                Name = "Work that will fail because it time out",
                TimeLimit = new TimeSpan(0, 0, 0, 1),
                WorkDuration = 3
            };
            worker.Work = work;
            await worker.StartWork();
            work.Status.Should()
                .Be(WorkStatus.Failed);
        }

        [Fact]
        public async void WorkThatSucceeds()
        {
            var worker = new Worker();
            var work = new Work
            {
                Name = "Work that will succeed",
                TimeLimit = new TimeSpan(0, 0, 0, 5),
                WorkDuration = 1
            };
            worker.Work = work;
            await worker.StartWork();
            work.Status.Should()
                .Be(WorkStatus.Finished);
        }
    }
}
