using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.DependencyMachines
{
    public class DependencyStage : IKeyable
    {
        internal DependencyStage() { }

        public string? Key { get; set; }

        public IEnumerable<string>? Provides => provides;
        private HashSet<string>? provides;

        public IEnumerable<object>? Dependencies => Members.SelectMany(m => m.Dependencies);

        public List<IParticipant> Members { get; } = new List<IParticipant>();
        public int Id { get; internal set; }
        

        public void Add(IParticipant member)
        {
            foreach(var provided in member.Provides)
            {
                provides ??= new HashSet<string>();
                provides.Add(provided.KeyForContributed());
            }
            Members.Add(member);
        }

        public override string ToString() => Key ?? $"{{DependencyStage Id={Id}}}";
    }
}
