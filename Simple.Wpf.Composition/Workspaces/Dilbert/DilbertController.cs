namespace Simple.Wpf.Composition.Workspaces.Dilbert
{
    using System.Threading.Tasks;
    using global::Dilbert;

    public sealed class DilbertController : BaseController<DilbertViewModel>
    {
        public DilbertController(DilbertViewModel viewModel, IDailyDilbertService dailyDilbertService) : base(viewModel)
        {
            dailyDilbertService.DailyAsFileAsync()
                .ContinueWith(t =>
                              {
                                  ViewModel.FilePath = t.IsFaulted ? null : t.Result;
                              }, TaskScheduler.FromCurrentSynchronizationContext());
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