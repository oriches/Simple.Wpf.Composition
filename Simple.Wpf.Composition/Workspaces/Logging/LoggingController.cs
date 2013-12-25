namespace Simple.Wpf.Composition.Workspaces.Logging
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    public sealed class LoggingController : BaseController<LoggingViewModel>
    {
        private readonly IDisposable _disposable;

        public LoggingController(LoggingViewModel viewModel, ILogReader reader, IScheduler scheduler = null)
            : base(viewModel)
        {
            scheduler = scheduler ?? DispatcherScheduler.Current;

            _disposable = reader.Entries
                .ObserveOn(scheduler)
                .Subscribe(x => ViewModel.AddEntry(x));
        }

        public override void Dispose()
        {
            base.Dispose();

            _disposable.Dispose();
        }
    }
}