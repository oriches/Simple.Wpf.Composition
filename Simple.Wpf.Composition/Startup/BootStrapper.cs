namespace Simple.Wpf.Composition.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Autofac.Core;
    using Workspaces;

    public static class BootStrapper
    {
        private static ILifetimeScope _rootScope;
        private static MainController _rootController;

        public static BaseViewModel RootVisual
        {
            get
            {
                if (_rootScope == null)
                {
                    Run();
                }

                _rootController = _rootScope.Resolve<MainController>();
                return _rootController.ViewModel;
            }
        }
        
        private static void Run()
        {
            if (_rootScope != null)
            {
                return;
            }

            var builder = new ContainerBuilder();

            // Register the application 'chrome' stuff...
            builder.RegisterType<MainViewModel>();
            builder.RegisterType<MainController>();
            
            // Register all the workspace descriptors in the assembly...
            foreach (var type in GetWorkspaceDescriptorTypes())
            {
                builder.RegisterType(type).As(typeof(IWorkspaceDescriptor));
            }

            builder.RegisterType<WorkspaceFactory>().InstancePerLifetimeScope();
            
            _rootScope = builder.Build();
            _rootScope.Resolve<WorkspaceFactory>(new Parameter[]{ new NamedParameter("scope", _rootScope) });
        }

        private static IEnumerable<Type> GetWorkspaceDescriptorTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(x => typeof(IWorkspaceDescriptor).IsAssignableFrom(x) && x.IsClass);
        }
    }
}