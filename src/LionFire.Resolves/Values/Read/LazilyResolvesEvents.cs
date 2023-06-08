#nullable enable
using System;

namespace LionFire.Data.Async.Gets;

public static class LazilyResolvesEvents
{
    public static event Action<(Type valueType, ILazilyResolves resolves, object? from, object? to)>? ValueChanged;

    public static void RaiseValueChanged<TValue>(ILazilyResolves<TValue> resolves, object? oldValue, object? newValue)
    {
        ValueChanged?.Invoke((typeof(TValue), resolves, oldValue, newValue));
    }
}

