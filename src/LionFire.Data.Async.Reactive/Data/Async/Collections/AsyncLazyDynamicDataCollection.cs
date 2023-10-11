// OLD - Main classes are lazy now
namespace LionFire.Data.Collections;

///// <remarks>
///// State:
/////  - ReadCacheValue
/////  
///// Implements:
/////  - Get
///// 
///// Abstract:
/////  - GetImpl
///// </remarks>
///// <typeparam name="TValue"></typeparam>
//public abstract partial class AsyncLazyDynamicDataCollection<TValue> : AsyncDynamicDataCollection<TValue>
//{
//    public override void DiscardValue() => ReadCacheValue = Enumerable.Empty<TValue>();

//    public override IEnumerable<TValue>? ReadCacheValue { get; } = Enumerable.Empty<TValue>();


//    //public override async ITask<IGetResult<IEnumerable<TValue>>> Get(CancellationToken cancellationToken = default)
//    //{
//    //    // TODO: THREADSAFETY, see code from non-collection 

//    //    var t = await GetImpl(cancellationToken).ConfigureAwait(false);

//    //    if (t.IsSuccess())
//    //    {
//    //        ReadCacheValue = t.Value;
//    //    }
//    //    else
//    //    {
//    //        // TODO: if Discard cache on Fail, discard it here
//    //    }
//    //    return t;
//    //}

//    //public abstract ITask<IGetResult<IEnumerable<TValue>>> GetImpl(CancellationToken cancellationToken = default);
//}
