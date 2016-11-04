using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public interface IInitializable
    {
        /// <summary>
        /// Attempt to initialize, returning true on success, false if initialization can be attempted again.
        /// </summary>
        /// <returns>True if successful, false if not, such as the case when dependencies are not available yet.  See IHasDependencies to indicate missing dependencies.</returns>
        Task<bool> Initialize();
    }
}
