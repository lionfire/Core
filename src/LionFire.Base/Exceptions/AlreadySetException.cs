using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire;

[Serializable]
public class AlreadySetException : AlreadyException
{
    public const string DefaultMessage = "Property can only be set once";

    public AlreadySetException() : base(DefaultMessage) { }
    public AlreadySetException(string message) : base(message) { }
    public AlreadySetException(string message, Exception inner) : base(message, inner) { }
    protected AlreadySetException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
        : base(info, context) { }
}
