namespace LionFire.Persistence
{
    public struct ExistsResult : ITransferResult
    {
        public object Error { get; set; }

        public TransferResultFlags Flags { get; set; }

        public bool? IsSuccess => Flags.IsSuccessTernary();

        public static readonly TransferResult Success = new TransferResult { Flags = TransferResultFlags.Success };
        public static readonly TransferResult Found = new TransferResult { Flags = TransferResultFlags.Found };
        public static readonly TransferResult NotFound = new TransferResult { Flags = TransferResultFlags.NotFound };
        public static readonly TransferResult PreviewFail = new TransferResult { Flags = TransferResultFlags.PreviewFail };
        public static readonly TransferResult PreviewSuccess = new TransferResult { Flags = TransferResultFlags.PreviewSuccess };
    }

 }

