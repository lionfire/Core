namespace LionFire.Persistence
{
    public struct ExistsResult : IPersistenceResult
    {
        public object Error { get; set; }

        public PersistenceResultFlags Flags { get; set; }

        public bool? IsSuccess => Flags.IsSuccessTernary();
        public bool IsNoop => Flags.HasFlag(PersistenceResultFlags.Noop);

        public static readonly PersistenceResult Success = new PersistenceResult { Flags = PersistenceResultFlags.Success };
        public static readonly PersistenceResult Found = new PersistenceResult { Flags = PersistenceResultFlags.Found };
        public static readonly PersistenceResult NotFound = new PersistenceResult { Flags = PersistenceResultFlags.NotFound };
        public static readonly PersistenceResult PreviewFail = new PersistenceResult { Flags = PersistenceResultFlags.PreviewFail };
        public static readonly PersistenceResult PreviewSuccess = new PersistenceResult { Flags = PersistenceResultFlags.PreviewSuccess };
    }

 }

