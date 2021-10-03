namespace DistributedWorker.Core.Domain;

public enum WorkStatus
{
    Ready,
    Working,
    Stopped,
    Finished,
    Failed
}
