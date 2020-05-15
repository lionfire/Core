#nullable enable
using System.Collections.Generic;

namespace LionFire.DependencyMachines
{
    public static class DependencyMachineConfigExtensions
    {
        public static DependencyMachineConfig Register(this DependencyMachineConfig config, IParticipant participant)
        {
            config.ParticipantInstances.Add(participant);
            return config;
        }
        public static DependencyMachineConfig Register(this DependencyMachineConfig config, IEnumerable<IParticipant> participants)
        {
            config.ParticipantInstances.AddRange(participants);
            return config;
        }
        public static DependencyMachineConfig StageDependsOn(this DependencyMachineConfig config, string dependantStage, string prerequisiteStage)
            => config.Register(new Placeholder(dependantStage).DependsOn(prerequisiteStage));
        
    }
}
