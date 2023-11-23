//#nullable enable

//using DynamicData;

//namespace LionFire.Blazor.Components;

//public class AsyncComposableObservableCache_OLD<TKey, TValue> : AsyncObservableCache_OLD<TKey, TValue>
//    where TKey : notnull
//{
//    public Func<TValue, Task>? AddOrUpdateFunc { get; set; }

//    public override async Task AddOrUpdate(TValue item)
//    {
//        if (AddOrUpdateFunc != null) { await AddOrUpdateFunc(item).ConfigureAwait(false); }
//        SourceCache.AddOrUpdate(item);
//    }
//}