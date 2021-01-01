using LionFire.Persistence.Handles;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos
{
    public class RVob<TValue> : ReadHandlePassthrough<TValue, VobReference<TValue>>
    {
        public static implicit operator RVob<TValue>(VobReference<TValue> reference) => new RVob<TValue> { Reference = reference };
        public static implicit operator RVob<TValue>(string vosPath) => new RVob<TValue> { Reference = vosPath };
        public static implicit operator RVob<TValue>(TValue value) => new RVob<TValue> { Reference = (value as IReferencable<VobReference<TValue>>)?.Reference, Value = value };
    }
}
