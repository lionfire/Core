using LionFire.Persistence.Handles;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos
{
    public class HVob<TValue> : ReadWriteHandlePassthrough<TValue, VobReference>
    {
        public static implicit operator HVob<TValue>(VobReference reference) => new HVob<TValue> { Reference = reference };
        public static implicit operator HVob<TValue>(string vosPath) => new HVob<TValue> { Reference = vosPath };
        public static implicit operator HVob<TValue>(TValue value) => new HVob<TValue> { Reference = (value as IReferencable<VobReference>)?.Reference, Value = value };
    }
}
