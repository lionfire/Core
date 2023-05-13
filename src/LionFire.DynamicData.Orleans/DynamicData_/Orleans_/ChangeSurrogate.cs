using DynamicData;

namespace LionFire.DynamicData.Orleans;

[GenerateSerializer]
public struct Change_Surrogate<T>
{
    [Id(0)]
    public ItemChange<T> Item;

    [Id(1)]
    public RangeChange<T> Range;

    [Id(2)]
    public ListChangeReason Reason;
}

[RegisterConverter]
public sealed class Change_SurrogateConverter<T> : IConverter<Change<T>, Change_Surrogate<T>>
{
    public Change<T> ConvertFromSurrogate(in Change_Surrogate<T> surrogate)
    {

        return surrogate.Reason switch
        {
            ListChangeReason.Add 
            or ListChangeReason.Remove
                => new(surrogate.Reason, surrogate.Item.Current, surrogate.Item.CurrentIndex),

            ListChangeReason.AddRange 
            or ListChangeReason.RemoveRange
                => new(surrogate.Reason, surrogate.Range, surrogate.Range.Index),

            //ListChangeReason.Moved => new(surrogate.Reason, surrogate.Item.Current, surrogate.Item.Previous, surrogate.Item.CurrentIndex, surrogate.Item.PreviousIndex),
            //or ListChangeReason.Replace

            ListChangeReason.Clear 
            or ListChangeReason.Refresh
            => new(surrogate.Reason, (T)default),
            _ => throw new NotImplementedException(),
        };
    }

    public Change_Surrogate<T> ConvertToSurrogate(in Change<T> value) =>
        new()
        {
            Item = value.Item,
            Range = value.Range,
            Reason = value.Reason
        };
}
