using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Assets
{

    /// <summary>
    /// Use this to specify the DefaultPath used to create the location for storing the asset.
    /// E.g., for type B, an asset may be stored at A/MyBname/C/AssetSubpath if [Asset("MyBname")] is specified.
    /// Interfaces must have [Asset] in order to be included in the path.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class AssetAttribute : Attribute
    {

        public AssetAttribute()
        {
        }

        public AssetAttribute(string defaultPath, bool isAbstract = false)
        {
            this.defaultPath = defaultPath;
            this.IsAbstract = isAbstract;
        }

        /// <summary>
        /// Ends with a slash.  (If set without a slash, it will be added.)
        /// </summary>
        public string DefaultPath
        {
            get { return defaultPath; }
            set { defaultPath = value.EndsWith("/") ? value : value + "/"; }
        }

        public bool IsAbstract { get; set; }

        private string defaultPath;

        //public string Path
        //{

        //}
        //public string Extension // FUTURE?
    }
}
