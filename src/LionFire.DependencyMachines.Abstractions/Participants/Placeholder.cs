using System.Collections.Generic;

namespace LionFire.DependencyMachines
{
    public class Placeholder : StartableParticipant<Placeholder>
    {
        public Placeholder(string name)
        {
            Key = name;
            this.Contributes(name);
            Flags |= ParticipantFlags.Noop;
        }
    }
}
