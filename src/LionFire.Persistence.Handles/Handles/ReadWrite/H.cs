using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public class H<TValue> : ReadWriteHandlePassthrough<TValue, IReference>
    {
        public static implicit operator H<TValue>(string uri) => new H<TValue> { Reference = uri.ToReference() };
        public static implicit operator H<TValue>(TValue value) => new H<TValue> { Reference = (value as IReferencable)?.Reference, Value = value };
    }

    public class H<TValue, TReference> : ReadWriteHandlePassthrough<TValue, TReference>
        where TReference : class, IReference
    {
        public static implicit operator H<TValue, TReference>(TReference reference) => new H<TValue, TReference> { Reference = reference };
        public static implicit operator H<TValue, TReference>(string uri) => new H<TValue, TReference> { Reference = uri.ToReference<TReference>() };
        public static implicit operator H<TValue, TReference>(TValue value) => new H<TValue, TReference> { Reference = (value as IReferencable<TReference>)?.Reference, Value = value };
    }
}
