using LionFire.Persistence.Handles;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos
{
    public class HVob<TValue> : ReadWriteHandlePassthrough<TValue, VosReference>
    {
        public static implicit operator HVob<TValue>(VosReference reference) => new HVob<TValue> { Reference = reference };
        public static implicit operator HVob<TValue>(string vosPath) => new HVob<TValue> { Reference = vosPath };
        public static implicit operator HVob<TValue>(TValue value) => new HVob<TValue> { Reference = (value as IReferencable<VosReference>)?.Reference, Value = value };
    }
}
