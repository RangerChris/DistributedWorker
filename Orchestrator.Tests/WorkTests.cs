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

        [Fact]
        public void UseWorkFactoryToCreateWork()
        {
            var sut = new WorkBuilder().CreateWork(10, false)
                                       .Build();

            sut.Count.Should()
               .Be(10);
        }
    }
}
