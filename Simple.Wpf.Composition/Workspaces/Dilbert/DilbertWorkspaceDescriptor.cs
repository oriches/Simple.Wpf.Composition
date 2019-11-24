using System;
using Autofac;
using Dilbert;

namespace Simple.Wpf.Composition.Workspaces.Dilbert
{
    public sealed class DilbertWorkspaceDescriptor : IWorkspaceDescriptor
    {
        private readonly Uri _resources = new Uri("/simple.wpf.composition;component/workspaces/dilbert/resources.xaml",
            UriKind.RelativeOrAbsolute);

        private readonly WorkspaceFactory _workspaceFactory;

        public DilbertWorkspaceDescriptor(WorkspaceFactory workspaceFactory)
        {
            _workspaceFactory = workspaceFactory;
        }

        public int Position => 0;

        public string Name => "Dilbert Workspace";

        public Workspace CreateWorkspace()
        {
            return _workspaceFactory.Create<Registrar, DilbertController>(_resources);
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class Registrar
        {
            // Scoped registrations, these will be thrown away when the workspace is disposed...
            public Registrar(ContainerBuilder container)
            {
                container.RegisterType<DailyDilbertService>().As<IDailyDilbertService>().InstancePerLifetimeScope();


                container.RegisterType<DilbertViewModel>();
                container.RegisterType<DilbertController>();
            }
        }
    }
}