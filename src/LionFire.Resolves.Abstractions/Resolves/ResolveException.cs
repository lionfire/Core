using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Gets;

public class ResolveException : Exception
{
	public ResolveException() { }
    public ResolveException(string message) : base(message) { }
    public ResolveException(IGetResult result, string message) : base(message)
    {
        Result = result;
    }
    public ResolveException(string message, Exception inner) : base(message, inner) { }

    public IGetResult? Result { get; }
}
