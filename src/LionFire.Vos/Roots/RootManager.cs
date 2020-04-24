#nullable enable
using LionFire.DependencyMachines;
using LionFire.ExtensionMethods;
using LionFire.Ontology;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Vos
{
    /// <summary>
    /// A registry of RootVobs.
    ///  - Creates RootVobs
    ///  - Exposes RootVob's IParticipants
    ///  - (optional) Restricts RootVob names to those listed in VosOptions.
    /// </summary>
    public class RootManager : IVos, IHasMany<IParticipant>, IHas<IServiceProvider>
    {
        #region Dependencies

        IServiceProvider IHas<IServiceProvider>.Object => ServiceProvider;
        public IServiceProvider ServiceProvider { get; }

        public VosOptions Options => vosOptions;
        readonly VosOptions vosOptions;

        #endregion

        IVos IHas<IVos>.Object => this; // REVIEW - is this needed?

        #region State

        ConcurrentDictionary<string, RootVob> roots = new ConcurrentDictionary<string, RootVob>();

        #endregion

        #region Construction

        public RootManager(IServiceProvider serviceProvider, IOptionsMonitor<VosOptions> vosOptionsMonitor)
        {
            this.vosOptions = vosOptionsMonitor.CurrentValue;
            ServiceProvider = serviceProvider;
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

        //private async Task Initialize(RootVob rootVob, CancellationToken cancellationToken = default)
        //{
        //    await RootManagerVobInitializer.Initialize(rootVob, cancellationToken).ConfigureAwait(false);
        //    rootVob.InitializeMounts();
        //}

        #endregion

        #region Methods

        IRootVob? IVos.Get(string? rootName) => Get(rootName);
        public RootVob Get(string? rootName = "")
        {
            rootName ??= "";
            if (roots == null) roots = new ConcurrentDictionary<string, RootVob>();

            var result = roots.GetOrAdd(rootName, n =>
            {
                if (!vosOptions.RootNames.Contains(n) && !vosOptions.AllowAdditionalRootNames) throw new KeyNotFoundException($"VosOptions.AllowAdditionalRootNames is false and the requested rootName '{n}' was not listed in VosOptions.RootNames.");
                //var root = new RootVob(this, n, vosOptions);
                return ActivatorUtilities.CreateInstance<RootVob>(ServiceProvider, this, n);
            });

            // ENH - if result is LazyInit and is not initialized, initialize it.

            return result;
        }

        #endregion

        IEnumerable<IParticipant> IHasMany<IParticipant>.Objects
        {
            get
            {
                foreach (var rootName in vosOptions.RootNames)
                {
                    //var rootOptions = vosOptions.NamedRootOptions.TryGetValue(rootName); ENH
                    //if (rootOptions != null && rootOptions.LazyInit)
                    //{
                    //    continue;
                    //}
                    var rootVob = Get(rootName);

                    foreach (var p in rootVob.GetParticipants()) { yield return p; }
                }
            }
        }

    }
}
