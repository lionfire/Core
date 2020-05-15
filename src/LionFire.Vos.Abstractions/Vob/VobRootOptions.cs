using LionFire.DependencyMachines;
using LionFire.Vos.Mounts;
using System;
using System.Collections.Generic;

namespace LionFire.Vos
{
    public class VobRootOptions
    {
        public ServiceProviderMode ServiceProviderMode { get; set; } = ServiceProviderMode.UseRootManager;

        public List<Func<IRootVob, IEnumerable<IParticipant>>> ParticipantsFactory { get; } = new List<Func<IRootVob, IEnumerable<IParticipant>>>();

        public bool UseVosOptionsInitializer { get; set; } = true;

        public List<ITMount> Mounts { get; set; } = new List<ITMount>();

    }
}
