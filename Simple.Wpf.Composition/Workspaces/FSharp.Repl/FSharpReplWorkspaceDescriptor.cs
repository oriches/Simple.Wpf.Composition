using System;
using Autofac;
using Simple.Wpf.FSharp.Repl.UI.Controllers;
using Simple.Wpf.FSharp.Repl.UI.ViewModels;

namespace Simple.Wpf.Composition.Workspaces.FSharp.Repl
{
    public sealed class FSharpReplWorkspaceDescriptor : IWorkspaceDescriptor
    {
        private readonly Uri _resources =
            new Uri("/simple.wpf.composition;component/workspaces/fsharp.repl/resources.xaml",
                UriKind.RelativeOrAbsolute);

        private readonly WorkspaceFactory _workspaceFactory;

        public FSharpReplWorkspaceDescriptor(WorkspaceFactory workspaceFactory)
        {
            _workspaceFactory = workspaceFactory;
        }

        public int Position => 0;

        public string Name => "F# REPL Workspace";

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