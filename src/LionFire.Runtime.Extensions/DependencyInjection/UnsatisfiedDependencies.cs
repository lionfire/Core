using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.DependencyInjection
{
    public class UnsatisfiedDependencies : List<UnsatisfiedDependency>
        //, IUnsatisfiedDependencies
    {
        /// <summary>
        /// Set to true on an owner when this set of missing dependencies has changed from the previous missing dependencies state.
        /// </summary>
        public bool MadeProgress { get; set; }

        public static readonly UnsatisfiedDependencies Resolved = new UnsatisfiedDependencies();
    }
}
