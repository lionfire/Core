//using CommunityToolkit.Mvvm.ComponentModel;
//using DynamicData;
//using LionFire.Mvvm;
//using Microsoft.Extensions.DependencyInjection;
//using ObservableCollections;

//namespace LionFire.DynamicData_;

///// <summary>
///// 
///// </summary>
///// <typeparam name="TItem"></typeparam>
///// <typeparam name="TValue"></typeparam>
///// <remarks>Implementors: override either RetrieveValues or RetrieveImpl</remarks>
//public abstract partial class AsyncSourceCache<TItem, TValue> :
//    AsyncSourceCacheBase2<TItem, TValue>
//    where TItem : notnull
//{
//    public AsyncSourceCache(Func<TValue, TItem> keySelector, ObservableDictionary<TItem, TValue>? dictionary = null, AsyncObservableCollectionOptions? options = null) : base(keySelector, dictionary, options)
//    {
//    }

//    protected override async Task<IEnumerable<KeyValuePair<TItem, TValue>>> RetrieveImpl(CancellationToken cancellationToken = default)
//        => (await RetrieveValues(cancellationToken)).Select(i => new KeyValuePair<TItem, TValue>(KeySelector(i), i));

//    protected virtual Task<IEnumerable<TValue>> RetrieveValues(CancellationToken cancellationToken = default) => throw new Exception($"Override either {nameof(RetrieveValues)} or {nameof(RetrieveImpl)}");

//}


//public abstract partial class AsyncSourceCacheBase2<TItem, TValue> : AsyncSourceCacheBase3<>
//    where TItem : notnull
//    where SourceCache<TValue, TItem> : class, IReadOnlyDictionary<TItem, TValue>, IObservableCollection<KeyValuePair<TItem, TValue>>, new()
//{
//    protected Func<TValue, TItem> KeySelector { get; }
//    #region Lifecycle

//    public AsyncSourceCacheBase2(Func<TValue, TItem> keySelector, SourceCache<TValue, TItem>? dictionary = null, AsyncObservableCollectionOptions? options = null) : base(dictionary, options)
//    {
//        KeySelector = keySelector;
//        sourceCache = new SourceCache<TValue, TItem>(keySelector);
//    }

//    #endregion

//    protected SourceCache<TValue, TItem> sourceCache;
//}

///// <summary>
///// ObservableCache for an underlying collection that is accessed asynchronously
///// </summary>
///// <typeparam name="TValue"></typeparam>
///// <typeparam name="SourceCache<TValue, TItem>">must be a CySharp/ObservableCollections IObservableCollection&lt;TValue&gt; type</typeparam>
//public abstract partial class AsyncSourceCacheBase3<TItem, TValue> : ObservableObject, IAsyncCollectionCache<KeyValuePair<TItem, TValue>>, IAsyncCreates<TValue>
//    where SourceCache<TValue, TItem> : class, IObservableCollection<TValue>, new ()
//{
//    #region Parameters

//    public AsyncObservableCollectionOptions GetOptions { get; set; }

//#region CreateableTypes

//public virtual IEnumerable<Type>? CreateableTypes { get => createableTypes ??= DefaultCreateableTypes; set => createableTypes = value; }
//private IEnumerable<Type>? createableTypes;
//public static IEnumerable<Type> DefaultCreateableTypes
//{
//    get
//    {
//        // TODO: use Type introspection service to get list types that derive from TValue
//        // alternate: inspect TValue for attributes
//        return Enumerable.Empty<Type>();
//    }
//}

//#endregion

//#region ServiceProvider (for TValue activation)

///// <summary>
///// IServiceProvider used for TValue activation
///// </summary>
//public virtual IServiceProvider? ServiceProvider => GetServiceProviderForItemActivation(this);
//public static Func<AsyncSourceCacheBase3<TValue, SourceCache<TValue, TItem>>, IServiceProvider?> GetServiceProviderForItemActivation = _ => DependencyContext.Current?.ServiceProvider;

//#endregion

//#endregion

//#region Lifecycle

