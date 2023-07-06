
namespace LionFire.Data.Sets;

public interface ISetOperation<out TValue>
{
    TValue? DesiredValue { get; }
    ITask<ITransferResult> Task { get; }
}

// ENH - maybe:
//public interface ISetOperationEx<out TValue> : ISetOperation<TValue>
//{
//    object ETag { get; }
//    TValue? SetFromValue { get; }
//}