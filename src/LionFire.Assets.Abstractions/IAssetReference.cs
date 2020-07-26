using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Assets
{
    public interface IAssetReference : ITypedReference
    {
        string Channel { get; }
    }
}
