namespace DistributedWorker.Core.Domain;

public interface IWork
{
    public Task DoWork(CancellationToken cancellationToken);
}
