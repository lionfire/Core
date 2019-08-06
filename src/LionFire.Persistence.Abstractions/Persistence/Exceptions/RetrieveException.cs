using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence
{

    [Serializable]
    public class RetrieveException : PersistenceException
    {
        public IReadResult Result { get; private set; }

        public RetrieveException() { }
        public RetrieveException(string message) : base(message) { }
        public RetrieveException(string message, Exception inner) : base(message, inner) { }
        public RetrieveException(IReadResult result) : base(result.ToString()) { this.Result = result; }
    }

}
