namespace LionFire.Data.Async.Sets;

public readonly struct SetOperation<TValue> : ISetOperation<TValue>
{
    public SetOperation(TValue? value, Task<ISetResult<TValue>> task) : this(value, task.AsITask()) { }

    public SetOperation(TValue? value, ITask<ISetResult<TValue>> task)
    {
        DesiredValue = value;
        Task = task;
    }
    public TValue? DesiredValue { get; }

    public ITask<ISetResult<TValue>> Task { get; }
}

