namespace Simple.Wpf.Composition.Workspaces.Logging
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    public sealed class LoggingController : BaseController<LoggingViewModel>
    {
        private readonly IDisposable _disposable;

        public LoggingController(LoggingViewModel viewModel, ILogReader reader, IScheduler scheduler = null) : base(viewModel)
        {
            IScheduler scheduler1 = scheduler ?? DispatcherScheduler.Current;

            _disposable = reader.GetInMemoryEntries()
                .ObserveOn(scheduler1)
                .Subscribe(viewModel.AddEntries);
        }

        public override void Dispose()
        {
            base.Dispose();

            _disposable.Dispose();
        }
    }
}