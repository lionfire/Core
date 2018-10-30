using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Structures
{
    /// <summary>
    /// </summary>
    /// <remarks>
    ///  - Used by FileSerializationService
    ///  - (used by Service RPC system)
    ///  MOVE to LionFire.Structures?
    /// </remarks>
    public interface IHasPath 
    {
        string Path { get; }
    }
}
