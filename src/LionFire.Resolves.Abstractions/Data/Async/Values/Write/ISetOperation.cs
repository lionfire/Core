
namespace LionFire.Data.Sets;

public interface ISetOperation<out TValue>
{
    TValue? Value { get; }
    ITask<ITransferResult> Task { get; }
}

