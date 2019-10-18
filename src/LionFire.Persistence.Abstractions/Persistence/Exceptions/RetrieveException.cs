using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence
{

    public class RetrieveException : PersistenceException
    {
        public new IRetrieveResult<object> Result { get; private set; }

        public RetrieveException() { }
        public RetrieveException(string message) : base(message) { }
        public RetrieveException(string message, Exception inner) : base(message, inner) { }
        public RetrieveException(IRetrieveResult<object> result) : base(result) { this.Result = result; }
    }

}
