using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Data.Id
{
    public interface IIdReference<TValue> : IReference<TValue>, IIdReference
    {
    }
    public interface IIdReference : ITypedReference
    {
        IIdReference<TValue> ForType<TValue>();
    }

}
