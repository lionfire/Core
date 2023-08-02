namespace LionFire.Data;

internal interface IGetsInternal<TValue> //: IAsyncGetsRx<TValue>
{
    IEqualityComparer<TValue> EqualityComparer { get; }
}
