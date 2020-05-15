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
        public OrphanedStagePolicy OrphanedStagePolicy { get; set; }
            //= OrphanedStagePolicy.Execute;
            = OrphanedStagePolicy.Throw;

        public List<Type> AutoRegisterFromServiceTypes { get; } = new List<Type>();

        public List<Func<IServiceProvider, IEnumerable<IParticipant>>> AutoRegisterManyParticipants { get; } = new List<Func<IServiceProvider, IEnumerable<IParticipant>>>();
        public List<Func<IServiceProvider, IParticipant>> AutoRegisterParticipants { get; } = new List<Func<IServiceProvider, IParticipant>>();

        public List<Func<IServiceProvider, IParticipant>> InjectedParticipants { get; } = new List<Func<IServiceProvider, IParticipant>>();
        public List<IParticipant> ParticipantInstances { get; } = new List<IParticipant>();

        
        public bool EnableLogging { get; set; }
    }
}
