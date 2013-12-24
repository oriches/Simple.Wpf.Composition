using System;
using System.Collections.Generic;
using System.Linq;

namespace Simple.Wpf.Composition.Workspaces.Logging
{
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using NLog.Targets;

    public sealed class LogReader : ILogReader
    {
        private readonly IScheduler _scheduler;
        private readonly IConnectableObservable<IEnumerable<string>> _connectObservable;

        public LogReader(IScheduler scheduler = null)
        {
            _scheduler = scheduler ?? TaskPoolScheduler.Default;

            _connectObservable = Observable.Interval(TimeSpan.FromSeconds(1), _scheduler)
                .Select(x => GetEntriesImpl())
                .Replay();

              _connectObservable.Connect();
        }

        public IObservable<IEnumerable<string>> GetInMemoryEntries()
        {
            var entries = new List<string>();
            return _connectObservable.Select(x =>
                   {
                       var newEntries = x.Except(entries).ToList();
                       entries.AddRange(newEntries);

                       return newEntries;
                   });
        }

        private static IEnumerable<string> GetEntriesImpl()
        {
             var memoryTarget = NLog.LogManager.Configuration.AllTargets.Where(target => target is MemoryTarget)
                .Cast<MemoryTarget>()
                .FirstOrDefault();

            return memoryTarget != null ? memoryTarget.Logs.Select(x => x + Environment.NewLine) : Enumerable.Empty<string>();

        }
    }
}
