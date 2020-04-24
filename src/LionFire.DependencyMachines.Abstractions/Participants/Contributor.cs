using LionFire.Collections.Concurrent;
using LionFire.Resolves;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.DependencyMachines
{
    public class Contributor : Participant<Contributor>
    {
        public Contributor(string contributesTo, string? name = null, params string[] dependsOn)
        {
            Contributes = new List<object> { contributesTo };
            this.Key = name ?? (contributesTo + $" ({Guid.NewGuid()})");
            if (dependsOn.Any())
            {
                Dependencies = new List<object>(dependsOn);
            } 
        }
    }
}
