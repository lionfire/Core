using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Dependencies
{
    public class UnsatisfiedDependencies : List<UnsatisfiedDependency>
    //, IUnsatisfiedDependencies
    {
        public UnsatisfiedDependencies() { }
        public UnsatisfiedDependencies(IEnumerable<UnsatisfiedDependency> items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Set to true on an owner when this set of missing dependencies has changed from the previous missing dependencies state.
        /// </summary>
        public bool MadeProgress { get; set; }

        public static readonly UnsatisfiedDependencies Resolved = new UnsatisfiedDependencies();

        public static implicit operator UnsatisfiedDependencies(string singleItem) => new UnsatisfiedDependencies { singleItem };
        public static implicit operator UnsatisfiedDependencies(string[] items) => new UnsatisfiedDependencies(items.Select(i => (UnsatisfiedDependency)i));
    }
}
