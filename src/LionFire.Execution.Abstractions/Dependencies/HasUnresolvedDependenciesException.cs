using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Dependencies
{
    public class HasUnresolvedDependenciesException : Exception
    {
        public HasUnresolvedDependenciesException() { }
        public HasUnresolvedDependenciesException(string message) : base(message) { }
        public HasUnresolvedDependenciesException(string message, Exception inner) : base(message, inner) { }

    }

}
