using DynamicData;
using LionFire.Data.Async.Gets;
using LionFire.Ontology;
using LionFire.Structures;
using LionFire.Structures.Keys;
using MorseCode.ITask;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reactive;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Dependencies;

namespace LionFire.Data.Collections;

public abstract class AsyncReadOnlyDictionary<TKey, TValue>
    : AsyncLazyDynamicDataCollection<KeyValuePair<TKey, TValue>>
    , IAsyncReadOnlyDictionaryCache<TKey, TValue>
    , IInjectable<IKeyProvider<TKey, TValue>>
    , System.IAsyncObserver<ChangeSet<TValue, TKey>>
    where TKey : notnull
{
    #region Dependencies

    #region Optional

    #region KeyProvider

    public IKeyProvider<TKey, TValue>? KeyProvider { get; set; }

    public IKeyProvider<TKey, TValue>? Object => KeyProvider;

    public IKeyProvider<TKey, TValue> Dependency { set => KeyProvider = value; }

    #endregion

    #endregion

    #endregion

    #region Parameters

    // TODO: Eliminate KeySelector for this class, since keys tend to come from user, not from TValue.  If keys come from TValue, use AsyncReadOnlyKeyedCollectionCache instead.
    protected Func<TValue, TKey> KeySelector { get; }
    public virtual Func<TValue, TKey> DefaultKeySelector() => KeySelectors.GetKeySelector<TValue,TKey>(DependencyContext.Current?.ServiceProvider);

    #endregion

    #region Lifecycle

    public AsyncReadOnlyDictionary() : this(null) { }

    public AsyncReadOnlyDictionary(Func<TValue, TKey>? keySelector, SourceCache<TValue, TKey>? dictionary = null, AsyncObservableCollectionOptions? options = null)
    {
        KeySelector = keySelector ?? DefaultKeySelector();
        SourceCache = dictionary ?? new SourceCache<TValue, TKey>(KeySelector);
    }

    #endregion

    #region State

    public SourceCache<TValue, TKey> SourceCache { get; }
    public IObservableCache<TValue, TKey> ObservableCache => SourceCache.AsObservableCache(); // Converts to read only
    //public override DynamicData.IObservableList<KeyValuePair<TKey, TValue>> List => throw new NotImplementedException();// SourceCache.AsObservableList();

    #endregion

    #region (explicit) IAsyncCollectionCache<TValue>

    // TODO: Several not implemented. Would benefit from being able to clone LazyResolveResult and other ResolveResults with a different type (overriding or transforming value)

    // Allows treating this as a list

    #endregion

    #region IAsyncReadOnlyCollectionCache<KeyValuePair<TKey, TItem>>

    #region IReadOnlyCollection<TItem>
    #endregion

    #region IObservableGets<IEnumerable<TItem>>


    IObservable<ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>>> IObservableGetOperations<IEnumerable<KeyValuePair<TKey, TValue>>>.GetOperations => throw new NotImplementedException();

    //IObservable<ITask<IGetResult<IEnumerable<TValue>>>> IObservableResolving<TValue>.Resolving => throw new NotImplementedException();

    #endregion

    IEnumerable<KeyValuePair<TKey, TValue>>? IReadWrapper<IEnumerable<KeyValuePair<TKey, TValue>>>.Value => throw new NotImplementedException();

    //#region IEnumerable<KeyValuePair<TKey, TValue>>

    //IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        //=> SourceCache.KeyValues.GetEnumerator();

    //#endregion

    #region ILazilyGets<IEnumerable<TItem>>

    async ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>> IGetter<IEnumerable<KeyValuePair<TKey, TValue>>>.GetIfNeeded()
    {
        var result = await this.GetIfNeeded().ConfigureAwait(false);
        throw new NotImplementedException();
    }

    IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>> IGetter<IEnumerable<KeyValuePair<TKey, TValue>>>.QueryValue()
    {
        var result = this.QueryValue();

        //if (result.IsSuccess) // THREADUNSAFE
        //{
        //    return new ResolveResultSuccess<IEnumerable<KeyValuePair<TKey, TValue>>>(SourceCache.KeyValues);
        //}
        throw new NotImplementedException();
    }

    #endregion

    #region IGets<IEnumerable<KeyValuePair<TKey, TValue>>>

    async ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TValue>>>> IStatelessGetter<IEnumerable<KeyValuePair<TKey, TValue>>>.Get(CancellationToken cancellationToken)
    {
        await this.Get(cancellationToken).ConfigureAwait(false);
        throw new NotImplementedException();
    }

    #endregion

    #endregion

    #region IAsyncObserver

    public ValueTask OnNextAsync(ChangeSet<TValue,TKey> value)
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

    // REVIEW
    public override IEnumerable<KeyValuePair<TKey, TValue>>? Value => this.SourceCache.KeyValues;

}

