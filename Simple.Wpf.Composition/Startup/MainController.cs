﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using NLog;
using Simple.Wpf.Composition.Infrastructure;
using Simple.Wpf.Composition.Services;
using Simple.Wpf.Composition.Workspaces;

namespace Simple.Wpf.Composition.Startup
{
    public sealed class MainController : BaseController<MainViewModel>
    {
        private const int DisposeDelay = 333;
        private readonly CompositeDisposable _disposable;
        private readonly Logger _logger;

        private readonly IEnumerable<IWorkspaceDescriptor> _workspaceDescriptors;

        public MainController(MainViewModel viewModel, IMemoryService memoryService,
            IDiagnosticsService diagnosticsService,
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
                _logger.Debug("Available workspace - '{0}'", availableWorkspace);

            availableWorkspaces.Insert(0, string.Empty);

            ViewModel.AddAvailableWorkspaces(availableWorkspaces);

            _disposable = new CompositeDisposable
            {
                ViewModel.AddWorkspaceStream
                    .Subscribe(CreateWorkspace),

                ViewModel.RemoveWorkspaceStream
                    .Do(RemoveWorkspace)
                    .Delay(TimeSpan.FromMilliseconds(DisposeDelay))
                    .Do(DeleteWorkspace)
                    .Subscribe(),

                memoryService.MemoryInMegaBytes
                    .DistinctUntilChanged()
                    .Throttle(TimeSpan.FromSeconds(1))
                    .Subscribe(UpdateUsedMemory),

                diagnosticsService.PerformanceCounters(1000)
                    .Subscribe(x =>
                    {
                        Debug.WriteLine("PrivateWorkingSetMemory = " +
                                        decimal.Round(Convert.ToDecimal(x.PrivateWorkingSetMemory) / (1024 * 1000), 2));
                        Debug.WriteLine("TotalAvailableMemoryMb = " + x.TotalAvailableMemoryMb);
                    }),

                diagnosticsService.LoadedAssembly
                    .Subscribe(x => Debug.WriteLine("Assembly = " + x.FullName))
            };

            for (var i = 0; i < 200; i++) _logger.Debug("Log Item = " + i);
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
            var group = ViewModel.Workspaces.GroupBy(x => x.Type.FullName)
                .FirstOrDefault(x => x.Key == newWorkspace.Type.FullName);
            var title = group == null
                ? requestedWorkspace
                : string.Format("{0} ({1})", requestedWorkspace, group.Count() + 1);

            newWorkspace.Title = title;

            _logger.Debug("Workspace title - '{0}'", title);

            ViewModel.AddWorkspace(newWorkspace);
            _logger.Debug("Workspace count = {0}", ViewModel.Workspaces.Count);
        }

        private void RemoveWorkspace(Workspace workspace)
        {
            _logger.Debug("Removing workspace, title - '{0}'", workspace.Title);

            ViewModel.RemoveWorkspace(workspace);

            _logger.Debug("Workspace count = {0}", ViewModel.Workspaces.Count);
        }

        private void DeleteWorkspace(Workspace workspace)
        {
            _logger.Debug("Deleting workspace, title - '{0}'", workspace.Title);

            workspace.Dispose();
        }

        private void UpdateUsedMemory(decimal usedMemory)
        {
            ViewModel.UpdateMemoryUsed(usedMemory);

            _logger.Debug("Used memory = {0}", ViewModel.MemoryUsed);
        }
    }
}