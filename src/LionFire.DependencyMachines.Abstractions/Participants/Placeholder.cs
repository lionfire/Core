using System.Collections.Generic;

namespace LionFire.DependencyMachines
{
    public class Placeholder : StartableParticipant<Placeholder>
    {
        public Placeholder(string key)
        {
            Key = key;
            this.Contributes(key);
            Flags |= ParticipantFlags.Noop;
        }
    }
}
