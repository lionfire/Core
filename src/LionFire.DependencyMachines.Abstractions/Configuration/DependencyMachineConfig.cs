#nullable enable
using System;
using System.Collections.Generic;

namespace LionFire.DependencyMachines
{

    public class DependencyMachineConfig
    {
        /// <summary>
        /// If true, allow additional participants to be added during start.
        /// Powerful but potentially confusing, so disabled by default.
        /// </summary>
        public bool IsMutableAfterStart { get => false; set { if (value) throw new NotImplementedException(); } } // = false;

        public static object RootDependency { get; } = new object();

        /// <summary>
        /// Default: Throw.  Recommend changing to Disable in application initialization frameworks once the implications are understood.
        /// </summary>
        public OrphanedStagePolicy OrphanedStagePolicy { get; set; } = OrphanedStagePolicy.Throw;

        public List<Type> AutoRegisterFromServiceTypes { get; } = new List<Type>();

        public List<Func<IServiceProvider, IEnumerable<IParticipant>>> AutoRegisterManyParticipants { get; } = new List<Func<IServiceProvider, IEnumerable<IParticipant>>>();
        public List<Func<IServiceProvider, IParticipant>> AutoRegisterParticipants { get; } = new List<Func<IServiceProvider, IParticipant>>();

        public List<IParticipant> Participants { get; } = new List<IParticipant>();
    }

    public static class DependencyMachineConfigExtensions
    {
        public static DependencyMachineConfig Register(this DependencyMachineConfig config, IParticipant participant)
        {
            config.Participants.Add(participant);
            return config;
        }
        public static DependencyMachineConfig StageDependsOn(this DependencyMachineConfig config, string dependantStage, string prerequisiteStage)
            => config.Register(new Placeholder(dependantStage).DependsOn(prerequisiteStage));
        
    }
}
