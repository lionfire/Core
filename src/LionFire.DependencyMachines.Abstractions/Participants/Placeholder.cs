using System.Collections.Generic;

namespace LionFire.DependencyMachines
{
    public class Placeholder : StartableParticipant<Placeholder>
    {
        public Placeholder(string name, params string[] dependencies)
        {
            Key = name;
            this.Contributes(name);
            this.DependsOn(dependencies);
            Flags |= ParticipantFlags.StageEnder;
        }
    }
}
