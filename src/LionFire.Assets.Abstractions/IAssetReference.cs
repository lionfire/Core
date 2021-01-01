using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Assets
{
    public interface IAssetReference<TValue> : IAssetReference, IReference<TValue> { }
    public interface IAssetReference : ITypedReference
        , IPersisterReference // Persister refers to RootVob name (typically null for default Vob root.)
    {
        // REVIEW: Is this necessary when there is Persister?
        /// <summary>
        /// UNTESTED
        /// </summary>
        string Channel { get; }

        IAssetReference<T> ForType<T>();
    }
}
