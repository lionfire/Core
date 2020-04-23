using LionFire.DependencyMachines;
using LionFire.Ontology;
using LionFire.Services;
using LionFire.Structures;
using LionFire.Vos.Mounts;
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

    public class RootVob : Vob, IRootVob, IHas<IRootManager>, IHostedService, IHasMany<IParticipant>
    {
        public IRootManager RootManager { get; private set; }
        IRootManager IHas<IRootManager>.Object => RootManager;

        public VosOptions VosOptions { get; }

        public static bool AllowMultipleDefaultRoots => LionFireEnvironment.IsMultiApplicationEnvironment;

        /// <summary>
        /// Empty for default Root
        /// </summary>
        public string RootName { get; }


        public RootVob(IRootManager rootManager, VosOptions vosOptions) : this(rootManager, VosConstants.DefaultRootName, vosOptions)
        {
        }

        //public static RootVob Create(IRootManager rootManager, string rootName, VosOptions vosOptions)
        //{
        //}

        public RootVob(IRootManager rootManager, string rootName, VosOptions vosOptions, IOptionsMonitor<List<VobInitializer>> vobInitializers) : base(null, null)
        {
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

            participants = new List<IParticipant>
            {
                new Contributor("mounts", $"{this} mounts")
                {
                    StartAction = (c,ct) =>
                    {
                        InitializeMounts();
                        return Task.FromResult<object>(null);
                    },
                },
            };
        }

        public IVob InitializeMounts()
        {
            foreach (var tMount in VosOptions[RootName].Mounts)
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
}
