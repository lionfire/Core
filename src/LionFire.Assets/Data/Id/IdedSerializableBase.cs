#nullable enable

using LionFire.Serialization;
using LionFire.Structures;
using LionFire.Referencing;
using System;
using LionFire.Types;

namespace LionFire.Data.Id
{
    

    // RENAME AssetSerializableBase, rename Asset* to ChanneledAsset*
    public class IdedSerializableBase<T> : IIded<string>, INotifyReferenceDeserialized<IdReference<T>>, ITreatAsType
    {
        public Type TreatAsType => typeof(T);

        #region Construction

        public IdedSerializableBase() { this.id = null; }
        public IdedSerializableBase(string id) { this.id = id; }

        #endregion

        #region Id

        [Ignore(LionSerializeContext.All)]
        [SetOnce]
        public string? Id
        {
            get => id;
            protected set
            {
                if (id == value) return;
                if (id != default) throw new AlreadySetException();
                id = value;
            }
        }
        protected string? id;

        #endregion
        
        public void OnDeserialized(IReference reference) => id = (reference as IdReference<T>)?.Id ?? OnMissingIdOnDeserialization();

        protected string OnMissingIdOnDeserialization()
        {
            throw new Exception("Missing Id on Deserialization.  OnDeserialized should receive an IdReference<T> with non-null Id.");
            //return null;
        }

        public void OnDeserialized(IdReference<T> reference) => this.Id = reference.Id;
    }
}

