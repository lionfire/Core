namespace LionFire.Data.Async;

internal interface IGetsInternal<TValue> //: IAsyncGetsRx<TValue>
{
    IEqualityComparer<TValue> EqualityComparer { get; }
}
