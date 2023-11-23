
namespace LionFire.Data.Async.Gets;

public static class LazyGetterEvents
{
    public static event Action<(Type valueType, ILazyGetter resolves, object? from, object? to)>? ValueChanged;

    public static void RaiseValueChanged<TValue>(IGetter<TValue> resolves, object? oldValue, object? newValue)
    {
        ValueChanged?.Invoke((typeof(TValue), resolves, oldValue, newValue));
    }
}

