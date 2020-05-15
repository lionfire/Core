using System;
using System.Linq;

namespace LionFire.DependencyMachines
{
    public class DependencyForContribution : StartableParticipant<DependencyForContribution>
    {
        public DependencyForContribution(string contributes, params string?[] dependencies)
        {
            Key = $"'{contributes}' contribution depends on '{dependencies.Select(d=>d??"(null)").Aggregate((x,y)=>$"{x}, {y}")}'";
            this.Contributes(contributes);
            this.DependsOn(dependencies);
            //Flags |= ParticipantFlags.Noop; // TODO: IsNoop property?
        }
    }
}
