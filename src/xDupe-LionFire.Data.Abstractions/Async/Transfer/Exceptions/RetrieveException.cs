using LionFire.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Data;


public class RetrieveException : TransferException
{
    public RetrieveException() { }
    public RetrieveException(ITransferResult result) : base(result) { }
    public RetrieveException(string message) : base(message) { }
    public RetrieveException(string message, Exception inner) : base(message, inner) { }
    //public RetrieveException(IRetrieveResult<object> result) : base(result) { this.Result = result; }
}
