using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using System.Windows.Input;
using Simple.Wpf.Composition.Commands;
using Simple.Wpf.Composition.Infrastructure;
using Simple.Wpf.Composition.Workspaces;

namespace Simple.Wpf.Composition.Startup
{
    public sealed class MainViewModel : BaseViewModel, IDisposable
    {
        private readonly Subject<string> _addWorkspaceStream;
        private readonly ObservableCollection<string> _availableWorkspaces;
        private readonly Subject<Workspace> _removeWorkspaceStream;
        private readonly ObservableCollection<Workspace> _workspaces;

        private ICommand _addCommand;

        private ReadOnlyObservableCollection<string> _availableWorkspacesReadOnly;
        private string _memoryUsed;
        private ICommand _removeCommand;
        private Workspace _selectedWorkspace;
        private ReadOnlyObservableCollection<Workspace> _workspacesReadOnly;

        public MainViewModel()
        {
            _availableWorkspaces = new ObservableCollection<string>();
            _workspaces = new ObservableCollection<Workspace>();

            _addWorkspaceStream = new Subject<string>();
            _removeWorkspaceStream = new Subject<Workspace>();

            MemoryUsed = "0.00 Mb";
        }

        public ReadOnlyObservableCollection<string> AvailableWorkspaces =>
            _availableWorkspacesReadOnly ??
            (_availableWorkspacesReadOnly = new ReadOnlyObservableCollection<string>(_availableWorkspaces));

        public ReadOnlyObservableCollection<Workspace> Workspaces =>
            _workspacesReadOnly ??
            (_workspacesReadOnly = new ReadOnlyObservableCollection<Workspace>(_workspaces));

        public ICommand AddWorkspaceCommand
        {
            get
            {
                return _addCommand ??
                       (_addCommand =
                           new RelayCommand<string>(x => _addWorkspaceStream.OnNext(x),
                               x => !string.IsNullOrEmpty(x)));
            }
        }

        public ICommand RemoveWorkspaceCommand
        {
            get
            {
                return _removeCommand ??
                       (_removeCommand =
                           new RelayCommand(() => _removeWorkspaceStream.OnNext(SelectedWorkspace),
                               () => SelectedWorkspace != null));
            }
        }

        public Workspace SelectedWorkspace
        {
            get => _selectedWorkspace;

            set => SetPropertyAndNotify(ref _selectedWorkspace, value);
        }

        public string MemoryUsed
        {
            get => _memoryUsed;

            private set => SetPropertyAndNotify(ref _memoryUsed, value);
        }

        public IObservable<string> AddWorkspaceStream => _addWorkspaceStream;

        public IObservable<Workspace> RemoveWorkspaceStream => _removeWorkspaceStream;

        public void Dispose()
        {
            _addWorkspaceStream.Dispose();
            _removeWorkspaceStream.Dispose();
        }

        public void AddAvailableWorkspaces(IEnumerable<string> workspaces)
        {
            using (SuspendNotifications())
            {
                foreach (var workspace in workspaces) _availableWorkspaces.Add(workspace);
            }
        }

        public void AddWorkspace(Workspace workspace)
        {
            using (SuspendNotifications())
            {
                _workspaces.Add(workspace);
                SelectedWorkspace = workspace;
            }
        }

        public void RemoveWorkspace(Workspace workspace)
        {
            using (SuspendNotifications())
            {
                if (_selectedWorkspace == workspace) SelectedWorkspace = null;

                _workspaces.Remove(workspace);
            }
        }

        public void UpdateMemoryUsed(decimal memoryUsed)
        {
            MemoryUsed = string.Format("{0} Mb", memoryUsed.ToString("N"));
        }
    }
}