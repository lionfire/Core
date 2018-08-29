#if UNUSED // HACK used somewhere?
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Handles.Experimental
{

    public interface IReadHandleEx<
#if !UNITY
        out // Crashes unity???
#endif
T> : IReadHandle
        where T : class
    {
        T ObjectField { get; } // REVIEW - this seems to be a hack.  eliminate it?
    }

}

#endif