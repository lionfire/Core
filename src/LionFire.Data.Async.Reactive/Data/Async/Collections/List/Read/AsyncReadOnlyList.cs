using DynamicData;
using System.Reactive.Subjects;
using System.Reactive;
using LionFire.Data.Async;

namespace LionFire.Data.Collections;

public abstract class AsyncReadOnlyList<TValue>
    : AsyncDynamicDataCollection<TValue> // REVIEW - was AsyncLazyDynamicDataCollection
    , System.IAsyncObserver<ChangeSet<TValue>> // OPTIMIZE: Separate this out  - REVIEW: is this the most conformant to DynamicData? Usually the receiver wants IObservable instead of IAsyncObserver, though this can provide backpressure
    where TValue : notnull
{

    #region Lifecycle

    public AsyncReadOnlyList() : this(null) { }
    public AsyncReadOnlyList(SourceList<TValue>? sourceList, Action<AsyncReadOnlyList<TValue>>? initializeHook = null) : base(false)
    {
        this.SourceList = sourceList ?? new SourceList<TValue>();
        initializeHook?.Invoke(this);
        InitializeGetOperations();
    }

    #endregion

    #region State

    protected SourceList<TValue> SourceList { get; }
    public IObservableList<TValue> ObservableList => SourceList.AsObservableList();
    public IObservableCache<TValue, string> ObservableCache => null;// SourceList.AsObservableList();

    public override IEnumerable<TValue>? Value => SourceList.Items;

    public TriggerMode DesiredGetMode { get; set; }
    public TriggerMode CurrentGetMode { get; protected set; }

    #endregion

    #region IAsyncObserver

    public ValueTask OnNextAsync(ChangeSet<TValue> value)
    {
        SourceList.Edit(u => u.Clone(value));
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
