using System.Collections.Generic;
using LionFire.ObjectBus;

namespace LionFire.Vos
{

    public interface IVBase : IOBase
    {
        // TODO: Move these to IVob and make IVBase inherit IVob

        Vob this[string path] { get; }
        Vob this[VosReference reference] { get; }

#if TOPORT
        //Vob this[params string[] pathChunks] { get; }

        //Vob this[IEnumerable<string> pathChunks] { get; }

        //Vob this[IEnumerator<string> pathChunks] { get; }
#endif
    }
}
