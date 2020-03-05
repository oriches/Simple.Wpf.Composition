using System.Threading.Tasks;
using Dilbert;
using Dilbert.Common;
using Simple.Wpf.Composition.Infrastructure;

namespace Simple.Wpf.Composition.Workspaces.Dilbert
{
    public sealed class DilbertController : BaseController<DilbertViewModel>
    {
        public DilbertController(DilbertViewModel viewModel, IDailyDilbertService dailyDilbertService) : base(viewModel)
        {
            dailyDilbertService.DailyAsFileAsync()
                .ContinueWith(t => { ViewModel.FilePath = t.IsFaulted ? null : t.Result; },
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        public override DilbertViewModel ViewModel => base.ViewModel;
    }
}