#nullable enable
using LionFire.DependencyMachines;
using LionFire.ExtensionMethods;
using LionFire.Ontology;
using LionFire.Structures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        public ILogger<IVos> Logger { get; }

        public VosOptions Options => vosOptions;
        readonly VosOptions vosOptions;

        #endregion

        IVos IHas<IVos>.Object => this; // REVIEW - is this needed?

        #region State

        // Use Lazy<> so that value factories are only invoked once, as we do not want to create multiple RootVobs.
        ConcurrentDictionary<string, Lazy<RootVob>> roots = new ConcurrentDictionary<string, Lazy<RootVob>>();

        #endregion

        #region Construction

#if DEBUG // REVIEW - shouldn't be needed
        private static object rootLock = new object();
#endif

        public RootManager(IServiceProvider serviceProvider, IOptionsMonitor<VosOptions> vosOptionsMonitor, ILogger<IVos> logger)
        {
#if DEBUG // REVIEW - shouldn't be needed
            if (!VosStatic.AllowMultipleDefaultRoots)
            {
                lock (rootLock) // REVIEW
                {
                    if (ManualSingleton<RootManager>.Instance != null)
                    {
                        throw new AlreadySetException("A default RootManager has already been created.  There can only be one when VosStatic.AllowMultipleDefaultRoots is true.");
                    }
                    else
                    {
#endif
                        ManualSingleton<RootManager>.Instance = this;
#if DEBUG
                    }
                }
            }
#endif

            this.vosOptions = vosOptionsMonitor.CurrentValue;
            ServiceProvider = serviceProvider;
            Logger = logger;
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
            roots ??= new ConcurrentDictionary<string, Lazy<RootVob>>();

            var result = roots.GetOrAdd(rootName, n =>
            {
                if (!vosOptions.RootNames.Contains(n) && !vosOptions.AllowAdditionalRootNames) throw new KeyNotFoundException($"VosOptions.AllowAdditionalRootNames is false and the requested rootName '{n}' was not listed in VosOptions.RootNames.");
                //var root = new RootVob(this, n, vosOptions);

                // REVIEW - I am not sure why there are multiple initializations attempted.

                //if (rootName == VosConstants.DefaultRootName && !VosStatic.AllowMultipleDefaultRoots && ManualSingleton<RootVob>.Instance != null)
                //{
                //    return ManualSingleton<RootVob>.Instance;
                //}
                //try
                //{
                    return new Lazy<RootVob>(() => ActivatorUtilities.CreateInstance<RootVob>(ServiceProvider, this, n), LazyThreadSafetyMode.ExecutionAndPublication); 
                //}
                //catch (AlreadySetException) when (rootName == VosConstants.DefaultRootName)
                //{
                //    return ManualSingleton<RootVob>.Instance ?? throw new Exception("RootVob ctor returned AlreadySetException but ManualSingleton<RootVob>.Instance  is not set");
                //}
            });

            // ENH - if result is LazyInit and is not initialized, initialize it.

            return result.Value;
        }

        #endregion

        IEnumerable<IParticipant> IHasMany<IParticipant>.Objects
        {
            get
            {
                yield return new StartableParticipant(ServiceProvider)
                {
                    StartAction = () => Logger.LogInformation("Vos initialized"),
                }
                .Provide("vos:")
                .RootDependency();

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
