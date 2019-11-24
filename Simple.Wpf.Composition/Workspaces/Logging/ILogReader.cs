using System;

namespace Simple.Wpf.Composition.Workspaces.Logging
{
    public interface ILogReader
    {
        IObservable<string> Entries { get; }
    }
}