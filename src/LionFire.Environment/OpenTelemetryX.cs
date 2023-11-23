#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace LionFire;

public static class OpenTelemetryX
{
    public static bool ThrowOnMultiApplicationEnvironmentMissingContext = true;

    public static void IncrementWithContext(this Counter<long> counter)
    {
        if (LionFireEnvironment.ContextTags != null)
        {
            counter.Add(1, LionFireEnvironment.ContextTags);
        }
        else
        {
            if (ThrowOnMultiApplicationEnvironmentMissingContext && LionFireEnvironment.IsMultiApplicationEnvironment) throw new Exception();
            counter.Add(1);
        }
    }
    public static void IncrementWithContext(this Counter<long> counter, long delta)
    {
        if (LionFireEnvironment.ContextTags != null)
        {
            counter.Add(delta, LionFireEnvironment.ContextTags);
        }
        else
        {
            if (ThrowOnMultiApplicationEnvironmentMissingContext && LionFireEnvironment.IsMultiApplicationEnvironment) throw new Exception();
            counter.Add(delta);
        }
    }
#if NET7_0_OR_GREATER
    public static void IncrementWithContext(this UpDownCounter<long> counter)
    {
        if (LionFireEnvironment.ContextTags != null)
        {
            counter.Add(1, LionFireEnvironment.ContextTags );
        }
        else
        {
            if (ThrowOnMultiApplicationEnvironmentMissingContext && LionFireEnvironment.IsMultiApplicationEnvironment) throw new Exception();
            counter.Add(1);
        }
    }
#endif
    //static Action<Counter<long>> AddAction;

    public static void RecordWithContext(this Histogram<long> counter, long value)
    {
        if (LionFireEnvironment.ContextTags != null)
        {
            counter.Record(value, LionFireEnvironment.ContextTags);
        }
        else
        {
            if (ThrowOnMultiApplicationEnvironmentMissingContext && LionFireEnvironment.IsMultiApplicationEnvironment) throw new Exception();
            counter.Record(value);
        }
    }
}
