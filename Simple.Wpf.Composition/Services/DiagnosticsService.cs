namespace Simple.Wpf.Composition.Services
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Reflection;
    using System.Windows;

    public sealed class DiagnosticsService : IDiagnosticsService, IDisposable
    {
        private readonly IDisposable _disposable;
        private readonly IScheduler _taskPoolScheduler;
        private readonly IScheduler _dispatcherScheduler;
        private readonly IConnectableObservable<Counters> _countersObservable;
        private readonly IConnectableObservable<Assembly> _assemblyObservable;

        public sealed class Counters : IDisposable
        {
            public PerformanceCounter PrivateWorkingSetMemoryCounter { get; private set; }
            public PerformanceCounter TotalAvailableMemoryCounter { get; private set; }
            public PerformanceCounter CpuCounter { get; private set; }

            public Counters(PerformanceCounter privateWorkingSetMemoryCounter,
                PerformanceCounter totalAvailableMemoryCounter,
                PerformanceCounter cpuCounter)
            {
                PrivateWorkingSetMemoryCounter = privateWorkingSetMemoryCounter;
                TotalAvailableMemoryCounter = totalAvailableMemoryCounter;
                CpuCounter = cpuCounter;
            }

            public void Dispose()
            {
                PrivateWorkingSetMemoryCounter.Dispose();
                TotalAvailableMemoryCounter.Dispose();
                CpuCounter.Dispose();
            }

            public Values NextValues()
            {
                return new Values(Convert.ToInt32(PrivateWorkingSetMemoryCounter.NextValue()),
                    Decimal.Round(Convert.ToDecimal(TotalAvailableMemoryCounter.NextValue()), 2),
                    Convert.ToInt32(CpuCounter.NextValue()));
            }
        }

        public struct Values
        {
            private readonly int _privateWorkingSetMemory;
            private readonly decimal _totalAvailableMemoryMb;
            private readonly int _cpu;

            public Values(int privateWorkingSetMemory,
                decimal totalAvailableMemoryMb,
                int cpu)
            {
                _privateWorkingSetMemory = privateWorkingSetMemory;
                _totalAvailableMemoryMb = totalAvailableMemoryMb;
                _cpu = cpu;
            }

            public int PrivateWorkingSetMemory { get { return _privateWorkingSetMemory; } }

            public decimal TotalAvailableMemoryMb {get { return _totalAvailableMemoryMb; } }

            public int Cpu { get { return _cpu; } }
        }

        public DiagnosticsService(IScheduler taskPoolScheduler = null, IScheduler dispatcherScheduler = null)
        {
            _taskPoolScheduler = taskPoolScheduler ?? TaskPoolScheduler.Default;
            _dispatcherScheduler = dispatcherScheduler ?? DispatcherScheduler.Current;

            _countersObservable = Observable.Create<Counters>(x => CreateCounters(x))
                .SubscribeOn(_taskPoolScheduler)
                .Replay(1);

            _assemblyObservable = AssemblyLoaded()
                .SubscribeOn(_taskPoolScheduler)
                .Merge(Observable.Return(AppDomain.CurrentDomain.GetAssemblies()).SelectMany(x => x))
                .Replay();

            _disposable = new CompositeDisposable
                          {
                              _countersObservable.Connect(),
                              _assemblyObservable.Connect()
                          };
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        public IObservable<Values> PerformanceCounters(int interval = 1000)
        {
            var timeSpan = TimeSpan.FromMilliseconds(interval);

            return _countersObservable.CombineLatest(BufferedDispatcherIdle(timeSpan), (x, y) => x)
                .Select(x => x.NextValues());
        }

        public IObservable<Assembly> LoadedAssembly { get { return _assemblyObservable; } }

        private static IDisposable CreateCounters(IObserver<Counters> observer)
        {
            var disposable = new CompositeDisposable();

            try
            {
                var processName = Process.GetCurrentProcess().ProcessName;

                var privateWorkingSetMemoryCounter = new PerformanceCounter("Process", "Working Set - Private", processName);
                var totalAvailableMemoryCounter = new PerformanceCounter("Memory", "Available MBytes");
                var cpuCounter = new PerformanceCounter("Process", "% Processor Time", processName);

                var counters = new Counters(privateWorkingSetMemoryCounter,
                    totalAvailableMemoryCounter,
                    cpuCounter);

                observer.OnNext(counters);
                disposable.Add(counters);
            }
            catch (Exception exn)
            {
                observer.OnError(exn);
            }

            return disposable;
        }

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

        private IObservable<Assembly> AssemblyLoaded()
        {
            return Observable.FromEventPattern<AssemblyLoadEventHandler, AssemblyLoadEventArgs>(
                h => AppDomain.CurrentDomain.AssemblyLoad += h,
                h => AppDomain.CurrentDomain.AssemblyLoad -= h)
                .Select(x => x.EventArgs.LoadedAssembly);
        }
    }
}