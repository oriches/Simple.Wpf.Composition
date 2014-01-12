namespace Simple.Wpf.Composition.Workspaces.Logging
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Infrastructure;

    public sealed class LoggingViewModel : BaseViewModel
    {
        private readonly ObservableCollection<string> _entries;

        public LoggingViewModel()
        {
            _entries = new ObservableCollection<string>();
        }

        public string Prompt { get { return "Log> "; } }

        public IEnumerable<string> Entries { get { return _entries; } }

        public void AddEntry(string entry)
        {
            _entries.Add(Prompt + entry);
        }
    }
}