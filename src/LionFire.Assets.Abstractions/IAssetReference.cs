using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Assets
{
    public interface IAssetReference : ITypedReference, IPersisterReference
    {
        // REVIEW: Is this necessary when there is Persister?
        string Channel { get; } 
    }
}
