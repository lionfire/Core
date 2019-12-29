using System.Collections.Generic;

namespace LionFire.Vos.Mounts
{
    
    public struct SingleMountResolutionCache : IMountResolutionCache
    {
        public Vob Vob { get; }
        public SingleMountResolutionCache(Vob vob, int version, IMount mount)
        {
            Vob = vob;
            Version = version;
            Mount = mount;
        }
        public int Version { get; }
        public IEnumerable<KeyValuePair<int, IMount>> Mounts { get { yield return new KeyValuePair<int, IMount>(1, Mount); } }
        public IMount Mount { get; }
    }
}
