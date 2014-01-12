namespace Simple.Wpf.Composition.Workspaces.FSharp.Repl
{
    using System;
    using Autofac;
    using Wpf.FSharp.Repl.UI.Controllers;
    using Wpf.FSharp.Repl.UI.ViewModels;

    public sealed class FSharpReplWorkspaceDescriptor : IWorkspaceDescriptor
    {
        private readonly Uri _resources = new Uri("/simple.wpf.composition;component/workspaces/fsharp.repl/resources.xaml", UriKind.RelativeOrAbsolute);

        private readonly WorkspaceFactory _workspaceFactory;

        public FSharpReplWorkspaceDescriptor(WorkspaceFactory workspaceFactory)
        {
            _workspaceFactory = workspaceFactory;
        }

        public int Position { get { return 0; } }

        public string Name { get { return "F# REPL Workspace"; } }

        public Workspace CreateWorkspace()
        {
            return _workspaceFactory.Create<Registrar, FSharpReplController>(_resources);
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class Registrar
        {
            // Scoped registrations, these will be thrown away when the workspace is disposed...
            public Registrar(ContainerBuilder container)
            {
                container.RegisterType<FSharpReplViewModel>();
                container.RegisterType<FSharpReplController>();

                container.RegisterType<ReplEngineController>()
                    .As<IReplEngineController>()
                    .WithParameter(new NamedParameter("workingDirectory", @"c:\temp\fsharp"));

                container.RegisterType<ReplEngineViewModel>()
                    .As<IReplEngineViewModel>();
            }
        }
    }
}