using LionFire.Data;
using System;
using System.Runtime.Serialization;

namespace LionFire.Data;

public class TransferException : Exception
{
    public TransferException() { }
    public TransferException(string message) : base(message) { }
    public TransferException(string message, Exception inner) : base(message, inner) { }

    public TransferException(ITransferResult result, string message = null) : base(message ?? $"Persistence operation failed.  See Result for details.")
    {
        this.Result = result;
    }

    protected TransferException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ITransferResult Result { get; private set; }

}
