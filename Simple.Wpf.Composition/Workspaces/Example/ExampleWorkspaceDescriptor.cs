using System;
using Autofac;

namespace Simple.Wpf.Composition.Workspaces.Example
{
    public sealed class ExampleWorkspaceDescriptor : IWorkspaceDescriptor
    {
        private readonly Uri _resources = new Uri("/simple.wpf.composition;component/workspaces/example/resources.xaml",
            UriKind.RelativeOrAbsolute);

        private readonly WorkspaceFactory _workspaceFactory;

        public ExampleWorkspaceDescriptor(WorkspaceFactory workspaceFactory)
        {
            _workspaceFactory = workspaceFactory;
        }

        public int Position => 0;

        public string Name => "Example Workspace";

        public Workspace CreateWorkspace()
        {
            return _workspaceFactory.Create<Registrar, ExampleController>(_resources);
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class Registrar
        {
            // Scoped registrations, these will be thrown away when the workspace is disposed...
            public Registrar(ContainerBuilder container)
            {
                container.RegisterType<ExampleViewModel>();
                container.RegisterType<ExampleController>();
            }
        }
    }
}