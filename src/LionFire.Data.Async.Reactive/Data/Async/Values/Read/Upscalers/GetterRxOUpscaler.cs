
using LionFire.Data.Mvvm;

namespace LionFire.Data.Async.Gets;

public static class GetterRxOUpscaler
{
    public static IGetterRxO<TValue>? TryUpscale<TValue>(this object value)
    {
        return value switch
        {
            IGetterRxO<TValue> full => full,
            IGetter<TValue> getter => new GetterToRxO<TValue>(getter),
            IStatelessGetter<TValue> getter => new StatelessGetterToRxO<TValue>(getter),
            _ => null,
        };

        //if (value is IGetterRxO<TValue> full) { return full; }
        //if (value is IGetter<TValue> getter) { return new GetterToRxO<TValue>(getter); }
        //if (value is IStatelessGetter<TValue> getter) { return new StatelessGetterToRxO<TValue>(getter); }
    }
    public static IGetterRxO<TValue>? Upscale<TValue>(this object value) => TryUpscale<TValue>(value) ?? throw new ArgumentException($"Could not upscale unsupported type {value?.GetType().FullName} to IGetterRxO<{typeof(TValue)}>");
}
