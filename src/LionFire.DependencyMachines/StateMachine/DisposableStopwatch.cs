using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LionFire.DependencyMachines;


public class DisposableStopwatch : IDisposable
{
    private readonly Action<Stopwatch>? s;
    private readonly Stopwatch sw;

    public DisposableStopwatch([CallerMemberName] string? name = null)
    {
        s = sw => Log.Get(name ?? nameof(DisposableStopwatch)).LogDebug($"{(name ?? "?")} took {sw.ElapsedMilliseconds}ms");
        sw = Stopwatch.StartNew();
    }

    public DisposableStopwatch(Action<Stopwatch>? s)
    {
        this.s = s;
        sw = Stopwatch.StartNew();
    }

    public void Dispose()
    {
        sw.Stop();
        s?.Invoke(sw);
    }
}

