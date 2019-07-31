using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Vos;
using System.Threading;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using LionFire.Referencing;

namespace LionFire.Assets
{

    [Asset(IsAbstract = true)]
    public class AssetBase : IAsset, INotifyPropertyChanged
    {

        public AssetBase()
        {
            this.Package = AssetContext.Current.DefaultCreatePackage;
        }


        #region AssetID

        [SerializeDefaultValue(false)]
        public AssetID ID
        {
            get => assetID;
            set
            {
                //l.Warn("AssetBase ID set to " + value + " " + Environment.StackTrace);
                assetID = value;
            }
        }
        private AssetID assetID;

        public string AssetSubPath { get; protected set; }

        private static ILogger l = Log.Get();

        #endregion

        //[Ignore]
        //public Package Package
        //{
        //    get => ID?.Package;
        //    set => ID.Package = value;
        //}

        #region Package

        [Ignore]
        public Package Package
        {
            get
            {
                if (_package == null)
                {
                    if (ID?.PackageName != null)
                    {
                        Package = AssetManager<Package>.Load(new AssetID(ID.PackageName) { PackageName = ID.PackageName });
                    }
                }
                return _package;
            }
            set
            {
                if (value != null)
                {
                    if (ID == null)
                    {
                        ID = new AssetID();
                    }
                    ID.PackageName = value.ID.Name; // Throws if already set to something different.
                }
                else
                {
                    if (ID != null)
                    {
                        ID.PackageName = null;
                    }
                }
                _package = value;
            }
        }
        private Package _package;

        #endregion

        //[Ignore]
        //public virtual string AssetTypePath {
        //    get { return AssetTypePath; }
        //    set { AssetTypePath = value; }
        //}

        [Ignore]
        public virtual string AssetTypePath // REVIEW FIXME - This currently returns Name, but AssetBase<> returns the AssetPath (e.g. Loadout/MyFolder/MyLoadout)
        {
            get => ID?.Name;
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

        public virtual Type Type => this.GetType();

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

        public override string ToString() => "[" + (ID == null ? (this.GetType().Name + " (null ID)") : ID.ToString()) + "]";

        public virtual object Clone()
        {
            var clone = (AssetBase)this.MemberwiseClone();
            if (ID != null)
            {
                if (this.Package != null)
                {
                    clone.ID = new AssetID();
                    clone.ID.PackageName = this.Package.Name;
                }
            }
            return clone;
        }

        //IReadHandle IHasReadHandle.ReadHandle => VosAssets.AssetToHandle(this);  TOPORT ?

        

        #region Misc


        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion


        #endregion
    }

}
