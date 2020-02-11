#if UNUSED // What was this for?
using LionFire.Resources;
using LionFire.Structures;

namespace LionFire.Assets.Tests
{
    public class KeyedResource : IAsset, IKeyed<string>
    {
        public string Key { get; protected set; }

        public KeyedResource() { }
        public KeyedResource(string key)
        {
            this.Key = key;
        }
        
        public string Str { get; set; }
        public int Int { get; set; }

    }
}
#endif