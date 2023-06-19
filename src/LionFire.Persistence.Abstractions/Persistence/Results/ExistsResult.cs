namespace LionFire.Persistence
{
    public struct ExistsResult : ITransferResult
    {
        public object Error { get; set; }

        public TransferResultFlags Flags { get; set; }

        public bool? IsSuccess => Flags.IsSuccessTernary();
        public bool IsNoop => Flags.HasFlag(TransferResultFlags.Noop);

        public static readonly PersistenceResult Success = new PersistenceResult { Flags = TransferResultFlags.Success };
        public static readonly PersistenceResult Found = new PersistenceResult { Flags = TransferResultFlags.Found };
        public static readonly PersistenceResult NotFound = new PersistenceResult { Flags = TransferResultFlags.NotFound };
        public static readonly PersistenceResult PreviewFail = new PersistenceResult { Flags = TransferResultFlags.PreviewFail };
        public static readonly PersistenceResult PreviewSuccess = new PersistenceResult { Flags = TransferResultFlags.PreviewSuccess };
    }

 }

