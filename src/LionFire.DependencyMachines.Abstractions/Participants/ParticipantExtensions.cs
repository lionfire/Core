#nullable enable
using System.Collections.Generic;

namespace LionFire.DependencyMachines
{
    public static class ParticipantExtensions
    {
        public static T Contributes<T>(this T participant, params string[] stage)
            where T : StartableParticipant<T>
        {
            participant.Contributes ??= new List<object>();
            participant.Contributes.AddRange(stage);
            return participant;
        }
        public static T DependsOn<T>(this T participant, params string[] stages)
         where T : StartableParticipant<T>
        {
            participant.Dependencies ??= new List<object>();
            participant.Dependencies.AddRange(stages);
            return participant;
        }
    }

}
