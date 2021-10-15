using System.ComponentModel;
using DistributedWorker.Core.Domain;

namespace WorkOrchestratorSite.Pages
{
    public class IndexViewModel
    {
        [DisplayName("Number of workers")] public int NumberOfWorkers { get; set; }

        [DisplayName("Number of work")] public int NumberOfWork { get; set; }

        public bool CanFail { get; set; }

        public Orchestrator Orchestrator { get; set; }
    }
}
