using System.Collections.Generic;
using System.Collections.ObjectModel;
using Simple.Wpf.Composition.Infrastructure;

namespace Simple.Wpf.Composition.Workspaces.Logging
{
    public sealed class LoggingViewModel : BaseViewModel
    {
        private readonly ObservableCollection<string> _entries;

        public LoggingViewModel()
        {
            _entries = new ObservableCollection<string>();
        }

        public string Prompt => "Log> ";

        public IEnumerable<string> Entries => _entries;

        public void AddEntry(string entry)
        {
            _entries.Add(Prompt + entry);
        }
    }
}