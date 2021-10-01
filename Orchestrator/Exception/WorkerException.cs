namespace DistributedWorker.Core.Exception;

public class WorkerException : System.Exception
{
    public WorkerException(string message) : base(message)
    {
    }
}
