using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Assets
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AssetPathAttribute : Attribute
    {
        public AssetPathAttribute(string assetPath)
        {
            this.assetPath = assetPath;
        }

        public string AssetPath {
            get { return assetPath; }
        }
        private readonly string assetPath;
    }

    
}
