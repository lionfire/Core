using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Data.Id
{
    public interface IIdReference : ITypedReference
    {
    }

    public interface IIdReference<TValue> : ITypedReference, IReference<TValue>
    {
    }
}
