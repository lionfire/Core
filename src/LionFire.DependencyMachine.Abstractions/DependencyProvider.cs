using System.Collections.Generic;
using System.Linq;

namespace LionFire.DependencyMachine
{
    public class DependencyProvider : DependencyProvider<string>
    {
        public override string Key => key;
        private string key;

        public DependencyProvider(string name, params string[] dependsOn)
        {
            this.key = name;

            if (dependsOn.Any())
            {
                Dependencies = new List<object>(dependsOn);
            }
        }
    }
}
