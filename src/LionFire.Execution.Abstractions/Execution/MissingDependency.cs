using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public class MissingDependency
    {
        /// <summary>
        /// Optional
        /// </summary>
        public int Id { get; set; }

        public string Description { get; set; }
    }
    
    public class MissingDependencyList : List<MissingDependency>, IMissingDependencyList
    {
        /// <summary>
        /// Set to true on an owner when this set of missing dependencies has changed from the previous missing dependencies state.
        /// </summary>
        public bool MadeProgress { get; set; }
    }
}
