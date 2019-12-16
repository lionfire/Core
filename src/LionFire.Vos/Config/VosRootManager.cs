#nullable enable
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace LionFire.Vos
{
    public class VosRootManager
    {
        readonly RootVob? namelessRootVob;

        readonly VosOptions vosOptions;
        ConcurrentDictionary<string, RootVob> roots = new ConcurrentDictionary<string, RootVob>();

        public VosRootManager(IOptionsMonitor<VosOptions> vosOptions)
        {
            this.vosOptions = vosOptions.CurrentValue;
        }

        public VosRootManager(RootVob rootVob, IOptionsMonitor<VosOptions> vosOptions) : this(vosOptions)
        {
            this.namelessRootVob = rootVob;
        }

        public RootVob? Get(string? rootName = null)
        {
            if (rootName == null || rootName == "")
            {
                return namelessRootVob; // Might be null
            }

            if (roots == null) roots = new ConcurrentDictionary<string, RootVob>();

            return roots.GetOrAdd(rootName, n => new RootVob(vosOptions));
        }
    }
}
