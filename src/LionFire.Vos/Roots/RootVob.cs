#nullable enable
using LionFire.DependencyMachines;
using LionFire.Ontology;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Services;
using LionFire.Structures;
using LionFire.Vos.Mounts;
using LionFire.Vos.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LionFire.FlexObjects;

namespace LionFire.Vos
{
    public static class VosStatic
    {
        public static bool AllowMultipleDefaultRoots => LionFireEnvironment.IsMultiApplicationEnvironment;

    }

    public class RootVob : Vob, IRootVob, IHas<IVos>, IHostedService, IHasMany<IParticipant>, IHas<IServiceProvider>
    {
        public static int CreateCount = 0;

        public IVos RootManager { get; private set; }
        IServiceProvider? RootManagerServiceProvider => (RootManager as IHas<IServiceProvider>)?.Object;

        //IServiceProvider IHas<IServiceProvider>.Object => this.TryGetOwnVobNode<IServiceProvider>()?.Value ?? RootManagerServiceProvider;
        IServiceProvider? IHas<IServiceProvider>.Object => this.Query<IServiceProvider>(); // ?? RootManagerServiceProvider;

        IVos IHas<IVos>.Object => RootManager;

        public VosOptions VosOptions { get; }
        public VobRootOptions Options { get; }


        /// <summary>
        /// Empty for default Root
        /// </summary>
        public string RootName { get; }

        private static object rootLock = new object();

        public RootVob(IVos rootManager, string rootName, VosOptions vosOptions, IOptionsMonitor<VobRootOptions> OptionsMonitor, IServiceProvider? serviceProvider)
            : base(parent: null, name: null) // Note: Use null parent and null name even for named Roots
        {
            Interlocked.Increment(ref CreateCount);
            //if (CreateCount > 1) throw new Exception("TEMP - RootVob already created");
            this.RootName = rootName;

            //var ServiceProvider = ((IHas<IServiceProvider>)rootManager).Object;
            //this.AddOwn(serviceProvider); // DEPRECATED: VobNode
            if (serviceProvider != null) { this.AddSingle<IServiceProvider>(serviceProvider); }

            Options = OptionsMonitor.Get(rootName);
            if (Options.ServiceProviderMode == ServiceProviderMode.UseRootManager)
            {
                var r = RootManagerServiceProvider;
                if (r != null)
                {
                    this.AddOwn(r);
                }
            }

            RootManager = rootManager;
            VosOptions = vosOptions ?? new VosOptions();

            #region Set to ManualSingleton<RootVob>.Instance to this if applicable

            if (rootName == VosConstants.DefaultRootName)
            {
                if (!VosStatic.AllowMultipleDefaultRoots)
                {
                    lock (rootLock) // REVIEW
                    {
                        if (ManualSingleton<RootVob>.Instance != null)
                        {
                            throw new AlreadySetException("A default RootVob has already been created.  There can only be one default.  If you wish to create another, provide a rootName.  Set AllowMultipleDefaultRoots to true to allow multiple default RootVobs (only recommended for unit testing or special cases.)");
                        }
                        else
                        {
                            ManualSingleton<RootVob>.Instance = this;
                        }
                    }
                }
            }

            #endregion
        }

        public RootVob(IVos rootManager, VosOptions vosOptions, IOptionsMonitor<VobRootOptions> OptionsMonitor, IServiceProvider serviceProvider)
        : this(rootManager, VosConstants.DefaultRootName, vosOptions, OptionsMonitor, serviceProvider:  serviceProvider)
        {

        }



        //public IVob InitializeMounts()
        //{
        //    foreach (var tMount in Options.Mounts)
        //    {
        //        this.Mount(tMount);
        //    }
        //    return this;
        //}

        IEnumerable<IParticipant> IHasMany<IParticipant>.Objects
        {
            get
            {
                var participants = new List<IParticipant>
                {
                    //new Contributor("mounts", $"{this} mounts") { StartAction = () => InitializeMounts(), },
                    //new Participant()
                    //{
                    //    StartAction = () =>{
                    //        var vob = this;
                    //            vob
                    //            .AddServiceProvider(s =>
                    //            {
                    //                s
                    //                .AddSingleton(_ => new ServiceDirectory((RootVob)vob))
                    //                .AddSingleton(vob)
                    //                .AddSingleton(vob.Root.RootManager)
                    //                .AddSingleton<VosPersister>()
                    //                .AddSingleton<VobMounter>()
                    //                ;
                    //            }, serviceProvider);
                    //            //.AddTransient<IServiceProvider, DynamicServiceProvider>() // Don't want this.  DELETE
                    //        },
                    //        Key = "RootInitializer",
                    //        Contributes("RootVobs"),
                    //},
                };

                if (VosOptions.GlobalRootInitializers != null) participants.AddRange(VosOptions.GlobalRootInitializers.SelectMany(i => i(this)));

                if (Options.ParticipantsFactory != null) participants.AddRange(Options.ParticipantsFactory.SelectMany(i => i(this)));

                if (Options.UseVosOptionsInitializer)
                {
                    if (RootName == VosConstants.DefaultRootName)
                    {
                        if (VosOptions.PrimaryRootInitializers != null) participants.AddRange(VosOptions.PrimaryRootInitializers.SelectMany(i => i(this)));
                    }
                    else
                    {
                        if (VosOptions.DefaultRootInitializers != null) participants.AddRange(VosOptions.DefaultRootInitializers.SelectMany(i => i(this)));
                    }
                }
                return participants;
            }
        }
        //private List<IParticipant> participants;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException("NEXT");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }

    public static class VobRootExtensions
    {
        public static IRootVob InitializeMounts(this IRootVob root)
        {
            foreach (var tMount in root.Options.Mounts)
            {
                root.Mount(tMount);
            }
            return root;
        }
    }
}
