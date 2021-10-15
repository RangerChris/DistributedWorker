using DistributedWorker.Core.Domain;
using DistributedWorker.Core.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WorkOrchestratorSite.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly Orchestrator _orchestrator;

    public IndexModel(ILogger<IndexModel> logger, Orchestrator orchestrator)
    {
        _logger = logger;
        _orchestrator = orchestrator;
    }

    [BindProperty]
    public IndexViewModel ViewModel
    {
        get;
        set;
    }

    public void OnGet()
    {
        ViewModel = new IndexViewModel
        {
            NumberOfWorkers = 1,
            NumberOfWork = 0,
            Orchestrator = _orchestrator
        };
    }

    public void OnPost()
    {
        if (ViewModel.NumberOfWorkers > 0)
        {
            CreateWorkers();
        }

        if (ViewModel.NumberOfWork > 0)
        {
            CreateWork();
        }
    }

    private void CreateWork()
    {
        try
        {
            var workList = new WorkBuilder().CreateWork(ViewModel.NumberOfWork, ViewModel.CanFail)
                                            .Build();
            _orchestrator.AddWork(workList);
        }
        catch (ArgumentException)
        {
            ModelState.AddModelError("validation", "You added a invalid number of work");
        }
    }

    private void CreateWorkers()
    {
        ViewModel.Orchestrator = _orchestrator;
        try
        {
            _orchestrator.AddWorker(ViewModel.NumberOfWorkers);
        }
        catch (ArgumentException)
        {
            ModelState.AddModelError("validation", "You added a invalid number of workers");
        }
    }
}
