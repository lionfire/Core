using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LionFire.CommandLine.Arguments
{
    public class CliArgumentAttribute : Attribute, ICliArgumentAttribute
    {
        /// <summary>
        /// Defaults to null (none)
        /// </summary>
        public char ShortForm {
            get;
            set;
        }

        /// <summary>
        /// Defaults to the lowercase version of the method name
        /// </summary>
        public string LongForm {
            get;
            set;
        }

        /// <summary>
        /// Defaults to true for verbs, false for options
        /// </summary>
        public bool IsExclusive {
            get;
            set;
        }

        /// <summary>
        /// Defaults to false for verbs, true for options
        /// </summary>
        public bool UsesOptionPrefix {
            get; set;
        } = false;

        public string Description { get; set; }
        public string Usage { get; set; }
    }


}
