using DynamicData;

namespace LionFire.DynamicData_.Orleans;




[GenerateSerializer]
public struct ChangeSet_Surrogate<TItem, TKey>
    where TKey:notnull
{
    [Id(0)]
    public Change<TItem, TKey>[] Data;

}


[RegisterConverter]
public sealed class ChangeSet_SurrogateConverter<TItem, TKey> : IConverter<ChangeSet<TItem, TKey>, ChangeSet_Surrogate<TItem, TKey>>
    where TKey : notnull
{
    public ChangeSet<TItem, TKey> ConvertFromSurrogate(in ChangeSet_Surrogate<TItem, TKey> surrogate) => new(surrogate.Data);

    public ChangeSet_Surrogate<TItem, TKey> ConvertToSurrogate(in ChangeSet<TItem, TKey> value) => new()
    {
        Data = value.ToArray(),
    };
}


