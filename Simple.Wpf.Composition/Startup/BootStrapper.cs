namespace Simple.Wpf.Composition.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Autofac.Core;
    using Infrastructure;
    using Services;
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
                    Start();
                }

                _rootController = _rootScope.Resolve<MainController>();
                return _rootController.ViewModel;
            }
        }
        
        public static void Start()
        {
            if (_rootScope != null)
            {
                return;
            }

            var builder = new ContainerBuilder();

            // Register the application 'chrome' stuff...
            builder.RegisterType<MainViewModel>();
            builder.RegisterType<MainController>();

            builder.RegisterType<MemoryService>().As<IMemoryService>().InstancePerLifetimeScope();
            
            // Register all the workspace descriptors in the assembly...
            foreach (var type in GetWorkspaceDescriptorTypes())
            {
                builder.RegisterType(type).As(typeof(IWorkspaceDescriptor));
            }

            builder.RegisterType<WorkspaceFactory>().InstancePerLifetimeScope();

            
            _rootScope = builder.Build();
            _rootScope.Resolve<WorkspaceFactory>(new Parameter[]{ new NamedParameter("scope", _rootScope) });
        }

        public static void Stop()
        {
            _rootController.Dispose();
            _rootScope.Dispose();

            _rootController = null;
            _rootScope = null;
        }

        private static IEnumerable<Type> GetWorkspaceDescriptorTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(x => typeof(IWorkspaceDescriptor).IsAssignableFrom(x) && x.IsClass);
        }
    }
}