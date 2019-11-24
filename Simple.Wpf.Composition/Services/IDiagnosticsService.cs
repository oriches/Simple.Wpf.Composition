using System;
using System.Reflection;

namespace Simple.Wpf.Composition.Services
{
    public interface IDiagnosticsService
    {
        IObservable<Assembly> LoadedAssembly { get; }

        IObservable<DiagnosticsService.Values> PerformanceCounters(int interval);
    }
}