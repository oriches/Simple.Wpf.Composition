namespace Simple.Wpf.Composition.Workspaces.Logging
{
    using System;
    using System.Collections.Generic;

    public interface ILogReader
    {
        IObservable<IEnumerable<string>> GetInMemoryEntries();
    }
}