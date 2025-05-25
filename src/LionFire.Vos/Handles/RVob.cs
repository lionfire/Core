using LionFire.Persistence.Handles;
using LionFire.Referencing;


namespace LionFire.Vos
{
    public class RVob<TValue> : ReadHandlePassthrough<TValue, VobReference<TValue>>
        where TValue : class
    {
        public static implicit operator RVob<TValue>(VobReference<TValue> reference) => new RVob<TValue> { Reference = reference };
        public static implicit operator RVob<TValue>(string vosPath) => new RVob<TValue> { Reference = vosPath };
        public static implicit operator RVob<TValue>(TValue value) => new RVob<TValue> { Reference = (value as IReferenceable<VobReference<TValue>>)?.Reference, Value = value };
    }
}
