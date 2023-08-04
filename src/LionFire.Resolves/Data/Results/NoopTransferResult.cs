
namespace LionFire.Data.Async;

public struct NoopTransferResult : ITransferResult
{
    #region (static)

    public static NoopTransferResult Instantiated = new NoopTransferResult { Flags = TransferResultFlags.Instantiated | TransferResultFlags.Noop };

    #endregion

    public NoopTransferResult()
    {
    }

    public TransferResultFlags Flags { get; set; } = TransferResultFlags.Noop;

    public bool? IsSuccess { get; set; } = null;
}

