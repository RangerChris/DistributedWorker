using DistributedWorker.Core.Exception;

namespace DistributedWorker.Core.Domain;

/// <summary>
///     Represents a work task that is to be consumed by a <see cref="Worker" />
///     The work have a 5 second time limit, but can be changed at any time.
/// </summary>
public class Work : IWork
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public Work()
    {
        Id = Guid.NewGuid();
        Status = WorkStatus.Ready;
        Name = "";
        TimeLimit = new TimeSpan(0, 0, 0, 5);
    }

    public Guid Id { get; set; }

    public WorkStatus Status
    {
        get;
        set;
    }

    /// <summary>
    ///     How many seconds is set aside to finish the work
    /// </summary>
    public TimeSpan TimeLimit
    {
        get;
        set;
    }

    public DateTime WorkStartedAt { get; set; }

    public string Name { get; set; }

    /// <summary>
    ///     How long will the work run for, NOT the same as <see cref="TimeLimit" />
    /// </summary>
    public int WorkDuration
    {
        get;
        set;
    }

    public WorkPriorityEnum Priority
    {
        get;
        set;
    }

    public async Task DoWork(CancellationToken cancellationToken)
    {
        WorkStartedAt = DateTime.Now;
        Status = WorkStatus.Working;
        for (var i = 0; i < WorkDuration; i++)
        {
            try
            {
                await Task.Delay(1000, _cancellationTokenSource.Token);
                // Check for cancel
                cancellationToken.ThrowIfCancellationRequested();
                // Check if we're over time limit
                if (IsTimeLimitReached())
                {
                    _cancellationTokenSource.Cancel();
                }
            }
            catch (OperationCanceledException)
            {
                Status = WorkStatus.Failed;
                return;
            }
        }

        Status = WorkStatus.Finished;
    }

    private bool IsTimeLimitReached()
    {
        var result = DateTime.Now > WorkStartedAt.Add(TimeLimit);
        return result;
    }

    public void StopWork()
    {
        _cancellationTokenSource.Cancel();
        Status = WorkStatus.Stopped;
    }

    public void CheckIfValid()
    {
        if (WorkStartedAt != DateTime.MinValue || Status != WorkStatus.Ready)
        {
            throw new WorkException($"{nameof(WorkStartedAt)} is not set to {DateTime.MinValue} and is not in ready state. State is {Status}");
        }

        if (Id == Guid.Empty)
        {
            throw new WorkException($"{nameof(Id)} is not set");
        }
    }
}

public enum WorkPriorityEnum
{
    High,
    Medium,
    Low
}
