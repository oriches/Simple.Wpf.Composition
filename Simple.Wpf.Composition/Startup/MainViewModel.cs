namespace Simple.Wpf.Composition.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive.Subjects;
    using System.Windows.Input;
    using Commands;
    using Workspaces;

    public sealed class MainViewModel : BaseViewModel, IDisposable
    {
        private readonly ObservableCollection<string> _availableWorkspaces;
        private readonly ObservableCollection<Workspace> _workspaces;
        private readonly Subject<string> _addWorkspaceStream;
        private readonly Subject<Workspace> _removeWorkspaceStream;

        private ReadOnlyObservableCollection<string> _availableWorkspacesReadOnly;
        private ReadOnlyObservableCollection<Workspace> _workspacesReadOnly;

        private ICommand _addCommand;
        private ICommand _removeCommand;
        private Workspace _selectedWorkspace;
        private string _memoryUsed;

        public MainViewModel()
        {
            _availableWorkspaces = new ObservableCollection<string>();
            _workspaces = new ObservableCollection<Workspace>();

            _addWorkspaceStream = new Subject<string>();
            _removeWorkspaceStream = new Subject<Workspace>();

            MemoryUsed = "0.00 Mb";
        }

        public ReadOnlyObservableCollection<string> AvailableWorkspaces
        {
            get
            {
                return _availableWorkspacesReadOnly ??
                       (_availableWorkspacesReadOnly = new ReadOnlyObservableCollection<string>(_availableWorkspaces));
            }
        }

        public ReadOnlyObservableCollection<Workspace> Workspaces
        {
            get
            {
                return _workspacesReadOnly ??
                       (_workspacesReadOnly = new ReadOnlyObservableCollection<Workspace>(_workspaces));
            }
        }

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
            get
            {
                return _selectedWorkspace;
            }

            set
            {
                SetPropertyAndNotify(ref _selectedWorkspace, value);
            }
        }

        public string MemoryUsed
        {
            get
            {
                return _memoryUsed;
            }

            private set
            {
                SetPropertyAndNotify(ref _memoryUsed, value);
            }
        }

        public IObservable<string> AddWorkspaceStream { get { return _addWorkspaceStream; } }

        public IObservable<Workspace> RemoveWorkspaceStream { get { return _removeWorkspaceStream; } }

        public void AddAvailableWorkspaces(IEnumerable<string> workspaces)
        {
            using (SuspendNotifications())
            {
                foreach (var workspace in workspaces)
                {
                    _availableWorkspaces.Add(workspace);
                }
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
                if (_selectedWorkspace == workspace)
                {
                    SelectedWorkspace = null;
                }

                _workspaces.Remove(workspace);
            }
        }

        public void UpdateMemoryUsed(decimal memoryUsed)
        {
            MemoryUsed = string.Format("{0} Mb", memoryUsed.ToString("N"));
        }

        public void Dispose()
        {
            _addWorkspaceStream.Dispose();
            _removeWorkspaceStream.Dispose();
        }
    }
}