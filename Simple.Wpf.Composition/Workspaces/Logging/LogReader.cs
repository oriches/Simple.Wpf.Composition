namespace Simple.Wpf.Composition.Workspaces.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using NLog.Targets;

    public sealed class LogReader : ILogReader
    {
        private readonly string _logName;
        private readonly IConnectableObservable<IEnumerable<string>> _connectObservable;

        public LogReader(string logName, IScheduler scheduler = null)
        {
            _logName = logName;
            scheduler = scheduler ?? TaskPoolScheduler.Default;

            _connectObservable = Observable.Interval(TimeSpan.FromSeconds(1), scheduler)
                .Select(x => GetEntriesImpl())
                .Replay();

              _connectObservable.Connect();
        }

        public IObservable<string> Entries
        {
            get
            {
                var entries = new List<string>();
                return _connectObservable.SelectMany(x =>
                                                 {
                                                     var newEntries = x.Except(entries).ToList();
                                                     entries.AddRange(newEntries);

                                                     return newEntries;
                                                 });
            }
        }

        private IEnumerable<string> GetEntriesImpl()
        {
            var memoryTarget = NLog.LogManager.Configuration.AllTargets.Where(target => target.Name == _logName)
                .Cast<MemoryTarget>()
                .FirstOrDefault();

            return memoryTarget != null ? memoryTarget.Logs.Select(x => x + Environment.NewLine) : Enumerable.Empty<string>();

        }
    }
}
