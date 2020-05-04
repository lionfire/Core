using System;

namespace LionFire.DependencyMachines
{
    public class Dependency : StartableParticipant<Placeholder>
    {
        public Dependency(string name, params string?[]? dependencies)
        {
            Key = $"{name} dependency {Guid.NewGuid()}";
            this.Contributes(name);
            if (dependencies != null) { this.DependsOn(dependencies); }
            //Flags |= ParticipantFlags.Noop;
        }
    }
}
