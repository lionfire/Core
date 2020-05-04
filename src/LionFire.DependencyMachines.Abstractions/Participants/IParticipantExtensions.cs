#nullable enable
using LionFire.Ontology;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.DependencyMachines
{
    public static class IParticipantExtensions
    {
        public static IEnumerable<IParticipant> GetParticipants(this object obj)
        {
            if (obj is IHas<IParticipant> hasParticipant)
            {
                yield return hasParticipant.Object;
            }
            if (obj is IHasMany<IParticipant> hasParticipants)
            {
                foreach (var p in hasParticipants.Objects) { yield return p; }
            }
        }

        #region Contributes

        public static IParticipant Contributes(this IParticipant participant, params string[] stage)
        {
            participant.Contributes ??= new List<object>();
            participant.Contributes.AddRange(stage);
            return participant;
        }
        public static IParticipant Contributes(this IParticipant participant, params Enum[] stages)
               => participant.Contributes(stages.Select(s => s.ToString()).ToArray());

        #endregion

        #region DependsOn

        public static IParticipant DependsOn(this IParticipant participant, params string?[] stages)
        {
            participant.Dependencies ??= new List<object>();
            participant.Dependencies.AddRange(stages.Cast<object>().Where(s=> s != null));
            return participant;
        }

        public static IParticipant DependsOn(this IParticipant participant, params Enum[] stages) 
            => participant.DependsOn(stages.Select(s => s.ToString()).ToArray());

        #endregion

        #region RequirementFor

        //public static IParticipant RequirementFor(this IParticipant participant, params string[] stage)
        //{
            
        //    participant.PrerequisiteFor ??= new List<object>();
        //    participant.PrerequisiteFor.AddRange(stage);
        //    return participant;
        //}
        //public static IParticipant RequirementFor(this IParticipant participant, params Enum[] stages)
        //            => participant.RequirementFor(stages.Select(s => s.ToString()).ToArray());

        //public static IDependencyStateMachine StageIsRequirementForStage(this IDependencyStateMachine dependencyStateMachine, string prerequisiteStage, string dependantStage)
        //      => dependencyStateMachine.Register(new Placeholder(prerequisiteStage)
        //          .PrerequisiteFor(dependantStage));

        #endregion
    }

}
