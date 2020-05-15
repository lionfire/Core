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

        public IEnumerable<object>? Dependencies => Members.SelectMany(m => m.Dependencies ?? Enumerable.Empty<object>());

        public bool AllowNoDependencies { get; set; }

        public List<IParticipant> Members { get; } = new List<IParticipant>();
        public int Id { get; internal set; }
        public bool IsUseless => !Members.Any() && !Provides.NullableAny();

        public void Add(IParticipant member)
        {
            if (member.Provides != null)
            {
                foreach (var provided in member.Provides)
                {
                    provides ??= new HashSet<string>();
                    provides.Add(provided.ToDependencyKey());
                }
            }
            Members.Add(member);
        }

        public override string ToString() => Key ?? $"{{DependencyStage Id={Id}}}";
        public string ToLongString() => $"{this} [ {Members.Select(m => m.ToString()).AggregateOrDefault((x, y) => $"{x}, {y}")} ]";
    }

 
}
