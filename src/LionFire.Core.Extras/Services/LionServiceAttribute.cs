using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.LionRing
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class LionServiceAttribute : Attribute
    {
        public uint KnownPort
        {
            get { return knownPort; }
            set { knownPort = value; }
        } private uint knownPort = 0;

        public string KnownPath
        {
            get { return knownPath; }
            set { knownPath = value; }
        } private string knownPath = null;

		public LionServiceAttribute()
		{
		}
		public LionServiceAttribute(uint knownPort)
		{
			this.KnownPort = knownPort;
		}
    }
}
