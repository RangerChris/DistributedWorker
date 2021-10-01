namespace DistributedWorker.Core.Domain;

public enum WorkerStatus
{
    Ready,
    Working,
    Stopped,
    Finished,
    Failed
}