//public AsyncSourceCacheBase3() { GetOptions = AsyncObservableCollectionOptions.Default; }
//public AsyncSourceCacheBase3(SourceCache<TValue, TItem>? collection = default, AsyncObservableCollectionOptions? options = null, bool blockOnSetCollection = false)
//{
//    if (!EqualityComparer<SourceCache<TValue, TItem>>.Default.Equals(collection, default)) { var task = SetCollection(collection); if (blockOnSetCollection) task.Wait(); }
//    GetOptions = options ?? AsyncObservableCollectionOptions.Default;
//}

//#endregion

//#region State

//object _lock = new();

//#region DictionaryCache

///// <summary>
///// 
///// </summary>
///// <remarks>
///// Property type is IObservableCollection&lt;TValue&gt; which provides only read-only access.
///// </remarks>
//public virtual IObservableCollection<TValue> DictionaryCache
//{
//    get
//    {
//        if (collection == null)
//        {
//            if (GetOptions.AutoInstantiateCollection)
//            {
//                lock (_lock)
//                {
//                    if (collection == null)
//                    {
//                        DictionaryCache = new SourceCache<TValue, TItem>();
//                    }
//                }
//            }
//            else
//            {
//                throw new ArgumentNullException(nameof(DictionaryCache));
//            }
//        }
//        return collection!;
//    }
//    set
//    {
//        if (value != null && value is not SourceCache<TValue, TItem>) throw new ArgumentException($"{nameof(value)} must be of type {typeof(SourceCache<TValue, TItem>).FullName}");
//        SetCollection((SourceCache<TValue, TItem>?)value).FireAndForget();
//    }
//}
//protected SourceCache<TValue, TItem>? collection;

//public async Task SetCollection(SourceCache<TValue, TItem>? collection, CancellationToken cancellationToken = default)
//{
//    if (ReferenceEquals(this.collection, collection)) { return; }
//    lock (_lock)
//    {
//        if (this.collection != null) { throw new AlreadySetException(); }
//        this.collection = collection;
//    }
//    if (GetOptions.AutoSync)
//    {
//        await Retrieve(cancellationToken: cancellationToken).ConfigureAwait(false);
//    }
//}

//#endregion

//#endregion

//// vvv REVIEW vvv

//#region Sync

//public bool Sync
//{
//    get => sync;
//    set
//    {
//        if (sync == value) return;
//        SetProperty(ref sync, value);
//        SetSync(sync).FireAndForget();
//    }
//}
//private bool sync;
//public event EventHandler<Exception> SyncError;
//private async Task SetSync(bool sync)
//{
//    if (sync)
//    {
//        try
//        {
//            var canSubscribe = CanSubscribe;
//            bool subscribed = false;
//            if (canSubscribe)
//            {
//                subscribed = await Subscribe().ConfigureAwait(false);
//            }
//            // Wait for Subscribe to finish before Retrieving to make sure we don't miss updates
//            if ((!canSubscribe || subscribed) && GetOptions.AlwaysRetrieveOnEnableSync && CanRetrieve)
//            {
//                await Retrieve().ConfigureAwait(false);
//            }
//            if (!canSubscribe)
//            {
//                Sync = false; // We are not really subscribed.  This is a one-shot retrieve.
//            }
//        }
//        catch (Exception ex)
//        {
//            SetProperty(ref sync, false);
//            SyncError?.Invoke(this, ex);
//        }
//    }
//    else
//    {
//        if (CanSubscribe)
//        {
//            await Unsubscribe().ConfigureAwait(false);
//        }
//    }
//}

//#endregion

//#region Subscription

///// <summary>
///// Can call Subscribe() and Unsubscribe().  Do not call these directly; set Sync instead.
///// </summary>
//public virtual bool CanSubscribe => false;

//SourceList<TValue> SourceList = new();

////public IDisposable Subscribe(IObserver<IChangeSet<TValue>>)
////{
////}

///// <summary>
///// 
///// </summary>
///// <returns>true if actually subscribed to something during this invocation, false if already subscribed (throw on fail)</returns>
///// <exception cref="NotSupportedException"></exception>
//protected virtual Task<bool> Subscribe() => throw new NotSupportedException();
////protected virtual IObservable<ChangeSet<>> SubscribeRx() => throw new NotSupportedException();

