using LionFire.Data;
using System;
using System.Runtime.Serialization;

namespace LionFire.Data;

public class TransferException : Exception
{
    public TransferException() { }
    public TransferException(string message) : base(message) { }
    public TransferException(string message, Exception inner) : base(message, inner) { }

    public TransferException(ITransferResult result, string? message = null) : base(message ?? $"Persistence operation failed.  See Result for details.")
    {
        this.Result = result;
    }

    //[Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051")]
    //protected TransferException(SerializationInfo info, StreamingContext context) : base(info, context)
    //{
    //    Result = default!; // Will be set by serialization
    //}

    public ITransferResult? Result { get; private set; } = default!;

}
