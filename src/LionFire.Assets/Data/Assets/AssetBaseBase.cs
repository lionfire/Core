using LionFire.Structures;
using LionFire.Assets;
using System.ComponentModel;
using System;
using LionFire.Referencing;
using LionFire.Types;
using LionFire.Copying;
using LionFire.Serialization;

namespace LionFire.Assets
{
    public abstract class RenameableAssetBaseBase<TValue> : AssetBaseBase<TValue>
    {
        public override AssetReference<TValue> Reference
        {
            get => reference;
            set
            {
                reference = (AssetReference<TValue>)value;
            }
        }
    }

    /// <remarks>
    /// AssetPath (Key), TreatAsType, Reference
    /// </remarks>
    /// <typeparam name="TValue"></typeparam>
    public abstract class AssetBaseBase<TValue> : IKeyed, INotifyPropertyChanged, IAsset<TValue>, IReferencable, INotifyReferenceDeserialized<AssetReference<TValue>>
    {
        public Type TreatAsType => typeof(TValue);

        //public string AssetPath { get => Key; set => Key = value; }
        [Ignore]
        public string AssetPath { get => reference?.Path; set => Reference = value; }

        /// <summary>
        /// Implementors should invoke OnKeyChanged
        /// </summary>
        public abstract string Key { get; /*set;*/ }

        /// <summary>
        /// The "file name" portion of the AssetPath (omit the directory)
        /// </summary>
        public string Name => IReferenceExtensions.Name(Reference);

        protected virtual void OnKeyChanged() => reference = null;

        // REVIEW - should only set back to null after a clone.  Is there a good way to enforce/avoid this? 
        // IOnCloned.Cloned interface to set back to null?
        [Assignment(AssignmentMode.Ignore)]
        [Ignore]
        public virtual AssetReference<TValue> Reference
        {
            get => reference;
            set
            {
                if (value != null && reference != null && !value.Equals(reference)) throw new AlreadySetException();
                reference = (AssetReference<TValue>)value;
            }
        }
        protected AssetReference<TValue> reference;
        IAssetReference IReferencable<IAssetReference>.Reference => reference;

        IReference IReferencable.Reference => Reference;

        #region Construction

        protected AssetBaseBase() { }
        protected AssetBaseBase(AssetReference<TValue> reference) { this.reference = reference; }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        public void OnDeserialized(AssetReference<TValue> reference) => this.reference = reference;
    }
}
