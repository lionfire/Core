namespace LionFire.Data.Sets;

public readonly struct SetOperation<TValue> : ISetOperation<TValue>
{
    public SetOperation(TValue? value, ITask<ITransferResult> task)
    {
        Value = value;
        Task = task;
    }
    public TValue? Value { get; }

    public ITask<ITransferResult> Task { get; }
}

