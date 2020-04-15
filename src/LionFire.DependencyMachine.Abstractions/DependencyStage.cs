using LionFire.Structures;
using System.Collections.Generic;

namespace LionFire.DependencyMachine
{
    public class DependencyStage : IKeyable
    {
        public string Key { get; set; }

        public object Provides { get; set; }

        //public IEnumerable<object> Provides { get; set; }
        public IEnumerable<object> Requries { get; set; }
    }
}
