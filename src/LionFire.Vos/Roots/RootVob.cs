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

namespace LionFire.Vos
{

    public class RootVob : Vob, IRootVob, IHas<IVos>, IHostedService, IHasMany<IParticipant>
    {
        public IVos RootManager { get; private set; }
        IServiceProvider RootManagerServiceProvider => (RootManager as IHas<IServiceProvider>)?.Object;


        IVos IHas<IVos>.Object => RootManager;

        public VosOptions VosOptions { get; }
        public RootVobOptions Options => VosOptions[RootName];

        public static bool AllowMultipleDefaultRoots => LionFireEnvironment.IsMultiApplicationEnvironment;

        /// <summary>
        /// Empty for default Root
        /// </summary>
        public string RootName { get; }

        public RootVob(IVos rootManager, VosOptions vosOptions, IOptionsMonitor<VobRootOptions> vobRootOptionsMonitor) : this(rootManager, VosConstants.DefaultRootName, vosOptions, vobRootOptionsMonitor)
        {
        }

        public RootVob(IVos rootManager, string rootName, VosOptions vosOptions, IOptionsMonitor<VobRootOptions> vobRootOptionsMonitor) : base(parent: null, name: null) // Note: Use null parent and null name even for named Roots
        {
            var vobRootOptions = vobRootOptionsMonitor.Get(rootName);
            if (vobRootOptions.ServiceProviderMode == ServiceProviderMode.UseRootManager)
            {
                var r = RootManagerServiceProvider;
                if (r != null) { this.AddOwn(r); }
            }

            RootManager = rootManager;
            VosOptions = vosOptions ?? new VosOptions();

            #region Set to ManualSingleton<RootVob>.Instance to this if applicable

            if (rootName == VosConstants.DefaultRootName)
            {
                if (!AllowMultipleDefaultRoots)
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

            #endregion

            this.RootName = rootName;

            #region Participants

            participants = new List<IParticipant>
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

            if (vosOptions.GlobalRootInitializers != null) participants.AddRange(vosOptions.GlobalRootInitializers.SelectMany(i => i(this)));

            if (vobRootOptions.ParticipantsFactory != null) participants.AddRange(vobRootOptions.ParticipantsFactory.SelectMany(i => i(this)));
            else
            {
                if (RootName == VosConstants.DefaultRootName)
                {
                    if (vosOptions.PrimaryRootInitializers != null) participants.AddRange(vosOptions.PrimaryRootInitializers.SelectMany(i => i(this)));
                }
                else
                {
                    if (vosOptions.DefaultRootInitializers != null) participants.AddRange(vosOptions.DefaultRootInitializers.SelectMany(i => i(this)));
                }
            }
            
            #endregion

        }


        public IVob InitializeMounts()
        {
            foreach (var tMount in Options.Mounts)
            {
                this.Mount(tMount);
            }
            return this;
        }

        IEnumerable<IParticipant> IHasMany<IParticipant>.Objects => participants;
        private List<IParticipant> participants;

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
