using LionFire.Collections.Concurrent;
using LionFire.Resolves;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.DependencyMachine
{

    public class DependencyContributor : ResolvableDependency
    {
        public override string Key => key;
        private readonly string key;


        public DependencyContributor(string contributesTo, string name = null, params string[] dependsOn)
        {
            Contributes = new object[] { contributesTo };
            this.key = name ?? (contributesTo + $" ({Guid.NewGuid().ToString()})");
            if (dependsOn.Any())
            {
                Dependencies = new List<object>(dependsOn);
            } 
        }

        public override string ToString() => Key;
    }

}
