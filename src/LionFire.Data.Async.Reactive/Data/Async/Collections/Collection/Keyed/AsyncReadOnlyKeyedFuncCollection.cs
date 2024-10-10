using DynamicData;
using LionFire.Data.Async;

namespace LionFire.Data.Collections;

public class AsyncReadOnlyKeyedFuncCollection<TKey, TValue> : AsyncReadOnlyKeyedCollection<TKey, TValue>
    , IEnumerable<TValue>
    , IObservableCacheKeyableGetter<TKey, TValue>
    where TKey : notnull//, IEquatable<TKey>
    where TValue : notnull//, IKeyed<TKey>
{

    //#if OLD
    #region (static)

    private static Func<TValue, TKey> GetGetKey()
    //static AsyncReadOnlyKeyedFuncCollection()
    {
        if (GetKey == null)
        {
            if (typeof(TValue).IsAssignableTo(typeof(IKeyed<TKey>)))
            {
                GetKey = i => ((IKeyed<TKey>)i).Key;
            }
#if REVIEW // was present while in a DLL that had Orleans.  Might still need this, but starting on migrating to try another way.  
        else if (typeof(TKey) == typeof(string) && typeof(TValue).IsAssignableTo(typeof(IKeyedG)))
        {
            GetKey = i => (TKey)(object)((Orleans.IGrainWithStringKey)(object)i).GetPrimaryKeyString();
            //GetKey = i => (TKey)(object)((IKeyedG)i).Key().Result; // BLOCKING and broken
        }
#endif
            else
            {
                throw new NotSupportedException("TValue must implement IKeyed<TKey> or IKeyedG<TKey>, or else GetKey static field must first be set.");
            }
        }
        return GetKey;
    }
    public static Func<TValue, TKey>? GetKey { get; set; }
    #endregion

    //#else
    //public static TKey GetKey(TValue item) => item.Key;
    //#endif

    #region Parameters

    public Func<ValueTask<IEnumerable<TValue>>> Func { get; }

    #endregion

    #region Lifecycle

    public AsyncReadOnlyKeyedFuncCollection(Func<ValueTask<IEnumerable<TValue>>> func, IObservable<IChangeSet<TValue, TKey>>? keyValueChanges = null) : base(GetGetKey(), keyValueChanges: keyValueChanges)
    {
        Func = func;
    }

    #endregion

    //#region IObservableCacheKeyableGetter

    //public Func<TValue, TKey> KeySelector => i => i.Key;

    //IObservableCache<TValue, TKey> IHasObservableCache<TValue, TKey>.ObservableCache => throw new NotImplementedException();

    //#endregion

    #region State

    public override IEnumerable<TValue>? ReadCacheValue => readCacheValue;
    private IEnumerable<TValue>? readCacheValue;

    public override void DiscardValue() => readCacheValue = null;

    #endregion

    protected override async ITask<IGetResult<IEnumerable<TValue>>> GetImpl(CancellationToken cancellationToken = default)
        => GetResult<IEnumerable<TValue>>.Success(await Func());


    #region Event Handling

    public override void OnNext(IGetResult<IEnumerable<TValue>> result)
    {
        readCacheValue = result.Value;
        SourceCache.EditDiff(readCacheValue ?? [], (a, b) => GetKey(a).Equals(GetKey(b)));
    }

    #endregion
}

