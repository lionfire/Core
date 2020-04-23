using System.Collections.Generic;

namespace LionFire.DependencyMachines
{
    public static class DependencyStages
    {
        public static IEnumerable<IParticipant> CreateStageChain(params string[] stages)
        {
            string previousStage = null;
            foreach(var stage in stages)
            {
                yield return new Placeholder(stage, previousStage == null ? (string[])null : new string[] { previousStage });
                previousStage = stage;
            }
        }
    }
}
