using LionFire.Structures;
using LionFire.Assets;
using System.ComponentModel;
using System;
using LionFire.Referencing;
using LionFire.Types;

namespace LionFire.Assets
{
    public abstract class AssetBaseBase<TValue> : IKeyable, INotifyPropertyChanged, IAsset<TValue>, IReferencable
    {
        public Type TreatAsType => typeof(TValue);

        public string AssetPath { get => Key; set => Key = value; }

        /// <summary>
        /// Implementors should invoke OnKeyChanged
        /// </summary>
        public abstract string Key { get; set; }

        /// <summary>
        /// The "file name" portion of the AssetPath (omit the directory)
        /// </summary>
        public string Name => LionPath.GetName(Key);

        protected virtual void OnKeyChanged() => reference = null;

        public IAssetReference Reference
        {
            get
            {
                if (reference == null)
                {
                    reference = new AssetReference<TValue>(AssetPath);
                }
                return reference;
            }
        }
        protected AssetReference<TValue> reference;

        IReference IReferencable.Reference => Reference;


        #region Construction

        protected AssetBaseBase() { }
        protected AssetBaseBase(AssetReference<TValue> reference) { this.reference = reference; }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
