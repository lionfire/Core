#if TOPORT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Vos;
using System.Threading;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace LionFire.Assets
{

    [Asset(IsAbstract = true)]
    public class AssetBase : IAsset, INotifyPropertyChanged
    {


#region AssetID

        [SerializeDefaultValue(false)]
        public AssetID ID
        {
            get { return assetID; }
            set
            {
                //l.Warn("AssetBase ID set to " + value + " " + Environment.StackTrace);
                assetID = value;
            }
        }
        private AssetID assetID;

        private static ILogger l = Log.Get();

#endregion

        [Ignore]
        public Package Package
        {
            get { return ID == null ? null : ID.Package; }
            set { ID.Package = value; }
        }

        //[Ignore]
        //public virtual string AssetTypePath {
        //    get { return AssetTypePath; }
        //    set { AssetTypePath = value; }
        //}

        [Ignore]
        public virtual string AssetTypePath // REVIEW FIXME - This currently returns Name, but AssetBase<> returns the AssetPath (e.g. Loadout/MyFolder/MyLoadout)
        {
            get { return ID == null ? null : ID.Name; }
            set
            {
                if (ID != null && ID.Path != null) throw new AlreadyException("ID Path already set");
                ID = new AssetID(value);
            }
        }

        ///// <summary>
        ///// The path within the default location for the asset type.
        ///// E.g.: DefaultPath for Loadouts: Loadouts/
        ///// AssetSubpath for Loadout for unit type FF: Loadouts/FF/MyLoadoutName
        ///// Name: MyLoadoutName
        ///// </summary>
        //public virtual string AssetNamePath
        //{
        //    get
        //    {
        //        return Name;
        //    }
        //}

        public virtual string DefaultPath
        {
            get
            {
                if (String.IsNullOrWhiteSpace(AssetTypePath)) return null;
                return this.AssetPathFromAssetTypePath(AssetTypePath);
            }
        }

        public virtual Type Type { get { return this.GetType(); } }

        //public override bool Equals(object obj)
        //{
        //    if (obj.GetType() != this.GetType()) return false;

        //    IAsset otherAsset = obj as IAsset;
        //    if (otherAsset == null) return false;
        //    //return this.ID.Equals(otherAsset.ID);
        //    return this.ID == otherAsset.ID;
        //}

        //public override int GetHashCode()
        //{
        //    if (ID == null) return 0.GetHashCode();
        //    return this.ID.GetHashCode();
        //}

        public override string ToString()
        {
            return "[" + (ID == null ? (this.GetType().Name + " (null ID)") : ID.ToString()) + "]";
        }

        public virtual object Clone()
        {
            var clone = (AssetBase)this.MemberwiseClone();
            if (ID != null)
            {
                if (this.ID.Package != null)
                {
                    clone.ID = new AssetID();
                    clone.ID.Package = this.ID.Package;
                }
            }
            return clone;
        }

        IReadHandle IHasReadHandle.ReadHandle
        {
            get
            {
                return VosAssets.AssetToHandle(this);
            }
        }

#region Misc


#region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var ev = PropertyChanged;
            if (ev != null) ev(this, new PropertyChangedEventArgs(propertyName));
        }

#endregion


#endregion
    }

}
#endif