namespace Simple.Wpf.Composition.Infrastructure
{
    using System.Collections.Generic;
    using NLog;
    using NLog.Targets;

    [Target("LimitedMemory")]
    public sealed class LimitedMemoryTarget : TargetWithLayout
    {
        private readonly Queue<string> _entries = new Queue<string>();

        public LimitedMemoryTarget()
        {
            Limit = 1000;
        }
        
        public int Limit { get; set; }

        public IEnumerable<string> Logs
        {
            get { return _entries; }
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var msg = Layout.Render(logEvent);

            _entries.Enqueue(msg);
            if (_entries.Count > Limit)
            {
                _entries.Dequeue();
            }
        }
    }
}