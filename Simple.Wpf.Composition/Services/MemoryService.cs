namespace Simple.Wpf.Composition.Services
{
    using System;
    using System.Diagnostics;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Windows;
    using NLog;

    public sealed class MemoryService : IMemoryService, IDisposable
    {
        private const int Kilo = 1024;
        private const int Mega = 1024 * 1000;
        private const int Giga = 1024 * 1000 * 1000;

        private readonly CompositeDisposable _disposable;
        private readonly IConnectableObservable<EventPattern<object>> _inactiveObservable;
        private readonly Logger _logger;
        private readonly PerformanceCounter _workingSetCounter;
        private IScheduler _scheduler;

        public MemoryService(IScheduler scheduler = null)
        {
            _scheduler = scheduler ?? TaskPoolScheduler.Default;

            _logger = LogManager.GetCurrentClassLogger();

            _workingSetCounter = new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName);

            _inactiveObservable = Observable.FromEventPattern(
               h => Application.Current.MainWindow.Dispatcher.Hooks.DispatcherInactive += h,
               h => Application.Current.MainWindow.Dispatcher.Hooks.DispatcherInactive -= h)
               .ObserveOn(_scheduler)
               .Replay(1);

            _disposable = new CompositeDisposable
                          {
                              _inactiveObservable.Connect(),
                              _workingSetCounter,
                          };
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        public IObservable<decimal> MemoryInBytes { get { return Create(1); } }

        public IObservable<decimal> MemoryInKiloBytes { get { return Create(Kilo); } }

        public IObservable<decimal> MemoryInMegaBytes { get { return Create(Mega); } } 

        public IObservable<decimal> MemoryInGigaBytes { get { return Create(Giga); } } 

        private IObservable<decimal> Create(int divisor)
        {
            return Observable.Return(Decimal.Round((decimal) WorkingSetPrivate()/divisor, 2), _scheduler)
                .Merge(_inactiveObservable.Select(x => Decimal.Round((decimal) WorkingSetPrivate()/divisor, 2)))
                .DistinctUntilChanged();
        }

        private float WorkingSetPrivate()
        {
            try
            {
                return _workingSetCounter.NextValue();
            }
            catch (Exception exn)
            {
                _logger.Warn("Failed to calculate working set (private), exception message - '{0}'", exn.Message);
                
                return 0;
            }
        }
    }
}