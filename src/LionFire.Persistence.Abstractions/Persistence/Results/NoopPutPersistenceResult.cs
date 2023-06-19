namespace LionFire.Persistence
{
    public struct NoopPutPersistenceResult : ITransferResult
    {
        public TransferResultFlags Flags { get => TransferResultFlags.Noop | TransferResultFlags.Success; set => throw new System.NotImplementedException(); }

        public object Error => "Noop implementation";

        public bool? IsSuccess => false;
        public bool IsNoop => true;

        public static NoopPutPersistenceResult Instance { get; } = new NoopPutPersistenceResult();
    }
}