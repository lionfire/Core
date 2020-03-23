using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;

namespace LionFire.Persistence
{
    public class PersistenceResult : IPersistenceResult
    {
        public PersistenceResult() { }

        public PersistenceResult(PersistenceResultFlags flags)
        {
            Flags = flags;
            Error = null;
        }

        public object Error { get; set; }
        public bool IsNoop => Flags.HasFlag(PersistenceResultFlags.Noop);

        public PersistenceResultFlags Flags { get; set; }

        public bool? IsSuccess => Flags.IsSuccessTernary();

        public static readonly PersistenceResult Indeterminate = new PersistenceResult { Flags = PersistenceResultFlags.Indeterminate };
        public static readonly PersistenceResult Success = new PersistenceResult { Flags = PersistenceResultFlags.Success };
        public static readonly PersistenceResult SuccessAndFound = new PersistenceResult { Flags = PersistenceResultFlags.Success | PersistenceResultFlags.Found };
        public static readonly PersistenceResult Found = new PersistenceResult { Flags = PersistenceResultFlags.Found };
        public static readonly PersistenceResult NotFound = new PersistenceResult { Flags = PersistenceResultFlags.NotFound };
        public static readonly PersistenceResult SuccessNotFound = new PersistenceResult { Flags = PersistenceResultFlags.Success | PersistenceResultFlags.NotFound };
        public static readonly PersistenceResult FailAndNotFound = new PersistenceResult { Flags = PersistenceResultFlags.Fail | PersistenceResultFlags.NotFound };
        public static readonly PersistenceResult PreviewFail = new PersistenceResult { Flags = PersistenceResultFlags.PreviewFail };
        public static readonly PersistenceResult PreviewSuccess = new PersistenceResult { Flags = PersistenceResultFlags.PreviewSuccess };
    }

    //public struct PersistenceResult<TObject>
    //{
    //    public TObject Object { get; set; }

    //    public bool IsSuccess { get; set; }
    //    public string FailReason { get; set; }
    //    public Exception InnerException { get; set; }

    //}


}


