#if false // OLD TRIAGE
using DynamicData;
using LionFire.Data.Collections;
using LionFire.Dependencies;
using System.Collections;

namespace LionFire.Mvvm;



/// <summary>
/// Cache for an underlying collection that is accessed asynchronously
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TCollection">must be a CySharp/ObservableCollections IObservableCollection&lt;TValue&gt; type</typeparam>
public abstract partial class AsyncObservableCollectionCacheBaseBase<TItem> : AsyncDynamicDataCollectionCache<TItem>
    , IAsyncCreates<TItem>
    //where TCollection : class, IObservableCollection<TItem>, new()
{
    #region Dependencies

    #region ServiceProvider (for TItem activation)

    /// <summary>
    /// IServiceProvider used for TValue activation
    /// </summary>
    public virtual IServiceProvider? ServiceProvider => GetServiceProviderForItemActivation(this);
    public static Func<AsyncObservableCollectionCacheBaseBase<TItem, TCollection>, IServiceProvider?> GetServiceProviderForItemActivation = _ => DependencyContext.Current?.ServiceProvider;

    #endregion

    #endregion

    #region Parameters

    public AsyncObservableCollectionOptions Options { get; set; }

    #region CreateableTypes

    public virtual IEnumerable<Type>? CreateableTypes { get => createableTypes ??= DefaultCreateableTypes; set => createableTypes = value; }
    private IEnumerable<Type>? createableTypes;
    public static IEnumerable<Type> DefaultCreateableTypes
    {
        get
        {
            // TODO: use Type introspection service to get list types that derive from TValue
            // alternate: inspect T for attributes
            return Enumerable.Empty<Type>();
        }
    }

    #endregion
    
    #endregion

    #region Lifecycle

    public AsyncObservableCollectionCacheBaseBase() { Options = AsyncObservableCollectionOptions.Default; }
    public AsyncObservableCollectionCacheBaseBase(TCollection? collection = default, AsyncObservableCollectionOptions? options = null, bool blockOnSetCollection = false)
    {
        if (!EqualityComparer<TCollection>.Default.Equals(collection, default)) { var task = SetCollection(collection); if (blockOnSetCollection) task.Wait(); }
        Options = options ?? AsyncObservableCollectionOptions.Default;
    }

    #endregion

    #region State

    object _lock = new();

    #region Collection

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Property type is IObservableCollection&lt;TValue&gt; which provides only read-only access.
    /// </remarks>
    public virtual IObservableCollection<TItem> CollectionCache
    {
        get
        {
            if (collection == null)
            {
                if (Options.AutoInstantiateCollection)
                {
                    lock (_lock)
                    {
                        if (collection == null)
                        {
                            CollectionCache = new TCollection();
                        }
                    }
                }
                else
                {
                    throw new ArgumentNullException(nameof(CollectionCache));
                }
            }
            return collection!;
        }
        set
        {
            if (value != null && value is not TCollection) throw new ArgumentException($"{nameof(value)} must be of type {typeof(TCollection).FullName}");
            SetCollection((TCollection?)value).FireAndForget();
        }
    }
    protected TCollection? collection;

    public async Task SetCollection(TCollection? collection, CancellationToken cancellationToken = default)
    {
        if (ReferenceEquals(this.collection, collection)) { return; }
        lock (_lock)
        {
            if (this.collection != null) { throw new AlreadySetException(); }
            this.collection = collection;
        }
        if (Options.AutoSync)
        {
            await Retrieve(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    #endregion

    #endregion

    // vvv REVIEW vvv

    #region Sync

    public bool Sync
    {
        get => sync;
        set
        {
            if (sync == value) return;
            SetProperty(ref sync, value);
            SetSync(sync).FireAndForget();
        }
    }
    private bool sync;
    public event EventHandler<Exception> SyncError;
    private async Task SetSync(bool sync)
    {
        if (sync)
        {
            try
            {
                var canSubscribe = CanSubscribe;
                bool subscribed = false;
                if (canSubscribe)
                {
                    subscribed = await Subscribe().ConfigureAwait(false);
                }
                // Wait for Subscribe to finish before Retrieving to make sure we don't miss updates
                if ((!canSubscribe || subscribed) && Options.AlwaysRetrieveOnEnableSync && CanRetrieve)
                {
                    await Get().ConfigureAwait(false);
                }
                if (!canSubscribe)
                {
                    Sync = false; // We are not really subscribed.  This is a one-shot retrieve.
                }
            }
            catch (Exception ex)
            {
                SetProperty(ref sync, false);
                SyncError?.Invoke(this, ex);
            }
        }
        else
        {
            if (CanSubscribe)
            {
                await Unsubscribe().ConfigureAwait(false);
            }
        }
    }

    #endregion

    #region Subscription

    /// <summary>
    /// Can call Subscribe() and Unsubscribe().  Do not call these directly; set Sync instead.
    /// </summary>
    public virtual bool CanSubscribe => false;

    SourceList<TItem> sourceList = new();

    //public IDisposable Subscribe(IObserver<IChangeSet<TValue>>)
    //{
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <returns>true if actually subscribed to something during this invocation, false if already subscribed (throw on fail)</returns>
    /// <exception cref="NotSupportedException"></exception>
    protected virtual Task<bool> Subscribe() => throw new NotSupportedException();
    //protected virtual IObservable<ChangeSet<>> SubscribeRx() => throw new NotSupportedException();

    protected virtual Task Unsubscribe() => throw new NotSupportedException();

    #endregion

    #region Collection-related

    public int Count => collection?.Count ?? 0;

    public IEnumerator<TItem> GetEnumerator()
        => collection?.GetEnumerator() ?? Enumerable.Empty<TItem>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public virtual bool IsReadOnly => true;

    #endregion

    #region Retrieve

    public bool HasRetrieved { get => hasRetrieved; protected set => SetProperty(ref hasRetrieved, value); }
    private bool hasRetrieved;

    public bool IsRetrieving { get => isRetrieving; protected set => SetProperty(ref isRetrieving, value); }
    private bool isRetrieving;

    public virtual bool CanRetrieve => false;

    public async Task<IEnumerable<TItem>> Retrieve(bool syncToCollection = true, CancellationToken cancellationToken = default)
    {
        if (!CanRetrieve) { throw new InvalidOperationException($"!{nameof(CanRetrieve)}"); }

        var result = await RetrieveImpl(cancellationToken);

        if (syncToCollection)
        {
            if (CollectionCache == null)
            {
                CollectionCache = new TCollection();
            }
            if (CollectionCache is ObservableHashSet<TItem> h) result.SyncTo(h);
            else result.SyncTo(CollectionCache);
        }

        return result;
    }
    protected virtual Task<IEnumerable<TItem>> RetrieveImpl(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    #endregion

    #region Instantiate

    public virtual bool CanInstantiate => true;
    public virtual TItem Instantiate(Type type, params object[] parameters)
    {
        TItem? newItem = default;
        var serviceProvider = ServiceProvider;
        if (serviceProvider != null)
        {
            newItem = (TItem)ActivatorUtilities.CreateInstance(serviceProvider, type, parameters);
        }

        if (EqualityComparer<TItem>.Default.Equals(newItem, default))
        {
            newItem = (TItem)Activator.CreateInstance(type, parameters)!;
        }
        if (EqualityComparer<TItem>.Default.Equals(newItem, default))
        {
            throw new ArgumentException($"Failed to instantiate type {type.FullName}");
        }
        return newItem!;
    }

    #endregion

    #region Create

    public virtual bool CanCreate => false;
    public virtual Task<TItem> Create(Type type, params object[]? constructorParameters)
    {
        throw new NotSupportedException();
        //if (!CanCreate) { throw new NotSupportedException($"!{nameof(CanCreate)}"); }

        //return (T)(Activator.CreateInstance(type, constructorParameters) ?? throw new Exception("CreateInstance failed for type: " + type.FullName));
    }
    #endregion

    #region Add

    public virtual IEnumerable<TItem> Adding => Enumerable.Empty<TItem>();

    public virtual bool CanAdd => false;
    public virtual Task Add(TItem item)
    {
        //if (!CanAdd) { throw new NotSupportedException($"{nameof(CanAdd)}"); }
        throw new NotSupportedException();
    }

    public bool CanAddNew => CanInstantiate && CanAdd;
    public virtual async Task<TItem> AddNew(Type type, params object[] constructorParameters)
    {
        if (!CanAddNew) { throw new NotSupportedException($"!{nameof(CanAddNew)}"); }
        var item = Instantiate(type, constructorParameters);
        await Add(item);
        return item;
    }

    // TODO: REVIEW - parallel vs batch vs serial
    public virtual Task AddRange(IEnumerable<TItem> items/*, int batchSize = 10 */) => Task.WhenAll(items.Select(item => Add(item)));

    #endregion

    #region Remove

    public IEnumerable<TItem> Removing => Enumerable.Empty<TItem>();

    public virtual Task<bool> Remove(TItem item) => throw new NotSupportedException();

    #endregion
}

#endif