using System;
using System.Reflection;

namespace Simple.Wpf.Composition.Core.Services
{
    public interface IDiagnosticsService
    {
        IObservable<Assembly> LoadedAssembly { get; }

        IObservable<DiagnosticsService.Values> PerformanceCounters(int interval);
    }
}