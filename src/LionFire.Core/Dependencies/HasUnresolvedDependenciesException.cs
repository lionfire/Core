using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Dependencies
{

    // TODO: Where is this used?  What does GetRequiredService throw on missing dependencies?  Maybe I should use that where I am also doing the same sort of thing.

    public class HasUnresolvedDependenciesException : Exception
    {
        public HasUnresolvedDependenciesException() { }
        public HasUnresolvedDependenciesException(Type type) : this($"Missing instance of type '{type.FullName}'") { }
        public HasUnresolvedDependenciesException(object obj, UnsatisfiedDependencies unresolvedDependencies)
        {
            var msg = " Missing dependencies: ";

            // REFACTOR with other ctor, use StringBuilder
            if (unresolvedDependencies.Count == 0) return;
            msg += $"Object of type {obj.GetType().Name} needs: ";
            bool isFirst = true;
            foreach (var d in unresolvedDependencies)
            {
                if (isFirst) isFirst = false; else msg += ", ";
                msg += d.Description;
            }

            this.Detail = msg;
        }
        public HasUnresolvedDependenciesException(Dictionary<object, UnsatisfiedDependencies> unresolvedDependencies)
        {
            var msg = " Missing dependencies: ";
            foreach (var kvp in unresolvedDependencies)
            {
                if (kvp.Value.Count == 0) continue;
                msg += $"Object of type {kvp.Key.GetType().Name} needs: ";
                bool isFirst = true;
                foreach (var d in kvp.Value)
                {
                    if (isFirst) isFirst = false; else msg += ", ";
                    msg += d.Description;
                }
            }
            this.Detail = msg;
        }

        public HasUnresolvedDependenciesException(string message) : base(message) { }
        public HasUnresolvedDependenciesException(string message, Exception inner) : base(message, inner) { }

        public string Detail { get; set; }

        public override string ToString()
        {

            return Detail ?? base.ToString();
        }
    }

}
