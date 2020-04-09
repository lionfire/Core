#nullable enable
using LionFire.Ontology;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
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
    public class RootManager : IRootManager 
    {
        #region Dependencies

        RootManagerVobInitializer VosInitializer { get; }

        readonly VosOptions vosOptions;

        #endregion

        IRootManager IHas<IRootManager>.Object => this;

        #region State

        //RootVob? namelessRootVob;
        ConcurrentDictionary<string, RootVob?> roots = new ConcurrentDictionary<string, RootVob?>();

        #endregion

        #region Construction

        public RootManager(IOptionsMonitor<VosOptions> vosOptionsMonitor, IServiceProvider provider)
        {
            this.vosOptions = vosOptionsMonitor.CurrentValue;
            VosInitializer = ActivatorUtilities.CreateInstance<RootManagerVobInitializer>(provider);

            //InitializeAll();
        }

        //private void InitializeAll()
        //{
        //    foreach (var rootName in vosOptions.RootNames.Distinct())
        //    {
        //        var rootVob = new RootVob(this, rootName, this.vosOptions);
        //        if (rootName == "")
        //        {
        //            namelessRootVob = rootVob;
        //        }
        //        else
        //        {
        //            roots.TryAdd(rootName, rootVob);
        //        }
        //        VosInitializer.Initialize(rootVob);
        //        rootVob.InitializeMounts();
        //    }
        //}

        private void Initialize(RootVob rootVob)
        {
            VosInitializer.Initialize(rootVob);
            rootVob.InitializeMounts();
        }

        #endregion

        #region Methods

        IRootVob? IRootManager.Get(string? rootName) => Get(rootName);
        public RootVob? Get(string? rootName = null)
        {
            //if (rootName == null || rootName == "")
            //{
            //    return namelessRootVob; // Might be null
            //}
            if (rootName == null) rootName = "";

            if (roots == null) roots = new ConcurrentDictionary<string, RootVob?>();

            return roots.GetOrAdd(rootName, n =>
            {
                if (!vosOptions.RootNames.Contains(n)) return null;
                var root = new RootVob(this, n, vosOptions);
                //root.InitializeMounts();
                //if (vosOptions.AutoInitRootVobs)
                //{
                    Initialize(root);
                //}
                return root;
            });
        }

        #endregion

    }
}
