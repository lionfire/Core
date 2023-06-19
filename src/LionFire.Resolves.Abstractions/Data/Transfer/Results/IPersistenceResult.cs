using LionFire.Results;

namespace LionFire.Data;

public interface ITransferResult : IErrorResult, ISuccessResult, INoopResult
{
    TransferResultFlags Flags { get; set; }
}