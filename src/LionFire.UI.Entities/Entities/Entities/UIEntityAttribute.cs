using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.UI
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class DefaultKeyAttribute : Attribute
    {
        private readonly string key;

        public DefaultKeyAttribute(string key)
        {
            this.key = key;
        }

        public string Key => key;
    }
}
