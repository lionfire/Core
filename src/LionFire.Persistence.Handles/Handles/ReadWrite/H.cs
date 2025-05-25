using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public class H<TValue> : ReadWriteHandlePassthrough<TValue, IReference<TValue>>
    {
        public static implicit operator H<TValue>(string uri) 
            => new H<TValue> { Reference = uri.ToReference<TValue>() };
        public static implicit operator H<TValue>(TValue value) 
            => new H<TValue> { Reference = (value as IReferenceableAsValueType<TValue>)?.Reference, Value = value };
    }

    public class H<TValue, TReference> : ReadWriteHandlePassthrough<TValue, TReference>
        where TReference : IReference<TValue>
    {
        public static implicit operator H<TValue, TReference>(TReference reference) => new H<TValue, TReference> { Reference = reference };
        public static implicit operator H<TValue, TReference>(string uri) => new H<TValue, TReference> { Reference = uri.ToReferenceType<TReference>() };

        public static implicit operator H<TValue, TReference>(TValue value)
            => value is IReferenceable<TReference> referencable
                ? new H<TValue, TReference> { Reference = referencable.Reference, Value = value }
                : new H<TValue, TReference> { Reference = default, Value = value };
    }
}
