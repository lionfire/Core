using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Shell  // Change to namespace .Entities
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class UIEntityAttribute : Attribute
    {
        private readonly string key;

        public UIEntityAttribute(string key)
        {
            this.key = key;
        }

        public string Key
        {
            get { return key; }
        }
    }
}
