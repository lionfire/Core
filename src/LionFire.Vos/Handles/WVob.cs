using LionFire.Persistence.Handles;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos
{
    public class WVob<TValue> : WriteHandlePassthrough<TValue, VosReference>
    {
        public static implicit operator WVob<TValue>(VosReference reference) => new WVob<TValue> { Reference = reference };
        public static implicit operator WVob<TValue>(string vosPath) => new WVob<TValue> { Reference = vosPath };
        public static implicit operator WVob<TValue>(TValue value) => new WVob<TValue> { Reference = (value as IReferencable<VosReference>)?.Reference, Value = value };
    }
}
