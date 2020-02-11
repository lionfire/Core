using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Assets
{

    /// <summary>
    /// Contains the information required to locate an asset at a particular path, package (if any), and location (if any).  If package or location are not specified, the active Vos mounts will be searched in the order of their precedence.
    /// </summary>
    /// <typeparam name="AssetType"></typeparam>
    public struct AssetIdentifier<AssetType>
        where AssetType : class
    {

        public AssetIdentifier(string assetTypePath) { this.AssetTypePath = assetTypePath; Package = null; Location = null; }

        public string AssetTypePath { get; set; }
        public string Package { get; set; }
        public string Location { get; set; }

        #region Derived

        public string AssetPath {
            get {
                return AssetTypePath.AssetPathFromAssetTypePath<AssetType>();
            }
        }

        #endregion

        #region Misc 


        public override int GetHashCode()
        {
            return (AssetTypePath == null ? 0 : AssetTypePath.GetHashCode())
                ^ (Package == null ? 0 : Package.GetHashCode())
               ^ (Location == null ? 0 : Location.GetHashCode());
        }
        public override bool Equals(object obj)
        {

            if (!(obj is AssetIdentifier<AssetType>)) return false;

            var other = (AssetIdentifier<AssetType>)obj;

            return AssetTypePath == other.AssetTypePath
                && Package == other.Package
                 && Location == other.Location;
        }
        
        #endregion
    }
}
