namespace Simple.Wpf.Composition.Workspaces.Logging
{
    using System;

    public interface ILogReader
    {
        IObservable<string> Entries { get; }
    }
}