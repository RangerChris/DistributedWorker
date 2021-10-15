namespace DistributedWorker.Core.Domain;

public enum WorkStatus
{
    Ready,
    NotReady,
    Working,
    Stopped,
    Finished,
    Failed
}
