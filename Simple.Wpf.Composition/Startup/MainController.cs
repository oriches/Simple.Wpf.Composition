namespace Simple.Wpf.Composition.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NLog;
    using Workspaces;

    public sealed class MainController : BaseController<MainViewModel>
    {
        private readonly IEnumerable<IWorkspaceDescriptor> _workspaceDescriptors;
        private readonly IDisposable _addDisposable;
        private IDisposable _removeDisposable;
        private Logger _logger;

        public MainController(MainViewModel viewModel, IEnumerable<IWorkspaceDescriptor> workspaceDescriptors)
            : base(viewModel)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _logger.Debug("Main controller starting...");

            _workspaceDescriptors = workspaceDescriptors;
        
            var availableWorkspaces = _workspaceDescriptors.OrderBy(x => x.Position)
                .Select(x => x.Name)
                .ToList();

            availableWorkspaces.Insert(0, string.Empty);

            ViewModel.AddAvailableWorkspaces(availableWorkspaces);

            _addDisposable = viewModel.AddWorkspaceStream
                .Subscribe(CreateWorkspace);

            _removeDisposable = viewModel.RemoveWorkspaceStream
                .Subscribe(DeleteWorkspace);
        }
        
        public override void Dispose()
        {
            base.Dispose();

            _addDisposable.Dispose();
        }

        private void CreateWorkspace(string requestedWorkspace)
        {
            _logger.Debug("Creating workspace, name - '{0}'", requestedWorkspace);

            var newWorkspace = _workspaceDescriptors.Single(x => x.Name == requestedWorkspace).CreateWorkspace();
            var @group = ViewModel.Workspaces.GroupBy(x => x.Type.FullName).FirstOrDefault(x => x.Key == newWorkspace.Type.FullName);
            var title = @group == null ? requestedWorkspace : string.Format("{0} ({1})", requestedWorkspace, @group.Count() + 1);

            newWorkspace.Title = title;
            
            ViewModel.AddWorkspace(newWorkspace);
        }

        private void DeleteWorkspace(Workspace workspace)
        {
            ViewModel.RemoveWorkspace(workspace);

            workspace.Dispose();
        }
    }
}