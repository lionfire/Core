#nullable enable
using System;
using System.Collections.Generic;

namespace LionFire.DependencyMachines
{
    public class DependencyMachineConfig
    {
        public List<Type> AutoRegisterFromServiceTypes { get; } = new List<Type>();

        public List<Func<IServiceProvider, IEnumerable<IParticipant>>> AutoRegisterManyParticipants { get; } = new List<Func<IServiceProvider, IEnumerable<IParticipant>>>();
        public List<Func<IServiceProvider, IParticipant>> AutoRegisterParticipants { get; } = new List<Func<IServiceProvider, IParticipant>>();

        public List<IParticipant> Participants { get; } = new List<IParticipant>();
    }
}
