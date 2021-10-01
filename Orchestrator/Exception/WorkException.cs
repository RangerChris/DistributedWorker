namespace DistributedWorker.Core.Exception;

public class WorkException : System.Exception
{
    public WorkException(string message) : base(message)
    {
    }
}
