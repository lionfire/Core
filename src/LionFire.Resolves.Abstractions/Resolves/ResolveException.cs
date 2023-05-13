using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Resolves;

public class ResolveException : Exception
{
	public ResolveException() { }
    public ResolveException(string message) : base(message) { }
    public ResolveException(IResolveResult result, string message) : base(message)
    {
        Result = result;
    }
    public ResolveException(string message, Exception inner) : base(message, inner) { }

    public IResolveResult Result { get; }
}
