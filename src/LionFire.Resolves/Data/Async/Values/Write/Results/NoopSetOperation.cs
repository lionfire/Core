
namespace LionFire.Data.Async.Sets;

public struct NoopSetOperation<TValue> : ISetOperation<TValue>
{
    public static NoopSetOperation<TValue> Instantiated { get; } = new NoopSetOperation<TValue> { Task = System.Threading.Tasks.Task.FromResult((ISetResult<TValue>)NoopSetResult<TValue>.Instantiated).AsITask() };

    public NoopSetOperation()
    {
    }

    public TValue? DesiredValue { get; set; }

    public ITask<ISetResult<TValue>> Task { get; set; }
}
