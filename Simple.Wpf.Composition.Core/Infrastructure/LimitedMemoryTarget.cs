using System.Collections.Concurrent;
using System.Collections.Generic;
using NLog;
using NLog.Targets;

namespace Simple.Wpf.Composition.Core.Infrastructure
{
    [Target("LimitedMemory")]
    public sealed class LimitedMemoryTarget : TargetWithLayout
    {
        private readonly ConcurrentQueue<string> _entries = new ConcurrentQueue<string>();
        private readonly object _sync = new object();

        public LimitedMemoryTarget()
        {
            Limit = 1000;
        }

        public int Limit { get; set; }

        public IEnumerable<string> Logs => _entries;

        protected override void Write(LogEventInfo logEvent)
        {
            var msg = Layout.Render(logEvent);

            _entries.Enqueue(msg);
            if (_entries.Count > Limit)
            {
                string output;
                _entries.TryDequeue(out output);
            }
        }
    }
}