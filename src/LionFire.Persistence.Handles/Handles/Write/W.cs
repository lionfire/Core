using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public class W<TValue> : ReadWriteHandlePassthrough<TValue, IReference<TValue>>
    {
        public static implicit operator W<TValue>(string uri) => new W<TValue> { Reference = uri.ToReference<TValue>() };
        public static implicit operator W<TValue>(TValue value) => new W<TValue> { Reference = (value as IReferenceableAsValueType<TValue>)?.Reference, Value = value };
    }

    public class W<TValue, TReference> : WriteHandlePassthrough<TValue, TReference>
       where TReference : IReference<TValue>
    {
        public static implicit operator W<TValue, TReference>(TReference reference) => new W<TValue, TReference> { Reference = reference };
        public static implicit operator W<TValue, TReference>(string uri) => new W<TValue, TReference> { Reference = uri.ToReferenceType<TReference>() };

        public static implicit operator W<TValue, TReference>(TValue value)
            => value is IReferenceable<TReference> referencable
                ? new W<TValue, TReference> { Reference = referencable.Reference, Value = value }
                : new W<TValue, TReference> { Reference = default, Value = value };
    }
}
