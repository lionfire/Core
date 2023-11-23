namespace LionFire.Data.Async;

public static class IValueStateX
{
    public static IValueState<TValueExposed> Cast<TValueReal, TValueExposed>(this IValueState<TValueReal> v)
    {
        return new ValueStateCoAndContraVariant<TValueReal, TValueExposed>(v);
    }
}
