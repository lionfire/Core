namespace LionFire.Data.Sets;

public readonly struct SetOperation<TValue> : ISetOperation<TValue>
{
    public SetOperation(TValue? value, ITask<ITransferResult> task)
    {
        DesiredValue = value;
        Task = task;
    }
    public TValue? DesiredValue { get; }

    public ITask<ITransferResult> Task { get; }
}

