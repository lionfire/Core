using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;

namespace LionFire.Persistence
{
    public class PersistenceResult : ITransferResult
    {
        public PersistenceResult() { }

        public PersistenceResult(TransferResultFlags flags)
        {
            Flags = flags;
            Error = null;
        }

        public object Error { get; set; }
        public bool IsNoop => Flags.HasFlag(TransferResultFlags.Noop);

        public TransferResultFlags Flags { get; set; }

        public bool? IsSuccess => Flags.IsSuccessTernary();

        public static readonly PersistenceResult Indeterminate = new PersistenceResult { Flags = TransferResultFlags.Indeterminate };
        public static readonly PersistenceResult Success = new PersistenceResult { Flags = TransferResultFlags.Success };
        public static readonly PersistenceResult SuccessAndFound = new PersistenceResult { Flags = TransferResultFlags.Success | TransferResultFlags.Found };
        public static readonly PersistenceResult Found = new PersistenceResult { Flags = TransferResultFlags.Found };
        public static readonly PersistenceResult NotFound = new PersistenceResult { Flags = TransferResultFlags.NotFound };
        public static readonly PersistenceResult SuccessNotFound = new PersistenceResult { Flags = TransferResultFlags.Success | TransferResultFlags.NotFound };
        public static readonly PersistenceResult FailAndNotFound = new PersistenceResult { Flags = TransferResultFlags.Fail | TransferResultFlags.NotFound };
        public static readonly PersistenceResult FailAndFound = new PersistenceResult { Flags = TransferResultFlags.Fail | TransferResultFlags.Found };
        public static readonly PersistenceResult PreviewFail = new PersistenceResult { Flags = TransferResultFlags.PreviewFail };
        public static readonly PersistenceResult PreviewSuccess = new PersistenceResult { Flags = TransferResultFlags.PreviewSuccess };

        public override string ToString() => $"{{{this.GetType().Name} {Flags}}}";
    }

    //public struct PersistenceResult<TObject>
    //{
    //    public TObject Object { get; set; }

    //    public bool IsSuccess { get; set; }
    //    public string FailReason { get; set; }
    //    public Exception InnerException { get; set; }

    //}


}


