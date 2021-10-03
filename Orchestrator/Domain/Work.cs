using DistributedWorker.Core.Exception;

namespace DistributedWorker.Core.Domain;

/// <summary>
///     Represents a work task that have been issued by a <see cref="Worker" />
/// </summary>
public class Work : IWork
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private DateTime _timeLimit;

    public Work()
    {
        Status = WorkStatus.Ready;
        Name = "";
    }

    public Guid Id { get; set; }

    public WorkStatus Status
    {
        get;
        set;
    }

    /// <summary>
    ///     Definition of when the work should be done.
    ///     The work will fail if not done within the time limit
    /// </summary>
    public DateTime TimeLimit
    {
        get =>
            _timeLimit;
        set
        {
            if (value < DateTime.Now)
            {
                throw new WorkException("The time limit for the work must be in the future");
            }

            _timeLimit = value;
        }
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
                if (DateTime.Now > TimeLimit)
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

    public void StopWork()
    {
        _cancellationTokenSource.Cancel();
        Status = WorkStatus.Stopped;
    }
}
