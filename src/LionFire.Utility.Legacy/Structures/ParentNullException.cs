using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire;

[Serializable]
public class ParentNullException : Exception
{
    public ParentNullException() { }
    public ParentNullException(string message) : base(message) { }
    public ParentNullException(string message, Exception inner) : base(message, inner) { }
    //protected ParentNullException(
    //  System.Runtime.Serialization.SerializationInfo info,
    //  System.Runtime.Serialization.StreamingContext context)
    //    : base(info, context) { }
}
