using System.Threading.Tasks;

namespace DistributedWorker.Core.Tests
{
    public static class ExtensionHelper
    {
        public static async void FireAndForget(this Task task)
        {
            await task;
        }
    }
}
