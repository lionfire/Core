using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ObservableCollections;
using System.Collections;

namespace LionFire.Mvvm;

public abstract partial class AsyncObservableCollectionCacheBase<T, TCollection> : ObservableObject, IAsyncCollectionCache<T>
    where TCollection : IObservableCollection<T>, new()
{
    #region (Static) Defaults

    public static IEnumerable<Type> DefaultCreateableTypes
    {
        get
        {
            // TODO: use Type introspection service to get list types that derive from TItem
            // alternate: inspect T for attributes
            return Enumerable.Empty<Type>();
        }
    }

    #endregion

    #region Parameters

    public AsyncObservableCollectionOptions Options { get; set; }

    public virtual IEnumerable<Type>? CreateableTypes { get => createableTypes ??= DefaultCreateableTypes; set => createableTypes = value; }
    private IEnumerable<Type>? createableTypes;

    public IServiceProvider? ServiceProvider { get; set; }

    #endregion

    #region Lifecycle

    public AsyncObservableCollectionCacheBase() { Options = AsyncObservableCollectionOptions.Default; }
    public AsyncObservableCollectionCacheBase(IObservableCollection<T>? collection = null, AsyncObservableCollectionOptions? options = null)
    {
        if (collection != null) { Collection = collection; }
        Options = options ?? AsyncObservableCollectionOptions.Default;
    }

    #endregion

    #region State

    public virtual IObservableCollection<T> Collection
    {
        get
        {
            if (collection == null)
            {
                if (Options.AutoInstantiateCollection)
                {
                    Collection = new TCollection();
                }
                else
                {
                    throw new ArgumentNullException(nameof(Collection));
                }
            }
            return collection!;
        }
        set
        {
            if (ReferenceEquals(value, collection)) { return; }
            if (collection != null) { throw new AlreadySetException(); }
            collection = value is TCollection collectionValue
                ? collectionValue
                : throw new ArgumentException($"{nameof(value)} must be of type {typeof(TCollection).FullName}");

            if (Options.AutoSync)
            {

            }
        }
    }
    protected TCollection? collection;

    #endregion

    #region Sync

    public bool Sync
    {
        get => sync;
        set
        {
            // Intentionally not here: if (sync == value) return;
            sync = value;
            if (sync)
            {
                Task.Run(async () =>
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
                            await Retrieve().ConfigureAwait(false);
                        }
                        if (!canSubscribe)
                        {
                            Sync = false; // We are not really subscribed.  This is a one-shot retrieve.
                        }
                    }
                    catch
                    {
                        sync = false;
                        throw;
                    }
                });
            }
            else
            {
                if (CanSubscribe)
                {
                    Unsubscribe().FireAndForget();
                }
            }
            SetProperty(ref sync, value);
        }
    }
    private bool sync;

    #endregion

    #region Subscription

    /// <summary>
    /// Can call Subscribe() and Unsubscribe().  Do not call these directly; set Sync instead.
    /// </summary>
    public virtual bool CanSubscribe => false;

    /// <summary>
    /// 
    /// </summary>
    /// <returns>true if actually subscribed to something during this invocation, false if already subscribed (throw on fail)</returns>
    /// <exception cref="NotSupportedException"></exception>
    protected virtual Task<bool> Subscribe() => throw new NotSupportedException();
    protected virtual Task Unsubscribe() => throw new NotSupportedException();

    #endregion

    #region Collection-related

    public int Count => collection?.Count ?? 0;

    public IEnumerator<T> GetEnumerator()
        => collection?.GetEnumerator() ?? Enumerable.Empty<T>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public virtual bool IsReadOnly => true;

    #endregion

    #region Retrieve

    public bool HasRetrieved { get => hasRetrieved; protected set => SetProperty(ref hasRetrieved, value); }
    private bool hasRetrieved;

    public bool IsRetrieving { get => isRetrieving; protected set => SetProperty(ref isRetrieving, value); }
    private bool isRetrieving;

    public virtual bool CanRetrieve => false;

    public async Task<IEnumerable<T>> Retrieve(bool syncToCollection = true, CancellationToken cancellationToken = default)
    {
        if (!CanRetrieve) { throw new InvalidOperationException($"!{nameof(CanRetrieve)}"); }

        var result = await RetrieveImpl(cancellationToken);

        if (syncToCollection)
        {
            if (Collection == null)
            {
                Collection = new TCollection();
            }
            if (Collection is ObservableHashSet<T> h) result.SyncTo(h);
            else result.SyncTo(Collection);
        }

        return result;
    }
    protected virtual Task<IEnumerable<T>> RetrieveImpl(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    #endregion

    #region Instantiate

    public virtual bool CanInstantiate => true;
    public virtual T Instantiate(Type type, params object[] parameters)
    {
        T? newItem = default;
        if (ServiceProvider != null)
        {
            newItem = (T)ActivatorUtilities.CreateInstance(ServiceProvider, type, parameters);
        }

        if (EqualityComparer<T>.Default.Equals(newItem, default))
        {
            newItem = (T)Activator.CreateInstance(type, parameters)!;
        }
        if (EqualityComparer<T>.Default.Equals(newItem, default))
        {
            throw new ArgumentException($"Failed to instantiate type {type.FullName}");
        }
        return newItem!;
    }

    #endregion

    #region Create

    public virtual bool CanCreate => false;
    public virtual Task<T> Create(Type type, params object[]? constructorParameters)
    {
        throw new NotSupportedException();
        //if (!CanCreate) { throw new NotSupportedException($"!{nameof(CanCreate)}"); }

        //return (T)(Activator.CreateInstance(type, constructorParameters) ?? throw new Exception("CreateInstance failed for type: " + type.FullName));
    }
    #endregion

    #region Add

    public virtual IEnumerable<T> Adding => Enumerable.Empty<T>();

    public virtual bool CanAdd => false;
    public virtual Task Add(T item)
    {
        //if (!CanAdd) { throw new NotSupportedException($"{nameof(CanAdd)}"); }
        throw new NotSupportedException();
    }

    public bool CanAddNew => CanInstantiate && CanAdd;
    public virtual async Task<T> AddNew(Type type, params object[] constructorParameters)
    {
        if (!CanAddNew) { throw new NotSupportedException($"!{nameof(CanAddNew)}"); }
        var item = Instantiate(type, constructorParameters);
        await Add(item);
        return item;
    }

    // TODO: REVIEW - parallel vs batch vs serial
    public virtual Task AddRange(IEnumerable<T> items/*, int batchSize = 10 */) => Task.WhenAll(items.Select(item => Add(item)));

    #endregion

    #region Remove

    public IEnumerable<T> Removing => Enumerable.Empty<T>();

    public virtual Task<bool> Remove(T item) => throw new NotSupportedException();

    #endregion
}

public static class ListExtensions
{
    public static void SyncTo<T>(this IEnumerable<T> source, IObservableCollection<T> destination)
    {
        ArgumentNullException.ThrowIfNull(destination);

        ICollection<T>? collection = destination as ICollection<T>;
        if (collection != null)
        {
            foreach (var removal in collection.Where(i => !source.Contains(i)).ToArray())
            {
                collection.Remove(removal);
            }
        }
        else
        {
            throw new NotSupportedException($"{destination.GetType().FullName}");
        }

        if (destination is ObservableList<T> list)
        {
            list.AddRange(source.Where(i => !collection.Contains(i)).ToArray());
        }
        else
        {
            foreach (var addition in source.Where(i => !collection.Contains(i)))
            {
                collection.Add(addition);
            }
        }
    }

    public static void SyncTo<T>(this IEnumerable<T> source, ObservableHashSet<T> destination)
        where T : notnull
    {
        foreach (var removal in destination.Where(i => !source.Contains(i)).ToArray())
        {
            destination.Remove(removal);
        }
        destination.AddRange(source.Where(i => !destination.Contains(i)));
    }
}