//protected virtual Task Unsubscribe() => throw new NotSupportedException();

//#endregion

//#region DictionaryCache-related

//public int Count => collection?.Count ?? 0;

//public IEnumerator<TValue> GetEnumerator()
//    => collection?.GetEnumerator() ?? Enumerable.Empty<TValue>().GetEnumerator();

//IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

//public virtual bool IsReadOnly => true;

//#endregion

//#region Retrieve

//public bool HasRetrieved { get => hasRetrieved; protected set => SetProperty(ref hasRetrieved, value); }
//private bool hasRetrieved;

//public bool IsRetrieving { get => isRetrieving; protected set => SetProperty(ref isRetrieving, value); }
//private bool isRetrieving;

//public virtual bool CanRetrieve => false;

//public async Task<IEnumerable<TValue>> Retrieve(bool syncToCollection = true, CancellationToken cancellationToken = default)
//{
//    if (!CanRetrieve) { throw new InvalidOperationException($"!{nameof(CanRetrieve)}"); }

//    var result = await RetrieveImpl(cancellationToken);

//    if (syncToCollection)
//    {
//        if (DictionaryCache == null)
//        {
//            DictionaryCache = new SourceCache<TValue, TItem>();
//        }
//        if (DictionaryCache is ObservableHashSet<TValue> h) result.SyncTo(h);
//        else result.SyncTo(DictionaryCache);
//    }

//    return result;
//}
//protected virtual Task<IEnumerable<TValue>> RetrieveImpl(CancellationToken cancellationToken = default)
//{
//    throw new NotSupportedException();
//}

//#endregion

//#region Instantiate

//public virtual bool CanInstantiate => true;
//public virtual TValue Instantiate(Type type, params object[] parameters)
//{
//    TValue? newItem = default;
//    var serviceProvider = ServiceProvider;
//    if (serviceProvider != null)
//    {
//        newItem = (TValue)ActivatorUtilities.CreateInstance(serviceProvider, type, parameters);
//    }

//    if (EqualityComparer<TValue>.Default.Equals(newItem, default))
//    {
//        newItem = (TValue)Activator.CreateInstance(type, parameters)!;
//    }
//    if (EqualityComparer<TValue>.Default.Equals(newItem, default))
//    {
//        throw new ArgumentException($"Failed to instantiate type {type.FullName}");
//    }
//    return newItem!;
//}

//#endregion

//#region Create

//public virtual bool CanCreate => false;
//public virtual Task<TValue> Create(Type type, params object[]? constructorParameters)
//{
//    throw new NotSupportedException();
//    //if (!CanCreate) { throw new NotSupportedException($"!{nameof(CanCreate)}"); }

//    //return (TValue)(Activator.CreateInstance(type, constructorParameters) ?? throw new Exception("CreateInstance failed for type: " + type.FullName));
//}
//#endregion

//#region Add

//public virtual IEnumerable<TValue> Adding => Enumerable.Empty<TValue>();

//public virtual bool CanAdd => false;
//public virtual Task Add(TValue item)
//{
//    //if (!CanAdd) { throw new NotSupportedException($"{nameof(CanAdd)}"); }
//    throw new NotSupportedException();
//}

//public bool CanAddNew => CanInstantiate && CanAdd;
//public virtual async Task<TValue> AddNew(Type type, params object[] constructorParameters)
//{
//    if (!CanAddNew) { throw new NotSupportedException($"!{nameof(CanAddNew)}"); }
//    var item = Instantiate(type, constructorParameters);
//    await Add(item);
//    return item;
//}

//// TODO: REVIEW - parallel vs batch vs serial
//public virtual Task AddRange(IEnumerable<TValue> items/*, int batchSize = 10 */) => Task.WhenAll(items.Select(item => Add(item)));

//#endregion

//#region Remove

//public IEnumerable<TValue> Removing => Enumerable.Empty<TValue>();

//public virtual Task<bool> Remove(TValue item) => throw new NotSupportedException();

//    #endregion
//}
