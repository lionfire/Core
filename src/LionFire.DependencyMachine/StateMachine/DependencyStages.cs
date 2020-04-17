using System.Collections.Generic;

namespace LionFire.DependencyMachine
{
    public static class DependencyStages
    {
        public static IEnumerable<IDependencyMachineParticipant> CreateStageChain(params string[] stages)
        {
            string previousStage = null;
            foreach(var stage in stages)
            {
                yield return new DependencyStagePlaceholder(stage, previousStage == null ? (string[])null : new string[] { previousStage });
                previousStage = stage;
            }
        }
    }
}
