using LionFire.Structures;
using System;
using System.Collections.Generic;

namespace LionFire.DependencyMachine
{
    public class DependencyStage : IKeyable
    {
        internal DependencyStage() { }

        public string Key { get; set; }

        public HashSet<string> Provides { get; set; }

        //public IEnumerable<object> Provides { get; set; }
        public IEnumerable<object> Requries { get; set; }

        public List<IDependencyMachineParticipant> Members { get; } = new List<IDependencyMachineParticipant>();
        public int Id { get; internal set; }

        public void Add(IDependencyMachineParticipant member)
        {
            foreach(var provided in member.Provides)
            {
                Provides.Add(provided.KeyForContributed());
            }
            Members.Add(member);
        }
    }
}
