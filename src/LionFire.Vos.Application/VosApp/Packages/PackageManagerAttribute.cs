using LionFire.Persistence.Handles;
using LionFire.Vos;
using System;

namespace LionFire.Packages
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class PackageManagerAttribute : Attribute
    {
        public PackageManagerAttribute(string name)
        {
            this.name = name;
        }

        public string Name => name;
        private readonly string name;
    }
}
