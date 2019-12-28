#nullable enable
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

namespace LionFire.Vos
{
    public class VosRootManager
    {
        readonly RootVob? namelessRootVob;

        readonly VosOptions vosOptions;
        //readonly IOptionsMonitor<VosOptions> vosOptionsMonitor;
        ConcurrentDictionary<string, RootVob> roots = new ConcurrentDictionary<string, RootVob>();

        public VosRootManager(IOptionsMonitor<VosOptions> vosOptionsMonitor)
        {
            this.vosOptions = vosOptionsMonitor.CurrentValue;

            foreach (var rootName in vosOptionsMonitor.CurrentValue.RootNames.Distinct())
            {
                var rootVob = new RootVob(rootName, this.vosOptions);
                if (rootName == "")
                {
                    namelessRootVob = rootVob;
                }
                else
                {
                    roots.TryAdd(rootName, rootVob);
                }
            }
        }


        public RootVob? Get(string? rootName = null)
        {
            if (rootName == null || rootName == "")
            {
                return namelessRootVob; // Might be null
            }

            if (roots == null) roots = new ConcurrentDictionary<string, RootVob>();

            return roots.GetOrAdd(rootName, n => new RootVob(n, vosOptions));
        }
    }
}
