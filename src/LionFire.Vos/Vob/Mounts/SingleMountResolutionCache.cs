using System.Collections.Generic;

namespace LionFire.Vos.Mounts
{
    
    public struct SingleMountResolutionCache : IMountResolutionCache
    {
        public Vob Vob { get; }
        public SingleMountResolutionCache(Vob vob, int version, Mount mount)
        {
            Vob = vob;
            Version = version;
            Mount = mount;
        }
        public int Version { get; }
        public IEnumerable<KeyValuePair<int, Mount>> Mounts { get { yield return new KeyValuePair<int, Mount>(1, Mount); } }
        public Mount Mount { get; }
    }
}
