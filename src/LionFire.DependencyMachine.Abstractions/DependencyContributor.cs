using LionFire.Collections.Concurrent;
using LionFire.Resolves;
using System;

namespace LionFire.DependencyMachine
{

    public class DependencyContributor : ResolvableDependency
    {
        public override string Key => key;
        private readonly string key;


        public DependencyContributor(string name)
        {
            Contributes = new object[] { name };
            this.key = name + $"({Guid.NewGuid().ToString()})";
        }
    }

}
