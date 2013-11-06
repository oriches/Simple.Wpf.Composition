namespace Simple.Wpf.Composition.Workspaces.Dilbert
{
    using System.Reactive.Threading.Tasks;
    using System.Threading.Tasks;
    using global::Dilbert;

    public sealed class DilbertController : BaseController<DilbertViewModel>
    {
        private readonly IDailyDilbertService _dailyDilbertService;

        public DilbertController(DilbertViewModel viewModel, IDailyDilbertService dailyDilbertService) : base(viewModel)
        {
            _dailyDilbertService = dailyDilbertService;

            _dailyDilbertService.DailyAsFileAsync()
                .ContinueWith(t => ViewModel.FilePath = t.Result, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public override DilbertViewModel ViewModel
        {
            get
            {
                { return base.ViewModel; }
            }
        }
    }
}