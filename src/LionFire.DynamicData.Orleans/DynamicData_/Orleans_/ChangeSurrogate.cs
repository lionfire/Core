using DynamicData;
using DynamicData.Kernel;

namespace LionFire.DynamicData_.Orleans;


[GenerateSerializer]
public struct Change_Surrogate<TItem, TKey>
{
    [Id(0)]
    public ChangeReason Reason;

    [Id(1)]
    public TKey Key;

    [Id(2)]
    public TItem Current;

    [Id(3)]
    public Optional<TItem> Previous;
    [Id(4)]
    public int CurrentIndex;
    [Id(5)]
    public int PreviousIndex;
}

[RegisterConverter]
public sealed class Change_SurrogateConverter<TItem, TKey> : IConverter<Change<TItem, TKey>, Change_Surrogate<TItem, TKey>>
    where TItem : notnull
    where TKey : notnull
{
    public Change<TItem, TKey> ConvertFromSurrogate(in Change_Surrogate<TItem, TKey> surrogate) 
        => new Change<TItem, TKey>(
            surrogate.Reason, 
            surrogate.Key, 
            surrogate.Current, 
            surrogate.Previous, 
            surrogate.CurrentIndex, 
            surrogate.PreviousIndex);

    public Change_Surrogate<TItem, TKey> ConvertToSurrogate(in Change<TItem, TKey> value) =>
        new()
        {
            Reason = value.Reason,
            Key = value.Key,
            Current = value.Current,
            Previous = value.Previous,
            CurrentIndex = value.CurrentIndex,
            PreviousIndex = value.PreviousIndex
        };
}


#if FUTURE // List Change
[RegisterConverter]
public sealed class Change_SurrogateConverter<TItem> : IConverter<Change<TItem>, Change_Surrogate<TItem>>
    where TKey : notnull
{
    public Change<TItem, TKey> ConvertFromSurrogate(in Change_Surrogate<TItem> surrogate)
    {

        return surrogate.Reason switch
        {
            ListChangeReason.Add
            or ListChangeReason.Remove
                => new(surrogate.Reason, surrogate.Item.Current, surrogate.Item.CurrentIndex),

            ListChangeReason.AddRange
            or ListChangeReason.RemoveRange
                => new(surrogate.Reason, surrogate.Range, surrogate.Range.Index),

            //ChangeReason.Moved => new(surrogate.Reason, surrogate.Item.Current, surrogate.Item.Previous, surrogate.Item.CurrentIndex, surrogate.Item.PreviousIndex),
            //or ChangeReason.Replace

            ListChangeReason.Clear
            or ListChangeReason.Refresh
            => new(surrogate.Reason, (T)default),
            _ => throw new NotImplementedException(),
        };
    }

    public Change_Surrogate<TItem> ConvertToSurrogate(in Change<TItem> value) =>
        new()
        {
            Item = value.Item,
            Range = value.Range,
            Reason = value.Reason
        };
}
#endif