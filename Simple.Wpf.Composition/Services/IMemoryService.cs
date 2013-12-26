namespace Simple.Wpf.Composition.Services
{
    using System;

    public interface IMemoryService
    {
        IObservable<long> MemoryInBytes { get;  }

        IObservable<decimal> MemoryInKiloBytes { get;  }

        IObservable<decimal> MemoryInMegaBytes { get; }

        IObservable<decimal> MemoryInGigaBytes { get; }
    }
}
