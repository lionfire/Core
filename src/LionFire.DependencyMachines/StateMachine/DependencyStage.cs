using LionFire.Structures;
using System;
using System.Collections.Generic;

namespace LionFire.DependencyMachines
{
    public class DependencyStage : IKeyable
    {
        internal DependencyStage() { }

        public string Key { get; set; }

        public HashSet<string> Provides { get; set; }

        //public IEnumerable<object> Provides { get; set; }
        public IEnumerable<object> Requries { get; set; }

        public List<IParticipant> Members { get; } = new List<IParticipant>();
        public int Id { get; internal set; }

        public void Add(IParticipant member)
        {
            foreach(var provided in member.Provides)
            {
                Provides.Add(provided.KeyForContributed());
            }
            Members.Add(member);
        }
    }
}
