#nullable enable

//using System.Reactive.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace LionFire.Data.Async.Gets;

public record PreresolvedGetter<T>(T Value) : IGetter<T>
{
    public bool HasValue => true;

    public void DiscardValue() => throw new NotSupportedException();

    public IGetResult<T> QueryValue() => new NoopGetResult2<T>(Value);
    public ITask<IGetResult<T>> GetIfNeeded() => Task.FromResult<IGetResult<T>>(new NoopGetResult<T>(true, Value)).AsITask();
    ITask<IGetResult<T>> IStatelessGetter<T>.Get(CancellationToken cancellationToken) => Task.FromResult<IGetResult<T>>(new NoopGetResult2<T>(Value)).AsITask();

    public Task<T> Get(CancellationToken cancellationToken = default) => Task.FromResult(Value);

    public void Discard() => DiscardValue();

    public TransferResultFlags Flags { get; set; }

    public object? Error { get; set; }

    T? IGetter<T>.ReadCacheValue => Value;

    public IObservable<IGetResult<T>> GetResults => Observable.Return<IGetResult<T>>(getResult);

    private IGetResult<T> getResult => new GetResult<T>(Value, true) { Flags = TransferResultFlags.Noop | TransferResultFlags.Found };
    public IObservable<ITask<IGetResult<T>>> GetOperations => Observable.Return(Task.FromResult(getResult).AsITask());
}