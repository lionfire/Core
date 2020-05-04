using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.DependencyMachines
{
    public static class DependencyStages
    {
        public static IEnumerable<IParticipant> CreateStageChain(params Enum[] stages)
            => CreateStageChain(stages.Select(s => s.ToString()).ToArray());

        public static IEnumerable<IParticipant> CreateStageChain(params string[] stages)
        {
            string? previousStage = null;
            foreach(var stage in stages)
            {
                yield return new Dependency(stage, previousStage == null ? (string?[]?)null : new string?[] { previousStage });
                previousStage = stage;
            }
        }
    }
}
