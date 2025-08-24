using DynamicData.Kernel;

namespace LionFire.DynamicData_.Orleans;

[GenerateSerializer]
public struct Optional_Surrogate<TValue>
{
    public bool HasValue;
    public TValue? Value;
}


[RegisterConverter]
public sealed class Optional_SurrogateConverter<TValue> : IConverter<Optional<TValue>, Optional_Surrogate<TValue>>
    where TValue : notnull
{
    public Optional<TValue> ConvertFromSurrogate(in Optional_Surrogate<TValue> surrogate)
        => surrogate.HasValue ? Optional.Some(surrogate.Value) : Optional.None<TValue>();

    public Optional_Surrogate<TValue> ConvertToSurrogate(in Optional<TValue> value)
        => new Optional_Surrogate<TValue> { HasValue = value.HasValue, Value = value.HasValue ? value.Value : default(TValue) };

}

