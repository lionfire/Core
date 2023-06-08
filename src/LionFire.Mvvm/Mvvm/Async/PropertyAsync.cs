#nullable enable

using LionFire.Resolves;
using MorseCode.ITask;
using Newtonsoft.Json.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;

namespace LionFire.Mvvm.Async;

public class AsyncValue<TValue> : ReactiveObject
    , ILazilyResolves<TValue>
    , IObservableResolves<TValue>
{
}

public class PropertyAsync<TObject, TValue> : AsyncValue<TValue>
{
    // TODO: Wire up change notifications from source

    #region Relationships

    public TObject Target { get; }

    public IAsyncObject? TargetAsync => Target as IAsyncObject;

    #endregion

    #region Parameters

    public AsyncValueOptions Options { get; }
    public static AsyncValueOptions DefaultOptions = new();
    public IEqualityComparer<TValue> EqualityComparer { get; set; } = EqualityComparer<TValue>.Default;

    #endregion

    #region Lifecycle

    public PropertyAsync(TObject target, AsyncValueOptions? options = null)
    {
        Target = target;
        Options = options ?? TargetAsync?.Options.PropertyOptions ?? DefaultOptions;
    }

    #endregion

    #region IResolves

    public IObservable<ITask<IResolveResult<TValue>>> Resolves => resolves;
    private BehaviorSubject<ITask<IResolveResult<TValue>>> resolves = new(Task.FromResult<IResolveResult<TValue>>(ResolveResultNotResolvedNoop<TValue>.Instance).AsITask());

    public bool IsResolving => !resolves.Value.AsTask().IsCompleted;

    #endregion

    #region ILazilyResolves

    #region Value

    public TValue? Value
    {
        get
        {
            if (!HasValue)
            {
                if (Options.GetOnDemand)
                {
                    if (Options.BlockToGet)
                    {
                        return Get().Result;
                    }
                    else
                    {
                        Get().FireAndForget();
                    }
                }
                else if (Options.ThrowOnGetValueIfNotResolved) { DoThrowOnGetValueIfNotLoaded(); }
            }
            return cachedValue;
        }
        set
        {
            //cachedValue = value;
            this.RaiseAndSetIfChanged(ref cachedValue, value);
        }
    }
    private TValue? cachedValue;

    #endregion

    #region HasValue

    public bool HasValue => hasValue.Value;
    private BehaviorSubject<bool> hasValue = new(false);
    //private IObservable<bool> hasValue { get; } = this.Resolves.Select(r => r.HasValue); // TODO: Derive this from Resolves

    #endregion

    public void DiscardValue()
    {
        Value = default;
        hasValue.OnNext(false);
    }

    public async ITask<ILazyResolveResult<TValue>> TryGetValue() // RENAME ResolveIfNeeded
    {
        if (HasValue) return new LazyResolveNoopResult<TValue>(HasValue, Value);
        var value = await this.Get().ConfigureAwait(false);
        return new LazyResolveResult<TValue>(true, value);
    }

    public ILazyResolveResult<TValue> QueryValue() => new LazyResolveNoopResult<TValue>(HasValue, Value);

    public ITask<IResolveResult<TValue>> Resolve()
    {
        lock (resolvingLock)
        {
            var task = resolves.Value;
            if (!task.AsTask().IsCompleted) { return task; }

            task = Task.Run<IResolveResult<TValue>>(async () =>
            {
                var task = Getter(Target, default);
                //gets.OnNext(task);
                var value = await task.ConfigureAwait(false);
                Value = value;
                hasValue.OnNext(true);
                return new ResolveResultSuccess<TValue>(value);
            }).AsITask();
            resolves.OnNext(task);
            return task;
        }
    }

    #endregion

    private void DoThrowOnGetValueIfNotLoaded() => throw new Exception("Value has not been gotten yet.  Invoke Get first or disable Options.ThrowOnGetValueIfNotLoaded");

    #region Get

    //public IObservable<Task<TValue>> Gets => gets;
    //private Subject<Task<TValue>> gets = new();

    public Func<TObject, CancellationToken, Task<TValue>> Getter { get; set; }

    private object resolvingLock = new();
    public virtual async Task<TValue?> Get(CancellationToken cancellationToken = default)
    {
        var result = await Resolve().ConfigureAwait(false);
        return result.Value;
    }

    #endregion



}

