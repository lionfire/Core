using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.DependencyMachines
{
    public static class DependencyStages
    {
        public static IEnumerable<IParticipant> CreateStageChain(params Enum[] stages)
            => CreateStageChain(stages.Select(s => s?.ToString()).ToArray());

        public static IEnumerable<IParticipant> CreateStageChain(params string?[] stages)
        {
            string? previousStage = null;
            bool isFirstStage = true;
            foreach (var stage in stages)
            {
                try
                {
                    if (stage == null) continue;
                    if (!isFirstStage)
                    {
                        yield return new DependencyForContribution(stage, new string?[] { previousStage });
                    }
                }
                finally
                {
                    previousStage = stage;
                    isFirstStage = false;
                }
            }
        }
    }
}
