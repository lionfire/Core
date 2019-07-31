#if TOPORT // Is this needed?
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    /// <summary>
    /// A generic reference class
    /// </summary>
    public class Reference : ReferenceBase
    {
        public Reference()
        {
        }

        //public new string Scheme { get;  set; }
        //public new string Scheme { set; }

        //public string Host { get; set; }
        //public string Port { get; set; }
        //public string Path { get; set; }
        //public string Layer { get; set; }
        //public string TypeName { get; set; }

        //public Type Type { get; set; }

        public override string Scheme
        {
            get;
            set; // TODO: Set once
        }

        public override string Key
        {
            get
            {
                return this.ToString();
            }
            set
            {
                throw new NotImplementedException("set_Key");
            }
        }
    }
}
#endif