using DistributedWorker.Core.Exception;

namespace DistributedWorker.Core.Domain;

/// <summary>
///     Represents a work task that have been issued by a <see cref="Worker" />
/// </summary>
public class Work
{
    private DateTime _timeLimit;

    public Guid Id { get; set; }

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

    public void DoWork()
    {
        WorkStartedAt = DateTime.Now;
    }
}
