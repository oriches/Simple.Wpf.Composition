namespace Simple.Wpf.Composition.Workspaces.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Infrastructure;

    public sealed class MemoryLogReader : ILogReader, IDisposable
    {
        private readonly string _logName;
        private readonly IConnectableObservable<IEnumerable<string>> _connectObservable;
        private readonly LimitedMemoryTarget _target;
        private readonly IDisposable _disposable;

        public MemoryLogReader(string logName, IScheduler scheduler = null)
        {
            _logName = logName;
            scheduler = scheduler ?? TaskPoolScheduler.Default;

            _target = FindTarget();

            _connectObservable = Observable.Interval(TimeSpan.FromSeconds(1), scheduler)
                .Select(x => ReadTargetEntries())
                .Publish();

              _disposable = _connectObservable.Connect();
        }

        public void Dispose()
        {
            _disposable.Dispose();
            _target.Dispose();
        }

        public IObservable<string> Entries
        {
            get
            {
                var entries = Enumerable.Empty<string>();
                return _connectObservable.SelectMany(x =>
                {
                    var materalisedX = x.ToArray();
                    var newEntries = materalisedX.Except(entries);
                    entries = materalisedX;

                    return newEntries;
                });
            }
        }

        private IEnumerable<string> ReadTargetEntries()
        {
            return _target != null ? _target.Logs.Select(x => x + Environment.NewLine) : Enumerable.Empty<string>();
        }

        private LimitedMemoryTarget FindTarget()
        {
            return NLog.LogManager.Configuration.AllTargets.Where(target => target.Name == _logName)
               .Cast<LimitedMemoryTarget>()
               .FirstOrDefault();
        }
    }
}
