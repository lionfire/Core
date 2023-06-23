#nullable enable
using LionFire.Data.Async.Gets;
using LionFire.Results;

namespace LionFire.Data;

public interface ITransferResult : ISuccessResult
{
    TransferResultFlags Flags { get; }
    new bool? IsSuccess => Flags.HasFlag(TransferResultFlags.Success);
}

public static class ITransferResultX
{
    public static bool IsNoop(this ITransferResult result) => result.Flags.HasFlag(TransferResultFlags.Noop);
}
