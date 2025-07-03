using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire;

public class NotInitializedException : Exception
{
    public NotInitializedException() { }
    public NotInitializedException(string message) : base(message) { }
    public NotInitializedException(string message, Exception inner) : base(message, inner) { }
}
