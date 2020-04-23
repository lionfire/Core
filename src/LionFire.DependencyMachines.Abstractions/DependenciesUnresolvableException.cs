using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.DependencyMachines
{

    [Serializable]
    public class DependenciesUnresolvableException : Exception
    {
        public DependenciesUnresolvableException() { }
        public DependenciesUnresolvableException(string message) : base(message) { }
        public DependenciesUnresolvableException(string message, Exception inner) : base(message, inner) { }
        protected DependenciesUnresolvableException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public IEnumerable<string> UnresolvableDependencies { get; set; }

    }
}
