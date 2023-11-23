using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;
using LionFire.Results;

namespace LionFire.Data;

public class TransferResult : ITransferResult, IErrorResult
{
    public TransferResult() { }

    public TransferResult(TransferResultFlags flags)
    {
        Flags = flags;
        Error = null;
    }

    public object? Error { get; set; }
    //public bool IsNoop => Flags.HasFlag(TransferResultFlags.Noop);

    public TransferResultFlags Flags { get; set; }

    public bool? IsSuccess => Flags.IsSuccessTernary();

    public static readonly TransferResult Initialized = new TransferResult { Flags = TransferResultFlags.Indeterminate | TransferResultFlags.Noop }; // REVIEW - what flag to indicate nothing happened yet?

    public static readonly TransferResult Indeterminate = new TransferResult { Flags = TransferResultFlags.Indeterminate };
    public static readonly TransferResult Success = new TransferResult { Flags = TransferResultFlags.Success };
    public static readonly TransferResult NoopSuccess = new TransferResult { Flags = TransferResultFlags.Success | TransferResultFlags.Noop };
    public static readonly TransferResult SuccessAndFound = new TransferResult { Flags = TransferResultFlags.Success | TransferResultFlags.Found };
    public static readonly TransferResult Found = new TransferResult { Flags = TransferResultFlags.Found };
    public static readonly TransferResult NotFound = new TransferResult { Flags = TransferResultFlags.NotFound };
    public static readonly TransferResult SuccessNotFound = new TransferResult { Flags = TransferResultFlags.Success | TransferResultFlags.NotFound };
    public static readonly TransferResult FailAndNotFound = new TransferResult { Flags = TransferResultFlags.Fail | TransferResultFlags.NotFound };
    public static readonly TransferResult FailAndFound = new TransferResult { Flags = TransferResultFlags.Fail | TransferResultFlags.Found };
    public static readonly TransferResult PreviewFail = new TransferResult { Flags = TransferResultFlags.PreviewFail };
    public static readonly TransferResult PreviewSuccess = new TransferResult { Flags = TransferResultFlags.PreviewSuccess };

    public static TransferResult FromException(Exception ex) => new TransferResult { Flags = TransferResultFlags.Fail, Error = ex };

    public override string ToString() => $"{{{this.GetType().Name} {Flags}}}";
}

//public struct TransferResult<TObject>
//{
//    public TObject Object { get; set; }

//    public bool IsSuccess { get; set; }
//    public string FailReason { get; set; }
//    public Exception InnerException { get; set; }

//}


