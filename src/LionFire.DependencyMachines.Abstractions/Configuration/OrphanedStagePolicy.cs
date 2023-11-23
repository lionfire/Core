#nullable enable

namespace LionFire.DependencyMachines
{
    public enum OrphanedStagePolicy
    {
        /// <summary>
        /// ToException if a dependency does not have any dependencies.
        /// This only applies to IParticipants that are not No-ops.
        /// </summary>
        Throw,

        /// <summary>
        /// Ignore orphaned stages.  They will be available at CompiledDependencyMachine.IgnoredOrphans, as long as they are not no-ops.
        /// This is recommended when there is a standardized initialization process that includes features that may or may not 
        /// be enabled for a particular application.
        /// </summary>
        Disable,

        /// <summary>
        /// Treat dependency-less stages as depending on DependencyMachineConfig.RootDependency.
        /// This is the least-strict mode and not recommended, unless the developer does not have a complex dependency graph.
        /// </summary>
        Execute,
    }
}
