using DynamicData;
using LionFire.Resolves;
using LionFire.Ontology;
using LionFire.Structures;
using LionFire.Structures.Keys;
using MorseCode.ITask;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reactive;

namespace LionFire.Collections.Async;

public abstract class AsyncReadOnlyDictionaryCache<TKey, TValue>
    : AsyncDynamicDataCollectionCache<KeyValuePair<TKey, TValue>>
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

    protected Func<TValue, TKey> KeySelector { get; }
    public virtual Func<TValue, TKey> DefaultKeySelector => v => AmbientKeyProviderX.GetKey<TKey, TValue>(v);

    #endregion

    #region Lifecycle

    public AsyncReadOnlyDictionaryCache() : this(null) { }

    public AsyncReadOnlyDictionaryCache(Func<TValue, TKey>? keySelector, SourceCache<TValue, TKey>? dictionary = null, AsyncObservableCollectionOptions? options = null)
    {
        KeySelector = keySelector ?? DefaultKeySelector;
        SourceCache = dictionary ?? new SourceCache<TValue, TKey>(KeySelector);
    }

    #endregion

    #region State

    public SourceCache<TValue, TKey> SourceCache { get; }
    public IObservableCache<TValue, TKey> Cache => SourceCache.AsObservableCache();
    public override DynamicData.IObservableList<KeyValuePair<TKey, TValue>> List => throw new NotImplementedException();// SourceCache.AsObservableList();

    #endregion



    #region (explicit) IAsyncCollectionCache<TValue>

    // TODO: Several not implemented. Would benefit from being able to clone LazyResolveResult and other ResolveResults with a different type (overriding or transforming value)

    // Allows treating this as a list


    #endregion

    #region IAsyncReadOnlyCollectionCache<KeyValuePair<TKey, TItem>>

    #region IReadOnlyCollection<TItem>
    #endregion

    #region  IObservableResolves<IEnumerable<TItem>>


    IObservable<ITask<IResolveResult<IEnumerable<KeyValuePair<TKey, TValue>>>>> IObservableResolves<IEnumerable<KeyValuePair<TKey, TValue>>>.Resolves => throw new NotImplementedException();

    #endregion

    IEnumerable<KeyValuePair<TKey, TValue>>? IReadWrapper<IEnumerable<KeyValuePair<TKey, TValue>>>.Value => throw new NotImplementedException();

    #region IEnumerable<KeyValuePair<TKey, TValue>>

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        => SourceCache.KeyValues.GetEnumerator();

    #endregion

    #region ILazilyResolves<IEnumerable<TItem>>

    async ITask<ILazyResolveResult<IEnumerable<KeyValuePair<TKey, TValue>>>> ILazilyResolves<IEnumerable<KeyValuePair<TKey, TValue>>>.TryGetValue()
    {
        var result = await this.TryGetValue().ConfigureAwait(false);
        throw new NotImplementedException();
    }

    ILazyResolveResult<IEnumerable<KeyValuePair<TKey, TValue>>> ILazilyResolves<IEnumerable<KeyValuePair<TKey, TValue>>>.QueryValue()
    {
        var result = this.QueryValue();

        //if (result.IsSuccess) // THREADUNSAFE
        //{
        //    return new ResolveResultSuccess<IEnumerable<KeyValuePair<TKey, TValue>>>(SourceCache.KeyValues);
        //}
        throw new NotImplementedException();
    }

    #endregion

    #region IResolves<IEnumerable<KeyValuePair<TKey, TValue>>>

    async ITask<IResolveResult<IEnumerable<KeyValuePair<TKey, TValue>>>> IResolves<IEnumerable<KeyValuePair<TKey, TValue>>>.Resolve()
    {
        await this.Resolve().ConfigureAwait(false);
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
}
