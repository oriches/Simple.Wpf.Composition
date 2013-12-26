namespace Simple.Wpf.Composition.Services
{
    using System;
    using System.Diagnostics;
    using System.Reactive;
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

        public MemoryService()
        {
            _logger = LogManager.GetCurrentClassLogger();

            _inactiveObservable = Observable.FromEventPattern(
                h => Application.Current.MainWindow.Dispatcher.Hooks.DispatcherInactive += h,
                h => Application.Current.MainWindow.Dispatcher.Hooks.DispatcherInactive -= h)
                .Replay(1);

            _workingSetCounter = new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName);
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

        public IObservable<long> MemoryInBytes
        {
            get
            {
                return _inactiveObservable.Select(x => (long)WorkingSetPrivate()).DistinctUntilChanged();
            }
        }

        public IObservable<decimal> MemoryInKiloBytes
        {
            get
            {
                return _inactiveObservable.Select(x => Decimal.Round((decimal)WorkingSetPrivate() / Kilo, 2));
            }
        }

        public IObservable<decimal> MemoryInMegaBytes
        {
            get
            {
                return _inactiveObservable.Select(x => Decimal.Round((decimal)WorkingSetPrivate() / Mega, 2));
            }
        }

        public IObservable<decimal> MemoryInGigaBytes
        {
            get
            {
                return _inactiveObservable.Select(x => Decimal.Round((decimal)WorkingSetPrivate() / Giga, 2));
            }
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