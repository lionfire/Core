using LionFire.MultiTyping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    public class PersistenceContext : MultiTypable
    {
        public string RootPath { get; set; }
        public object RootObject { get; set; }

        public bool? AllowInstantiator { get; set; } // REVIEW

        /// <summary>
        /// Defaults to typeof(object) which will save the full type information.
        /// </summary>
        public Type SaveType { get; set; }
    }
}
