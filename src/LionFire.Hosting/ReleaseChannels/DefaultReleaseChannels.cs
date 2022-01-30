using System.Collections.Generic;
using System.Linq;

namespace LionFire.Deployment
{
    public static class DefaultReleaseChannels
    {
        public static IEnumerable<ReleaseChannel> All
        {
            get
            {
                yield return Prod;
                yield return Green;
                yield return Blue;

                yield return Beta;
                yield return Alpha;

                yield return Nightly;
                yield return AutoBuild;

                yield return Test;

                yield return Dev;
                yield return Src;
            }
        }

        public static IDictionary<string, ReleaseChannel> Dictionary => dictionary ??= All.ToDictionary(x => x.Id);
        private static IDictionary<string, ReleaseChannel> dictionary;

        public static ReleaseChannel Prod = new("prod", "Production", 0);
        public static ReleaseChannel Green = new("green", "Production", 0);
        public static ReleaseChannel Blue = new("blue", "Production", 1);

        public static ReleaseChannel Beta = new ("beta", "Beta", 2);
        public static ReleaseChannel Alpha = new ("alpha", "Alpha", 3);
        public static ReleaseChannel Nightly = new ("nightly", "Nightly", 4);
        public static ReleaseChannel AutoBuild = new ("autobuild", "AutoBuild", 5);
        public static ReleaseChannel Test = new("test", "Test", 6);
        public static ReleaseChannel Dev = new("dev", "Development", 7);
        public static ReleaseChannel Src = new("src", "Source", 8);

        public static int? TryGetReleaseChannelPortOffset(string id)
            => All.Where(r => r.Id == id).FirstOrDefault().precedence;

        //public static Dictionary<string, int> ReleaseChannelPortOffsets = new()
        //{
        //    ["prod"] = 0,
        //    ["green"] = 0,
        //    ["blue"] = 1,

        //    ["beta"] = 2,
        //    ["alpha"] = 3,
        //    //["gamma"] = 3,
        //    //["test"] = 0,
        //    ["dev"] = 6,
        //};
    }

}
