using DynamicData;

namespace LionFire.DynamicData.Orleans;

[GenerateSerializer]
public struct ChangeSet_Surrogate<T>
{
    [Id(0)]
    public List<Change<T>> Data;

}


[RegisterConverter]
public sealed class ChangeSet_SurrogateConverter<T> : IConverter<ChangeSet<T>, ChangeSet_Surrogate<T>>
{
    public ChangeSet<T> ConvertFromSurrogate(in ChangeSet_Surrogate<T> surrogate) => new(surrogate.Data);

    public ChangeSet_Surrogate<T> ConvertToSurrogate(in ChangeSet<T> value) => new()
    {
        Data = value
    };
}
