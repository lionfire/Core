#nullable enable
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

namespace LionFire.Vos
{

    /// <remarks>
    /// Philosophy:
    ///  - All RootVobs are initialized once at the construction of VosRootManager
    ///  - This could be moved to on-demand initialization of RootVobs (and tracking whether RootVobs are initialized) if they grow large or numerous and sparsely needed.
    /// </remarks>
    public class RootManager
    {
        #region Dependencies

        VosInitializer VosInitializer { get; }
        readonly VosOptions vosOptions;

        #endregion

        #region State

        RootVob? namelessRootVob;
        ConcurrentDictionary<string, RootVob> roots = new ConcurrentDictionary<string, RootVob>();

        #endregion

        #region Construction

        public RootManager(IOptionsMonitor<VosOptions> vosOptionsMonitor, VosInitializer vosInitializer)
        {
            this.vosOptions = vosOptionsMonitor.CurrentValue;
            VosInitializer = vosInitializer;

            InitializeAll();
        }

        private void InitializeAll()
        {
            foreach (var rootName in vosOptions.RootNames.Distinct())
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
                VosInitializer.Initialize(rootVob);
                rootVob.InitializeMounts();
            }
        }

        #endregion

        #region Methods

        public RootVob? Get(string? rootName = null)
        {
            if (rootName == null || rootName == "")
            {
                return namelessRootVob; // Might be null
            }

            if (roots == null) roots = new ConcurrentDictionary<string, RootVob>();

            return roots.GetOrAdd(rootName, n => new RootVob(n, vosOptions));
        }

        #endregion

    }
}
