#nullable enable
using Orleans.Hosting;

namespace LionFire.Hosting;

public static class ISiloBuilderX
{
    public static ISiloBuilder If(this ISiloBuilder siloBuilder, Func<bool> predicate, Action<ISiloBuilder> a)
    {
        if (predicate()) a(siloBuilder);
        return siloBuilder;
    }
    public static ISiloBuilder If(this ISiloBuilder siloBuilder, bool predicate, Action<ISiloBuilder> a)
    {
        if (predicate) a(siloBuilder);
        return siloBuilder;
    }
}
