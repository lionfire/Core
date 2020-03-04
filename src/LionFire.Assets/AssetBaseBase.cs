using LionFire.Structures;
using LionFire.Assets;
using System.ComponentModel;
using System;

namespace LionFire.Assets
{
    public abstract class AssetBaseBase<TValue> : IKeyable, INotifyPropertyChanged, IAsset<TValue>
    {
        public Type TreatAsType => typeof(TValue);

        public string AssetPath { get => Key; set => Key = value; }

        /// <summary>
        /// Implementors should invoke OnKeyChanged
        /// </summary>
        public abstract string Key { get; set; }
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

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
