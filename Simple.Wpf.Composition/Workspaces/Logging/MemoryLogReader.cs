﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NLog;
using Simple.Wpf.Composition.Infrastructure;

namespace Simple.Wpf.Composition.Workspaces.Logging
{
    public sealed class MemoryLogReader : ILogReader, IDisposable
    {
        private readonly IConnectableObservable<IEnumerable<string>> _connectObservable;
        private readonly IDisposable _disposable;
        private readonly string _logName;
        private readonly LimitedMemoryTarget _target;

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
                var entries = new string[0];
                return _connectObservable.SelectMany(x =>
                {
                    var newEntries = x.Except(entries);
                    entries = x.ToArray();

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
            return LogManager.Configuration.AllTargets.Where(target => target.Name == _logName)
                .Cast<LimitedMemoryTarget>()
                .FirstOrDefault();
        }
    }
}