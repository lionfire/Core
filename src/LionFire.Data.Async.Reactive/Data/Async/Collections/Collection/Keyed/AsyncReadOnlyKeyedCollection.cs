﻿using DynamicData;
using LionFire.Data.Async.Gets;
using LionFire.Ontology;
using LionFire.Structures;
using LionFire.Structures.Keys;
using MorseCode.ITask;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reactive;
using LionFire.Dependencies;
using System.Collections;
using LionFire.ExtensionMethods;

namespace LionFire.Data.Collections;

public abstract class AsyncReadOnlyKeyedCollection<TKey, TValue>
    : AsyncDynamicDataCollection<TValue>
    , IObservableCacheKeyableGetter<TKey, TValue>
    , IInjectable<IKeyProvider<TKey, TValue>>
    , System.IAsyncObserver<ChangeSet<TValue, TKey>>
    where TKey : notnull
    where TValue : notnull
{
    #region Dependencies

    #region Optional

    #region KeyProvider

    public IKeyProvider<TKey, TValue>? KeyProvider { get; set; }
    IKeyProvider<TKey, TValue>? IHas<IKeyProvider<TKey, TValue>>.Object => KeyProvider;
    IKeyProvider<TKey, TValue> IDependsOn<IKeyProvider<TKey, TValue>>.Dependency { set => KeyProvider = value; }

    #endregion

    #endregion

    #endregion

    #region Parameters

    protected static IEqualityComparer<TKey> DefaultKeyEqualityComparer { get; set; } = EqualityComparer<TKey>.Default;

    public Func<TValue, TKey> KeySelector { get; }
    public virtual Func<TValue, TKey> DefaultKeySelector(IServiceProvider? serviceProvider = null) => KeySelectors.GetKeySelector<TValue, TKey>(serviceProvider ?? DependencyContext.Current?.ServiceProvider);

    AsyncObservableCollectionOptions? options; // UNUSED - TODO

    #endregion

    #region Lifecycle

    public AsyncReadOnlyKeyedCollection() : this(null) { }

    public AsyncReadOnlyKeyedCollection(Func<TValue, TKey>? keySelector = null, SourceCache<TValue, TKey>? dictionary = null, AsyncObservableCollectionOptions? options = null)
    {
        KeySelector = keySelector ?? DefaultKeySelector();
        SourceCache = dictionary ?? new SourceCache<TValue, TKey>(KeySelector);
        this.options = options;
    }

    #endregion

    #region State

    public SourceCache<TValue, TKey> SourceCache { get; }
    public IObservableCache<TValue, TKey> ObservableCache => SourceCache.AsObservableCache(); // AsObservableCache converts it read only

    //public override DynamicData.IObservableList<KeyValuePair<TKey, TValue>> List => throw new NotImplementedException();// SourceCache.AsObservableList();

    #endregion

    #region (explicit) IAsyncCollectionCache<TValue>

    // TODO: Several not implemented. Would benefit from being able to clone LazyResolveResult and other ResolveResults with a different type (overriding or transforming value)

    // Allows treating this as a list


    #endregion

    public override void OnNext(IGetResult<IEnumerable<TValue>> result)
    {
        //Debug.WriteLine($"{this.GetType().ToHumanReadableName()} OnNext GetResult: {result}");
        SourceCache?.EditDiff(result.Value ?? Enumerable.Empty<TValue>(), ValueEqualityComparerFunc);
    }

    #region IAsyncReadOnlyCollectionCache<KeyValuePair<TKey, TItem>>

    #region IReadOnlyCollection<TItem>
    #endregion

    //#region  IObservableGets<IEnumerable<TItem>>

    //IObservable<ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>>> IObservableGets<IEnumerable<KeyValuePair<TKey, TValue>>>.AsyncGetsWithEvents => throw new NotImplementedException();

    //#endregion

    //IEnumerable<KeyValuePair<TKey, TValue>>? IReadWrapper<IEnumerable<KeyValuePair<TKey, TValue>>>.Value => throw new NotImplementedException();

    //#region IEnumerable<KeyValuePair<TKey, TValue>>

    //IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    //    => SourceCache.KeyValues.GetEnumerator();

    //#endregion

    #region ILazilyGets<IEnumerable<TItem>>

    public override bool HasValue => LastFullResolveValue != null;

    // OLD
    //ITask<IGetResult<IEnumerable<TValue>>> ILazilyGets<IEnumerable<TValue>>.TryGetValue()
    //{
    //    var result = this.TryGetValue();
    //    throw new NotImplementedException("TODO: Notify / process results");
    //    return result;
    //}

    // OLD
    //IGetResult<IEnumerable<TValue>> ILazilyGets<IEnumerable<TValue>>.QueryValue()
    //{
    //    var result = this.QueryValue();

    //    //if (result.IsSuccess) // THREADUNSAFE
    //    //{
    //    //    return new ResolveResultSuccess<IEnumerable<KeyValuePair<TKey, TValue>>>(SourceCache.KeyValues);
    //    //}
    //    throw new NotImplementedException("TODO: Notify / process results");
    //    return result;
    //}

    public override IEnumerable<TValue>? Value => SourceCache.Items;

    public override void DiscardValue()
    {
        LastFullResolveValue = null;
        SourceCache.Clear();
    }

    #endregion

    #region IGets<IEnumerable<TValue>>

    #region Resolve


    Func<TValue, TValue, bool> ValueEqualityComparerFunc => (l, r) => DefaultKeyEqualityComparer.Equals(KeySelector(l), KeySelector(r));

    //public override IEnumerable<TValue>? ReadCacheValue => SourceCache.Items;
    public override IEnumerable<TValue>? ReadCacheValue { get; }


    //protected abstract ITask<IGetResult<IEnumerable<TValue>>> GetImpl(CancellationToken cancellationToken = default);
    //protected abstract ITask<IGetResult<IEnumerable<TValue>>> GetImpl(CancellationToken cancellationToken = default);
    protected override async ITask<IGetResult<IEnumerable<TValue>>> GetImpl(CancellationToken cancellationToken = default)
    {
        var result = await GetImpl(cancellationToken).ConfigureAwait(false);
        if (result.IsSuccess == true)
        {
            SourceCache.EditDiff(result.Value ?? Enumerable.Empty<TValue>(), ValueEqualityComparerFunc);
            this.LastFullResolveValue = result.Value;
        }
        else
        {
            Debug.WriteLine($"TODO: Handle non-success: " + result);
        }
        return result;
    }

    protected IEnumerable<TValue>? LastFullResolveValue { get; private set; }

    #endregion

    #endregion

    #endregion

    //#region ICollection<int>

    //public override int Count => SourceCache.Count;

    //#endregion

    //#region IEnumerable

    //public override IEnumerator<TValue> GetEnumerator() => SourceCache.Items.GetEnumerator();

    //#endregion

    #region IAsyncObserver

    public ValueTask OnNextAsync(ChangeSet<TValue, TKey> value)
    {
        SourceCache.Edit(u => u.Clone(value));
        return ValueTask.CompletedTask;
    }

    public IObservable<Exception> AsyncObserverErrors => asyncObserverErrors.Value;
    private Lazy<Subject<Exception>> asyncObserverErrors = new();

    public ValueTask OnErrorAsync(Exception error)
    {
        asyncObserverErrors.Value.OnNext(error);
        return ValueTask.CompletedTask;
    }

    public IObservable<Unit> AsyncObserverCompleted => asyncObserverCompleted.Value;
    private Lazy<Subject<Unit>> asyncObserverCompleted = new();

    public ValueTask OnCompletedAsync()
    {
        asyncObserverCompleted.Value.OnNext(Unit.Default);
        return ValueTask.CompletedTask;
    }

    #endregion
}
