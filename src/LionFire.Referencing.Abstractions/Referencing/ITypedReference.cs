using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{
    public interface ITypedReference : IReference
    {
        Type Type { get; }
    }

}
