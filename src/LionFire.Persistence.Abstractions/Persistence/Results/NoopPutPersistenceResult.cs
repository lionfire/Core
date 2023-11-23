using LionFire.Data;

namespace LionFire.Persistence;

public struct NoopPutPersistenceResult : ITransferResult
{
    public static NoopPutPersistenceResult Instance { get; } = new NoopPutPersistenceResult();

    public bool? IsSuccess => true;

    public TransferResultFlags Flags { get => TransferResultFlags.Noop | TransferResultFlags.Success; }

}