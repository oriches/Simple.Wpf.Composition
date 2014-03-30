namespace Simple.Wpf.Composition.Services
{
    using System;
    using System.Reflection;

    public interface IDiagnosticsService
    {
        IObservable<Assembly> LoadedAssembly { get; }

        IObservable<DiagnosticsService.Values> PerformanceCounters(int interval);
    }
}