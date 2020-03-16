using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public class W<TValue> : ReadWriteHandlePassthrough<TValue, IReference>
    {
        public static implicit operator W<TValue>(string uri) => new W<TValue> { Reference = uri.ToReference() };
        public static implicit operator W<TValue>(TValue value) => new W<TValue> { Reference = (value as IReferencable)?.Reference, Value = value };
    }

    public class W<TValue, TReference> : WriteHandlePassthrough<TValue, TReference>
       where TReference : class, IReference
    {
        public static implicit operator W<TValue, TReference>(TReference reference) => new W<TValue, TReference> { Reference = reference };
        public static implicit operator W<TValue, TReference>(string uri) => new W<TValue, TReference> { Reference = uri.ToReference<TReference>() };
        public static implicit operator W<TValue, TReference>(TValue value) => new W<TValue, TReference> { Reference = (value as IReferencable<TReference>)?.Reference, Value = value };
    }
}
