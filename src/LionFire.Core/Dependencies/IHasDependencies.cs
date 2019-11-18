using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Dependencies
{
    public interface IHasDependencies
    {
        /// <summary>
        /// This is set after a TryConfigure or (more typically) TryInitialize.
        /// </summary>
        UnsatisfiedDependencies UnsatisfiedDependencies { get; }
    }
}
