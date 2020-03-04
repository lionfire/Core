using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Assets
{
    /// <summary>
    /// Use this to specify the DefaultPath used to create the location for storing the asset.
    /// E.g., for type B, an asset may be stored at A/MyBname/C/AssetSubpath if [Asset("MyBname")] is specified. // REVIEW - how was this working in prior gen?
    /// Interfaces must have [Asset] in order to be included in the path.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class AssetAttribute : Attribute
    {
        #region Construction

        public AssetAttribute()
        {
        }

        public AssetAttribute(string defaultPath, bool isAbstract = false)
        {
            this.defaultPath = defaultPath;
            this.IsAbstract = isAbstract;
        }

        #endregion

        /// <summary>
        /// Ends with a slash.  (If set without a slash, it will be added.)
        /// </summary>
        public string DefaultPath
        {
            get => defaultPath;
            set => defaultPath = value.EndsWith("/") ? value : value + "/";
        }

        public bool IsAbstract { get; set; }

        private string defaultPath;

    }
}
