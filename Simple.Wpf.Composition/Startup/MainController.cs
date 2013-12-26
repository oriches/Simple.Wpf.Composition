namespace Simple.Wpf.Composition.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using NLog;
    using Services;
    using Workspaces;

    public sealed class MainController : BaseController<MainViewModel>
    {
        private readonly IEnumerable<IWorkspaceDescriptor> _workspaceDescriptors;
        private readonly Logger _logger;
        private readonly CompositeDisposable _disposable;

        public MainController(MainViewModel viewModel, IMemoryService memoryService,
            IEnumerable<IWorkspaceDescriptor> workspaceDescriptors)
            : base(viewModel)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _logger.Debug("Main controller starting...");

            _workspaceDescriptors = workspaceDescriptors;

            var availableWorkspaces = _workspaceDescriptors.OrderBy(x => x.Position)
                .Select(x => x.Name)
                .ToList();

            foreach (var availableWorkspace in availableWorkspaces)
            {
                _logger.Debug("Available workspace - '{0}'", availableWorkspace);
            }

            availableWorkspaces.Insert(0, string.Empty);

            ViewModel.AddAvailableWorkspaces(availableWorkspaces);

            _disposable = new CompositeDisposable
            {
                ViewModel.AddWorkspaceStream
                    .Subscribe(CreateWorkspace),
                ViewModel.RemoveWorkspaceStream
                    .Subscribe(DeleteWorkspace),
                memoryService.MemoryInMegaBytes
                    .DistinctUntilChanged()
                    .Throttle(TimeSpan.FromSeconds(1))
                    .Subscribe(UpdateUsedMemory)
            };
        }

        public override void Dispose()
        {
            base.Dispose();

            _disposable.Dispose();
        }

        private void CreateWorkspace(string requestedWorkspace)
        {
            _logger.Debug("Creating workspace, name - '{0}'", requestedWorkspace);

            var newWorkspace = _workspaceDescriptors.Single(x => x.Name == requestedWorkspace).CreateWorkspace();
            var @group = ViewModel.Workspaces.GroupBy(x => x.Type.FullName).FirstOrDefault(x => x.Key == newWorkspace.Type.FullName);
            var title = @group == null ? requestedWorkspace : string.Format("{0} ({1})", requestedWorkspace, @group.Count() + 1);

            newWorkspace.Title = title;

            _logger.Debug("Workspace title - '{0}'", title);
            
            ViewModel.AddWorkspace(newWorkspace);
            _logger.Debug("Workspace count = {0}", ViewModel.Workspaces.Count);
        }

        private void DeleteWorkspace(Workspace workspace)
        {
            _logger.Debug("Deleting workspace, title - '{0}'", workspace.Title);

            ViewModel.RemoveWorkspace(workspace);

            workspace.Dispose();

            _logger.Debug("Workspace count = {0}", ViewModel.Workspaces.Count);
        }

        private void UpdateUsedMemory(decimal usedMemory)
        {
            ViewModel.UpdateMemoryUsed(usedMemory);

            _logger.Debug("Used memory = {0}", ViewModel.MemoryUsed);
        }
    }
}