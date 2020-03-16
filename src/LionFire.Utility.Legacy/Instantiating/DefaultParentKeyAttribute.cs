using System;

namespace LionFire.Instantiating
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DefaultParentKeyAttribute : Attribute
	{
		readonly string defaultParent;

		public DefaultParentKeyAttribute(string defaultParentKey)
		{
			this.defaultParent = defaultParentKey;
		}

		public string DefaultParentKey {
			get { return defaultParent; }
		}
	}
}

