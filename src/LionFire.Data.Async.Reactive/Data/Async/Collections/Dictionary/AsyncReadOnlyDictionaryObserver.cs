using DynamicData;
using System.Reactive.Subjects;
using System.Reactive;

namespace LionFire.Data.Collections;

public abstract class AsyncReadOnlyDictionaryObserver<TKey, TValue>
    : AsyncReadOnlyDictionary<TKey, TValue>
    , System.IAsyncObserver<ChangeSet<TValue, TKey>>
    where TKey : notnull
{
    #region Lifecycle

    public AsyncReadOnlyDictionaryObserver() : this(null) { }

    public AsyncReadOnlyDictionaryObserver(Func<TValue, TKey>? keySelector, SourceCache<TValue, TKey>? dictionary = null, AsyncObservableCollectionOptions? options = null) : base(keySelector, dictionary, options)
    {
    }

    #endregion

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