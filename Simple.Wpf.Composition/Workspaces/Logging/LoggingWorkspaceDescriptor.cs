using System;
using Autofac;

namespace Simple.Wpf.Composition.Workspaces.Logging
{
    public sealed class LoggingWorkspaceDescriptor : IWorkspaceDescriptor
    {
        private readonly Uri _resources = new Uri("/simple.wpf.composition;component/workspaces/logging/resources.xaml",
            UriKind.RelativeOrAbsolute);

        private readonly WorkspaceFactory _workspaceFactory;

        public LoggingWorkspaceDescriptor(WorkspaceFactory workspaceFactory)
        {
            _workspaceFactory = workspaceFactory;
        }

        public int Position => 0;

        public string Name => "Logging Workspace";

        public Workspace CreateWorkspace()
        {
            return _workspaceFactory.Create<Registrar, LoggingController>(_resources);
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class Registrar
        {
            // Scoped registrations, these will be thrown away when the workspace is disposed...
            public Registrar(ContainerBuilder container)
            {
                container.RegisterType<MemoryLogReader>().As<ILogReader>()
                    .WithParameter(new NamedParameter("logName", "memory"));
                container.RegisterType<LoggingViewModel>();
                container.RegisterType<LoggingController>();
            }
        }
    }
}