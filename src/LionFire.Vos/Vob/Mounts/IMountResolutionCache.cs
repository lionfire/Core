using LionFire.Referencing;
using System.Collections.Generic;

namespace LionFire.Vos.Mounts
{
    public interface IMountResolutionCache
    {
        Vob Vob { get; }
        IEnumerable<KeyValuePair<int, IMount>> Mounts { get; }
        int Version { get; }

        //Import(IEnumerable<Mount> mounts, PersistenceDirection persistenceDirection, bool autoResolveDuplicatePriorities = false);
        //Import(Mount mount, PersistenceDirection persistenceDirection, bool autoResolveDuplicatePriorities = false);
    }
}
