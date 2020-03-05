using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using NLog;
// ReSharper disable ConvertClosureToMethodGroup

namespace Simple.Wpf.Composition.Core.Services
{
    public sealed class MemoryService : IMemoryService, IDisposable
    {
        private const int Kilo = 1024;
        private const int Mega = 1024 * 1000;
        private const int Giga = 1024 * 1000 * 1000;
        private readonly IConnectableObservable<Counters> _countersObservable;
        private readonly IScheduler _dispatcherScheduler;

        private readonly IDisposable _disposable;

        private readonly Logger _logger;
        private readonly IScheduler _taskPoolScheduler;

        public MemoryService(IScheduler taskPoolScheduler = null, IScheduler dispatcherScheduler = null)
        {
            _taskPoolScheduler = taskPoolScheduler ?? TaskPoolScheduler.Default;
            _dispatcherScheduler = dispatcherScheduler ?? DispatcherScheduler.Current;

            _logger = LogManager.GetCurrentClassLogger();

            _countersObservable = Observable.Create<Counters>(x => CreateCounters(x))
                .SubscribeOn(_taskPoolScheduler)
                .CombineLatest(BufferedDispatcherIdle(TimeSpan.FromSeconds(1)), (x, y) => x)
                .Replay(1);

            _disposable = _countersObservable.Connect();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        public IObservable<decimal> MemoryInBytes => Create(1);

        public IObservable<decimal> MemoryInKiloBytes => Create(Kilo);

        public IObservable<decimal> MemoryInMegaBytes => Create(Mega);

        public IObservable<decimal> MemoryInGigaBytes => Create(Giga);

        private IObservable<Unit> BufferedDispatcherIdle(TimeSpan timeSpan)
        {
            var mainWindow = Application.Current.MainWindow;

            return Observable.FromEventPattern(
                    h => mainWindow.Dispatcher.Hooks.DispatcherInactive += h,
                    h => mainWindow.Dispatcher.Hooks.DispatcherInactive -= h, _dispatcherScheduler)
                .Buffer(timeSpan, _taskPoolScheduler)
                .Where(x => x.Any())
                .Select(x => Unit.Default);
        }

        private IDisposable CreateCounters(IObserver<Counters> observer)
        {
            var disposable = new CompositeDisposable();

            try
            {
                var processName = Process.GetCurrentProcess().ProcessName;

                var memoryCounter = new PerformanceCounter("Process", "Working Set - Private", processName);
                var cpuCounter = new PerformanceCounter("Process", "% Processor Time", processName);

                var counters = new Counters(memoryCounter, cpuCounter);

                observer.OnNext(counters);
                disposable.Add(counters);
            }
            catch (Exception exn)
            {
                observer.OnError(exn);
            }

            return disposable;
        }

        private IObservable<decimal> Create(int divisor)
        {
            return _countersObservable.Select(x => decimal.Round(WorkingSetPrivate(x.MemoryCounter) / divisor, 2))
                .DistinctUntilChanged();
        }

        private decimal WorkingSetPrivate(PerformanceCounter memoryCounter)
        {
            try
            {
                return Convert.ToDecimal(memoryCounter.NextValue());
            }
            catch (Exception exn)
            {
                _logger.Warn("Failed to calculate working set (private), exception message - '{0}'", exn.Message);

                return 0;
            }
        }

        internal sealed class Counters : IDisposable
        {
            public Counters(PerformanceCounter memoryCounter, PerformanceCounter cpuCounter)
            {
                MemoryCounter = memoryCounter;
                CpuCounter = cpuCounter;
            }

            public PerformanceCounter MemoryCounter { get; }

            public PerformanceCounter CpuCounter { get; }

            public void Dispose()
            {
                MemoryCounter.Dispose();
                CpuCounter.Dispose();
            }
        }
    }
}