
namespace LionFire.Data.Async.Gets;

public static class LazilyGetsEvents
{
    public static event Action<(Type valueType, ILazilyGets resolves, object? from, object? to)>? ValueChanged;

    public static void RaiseValueChanged<TValue>(ILazilyGets<TValue> resolves, object? oldValue, object? newValue)
    {
        ValueChanged?.Invoke((typeof(TValue), resolves, oldValue, newValue));
    }
}

