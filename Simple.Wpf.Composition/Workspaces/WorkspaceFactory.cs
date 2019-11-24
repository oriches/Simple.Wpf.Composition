using System;
using Autofac;
using Autofac.Core;
using Simple.Wpf.Composition.Infrastructure;

namespace Simple.Wpf.Composition.Workspaces
{
    public sealed class WorkspaceFactory
    {
        private readonly ILifetimeScope _parentScope;

        public WorkspaceFactory(ILifetimeScope parentScope)
        {
            _parentScope = parentScope;
        }

        public Workspace Create<TRegistrar, TController>(Uri resourceUri) where TController : BaseController
        {
            if (_parentScope == null) throw new Exception("Autofac parent scope has not been defined!");

            try
            {
                // ReSharper disable once ConvertClosureToMethodGroup
                var childScope = _parentScope.BeginLifetimeScope(x => RunRegistrar<TRegistrar>(x));

                // First thing we do is create the child workspace factory and associate the child scope - workspace factory
                // is registered as singleton per scope in the bootstrapper...
                //
                // Doing this means the correct scope is associated with the workspace...
                Parameter[] parameters = {new NamedParameter("scope", childScope)};
                childScope.Resolve<WorkspaceFactory>(parameters);

                return new Workspace(childScope.Resolve<TController>(), resourceUri, childScope.Dispose);
            }
            catch (Exception exn)
            {
                throw new Exception("Failed to create Workspace!", exn);
            }
        }

        private static void RunRegistrar<TRegistrar>(ContainerBuilder containerBuilder)
        {
            // This creates an instance of the required registrar and passes the scoped container builder in the constructor
            var constructor = typeof(TRegistrar).GetConstructor(new[] {typeof(ContainerBuilder)});
            if (constructor == null) throw new Exception("Failed to find workspace registrar constructor!");

            constructor.Invoke(new object[] {containerBuilder});
        }
    }
}