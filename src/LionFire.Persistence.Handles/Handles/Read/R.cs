using LionFire.Persistence.Handles;
using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public class R<TValue> : ReadHandlePassthrough<TValue, IReference<TValue>>
    {
        public static implicit operator R<TValue>(string uri) => new R<TValue> { Reference = uri.ToReference<TValue>() };
        public static implicit operator R<TValue>(TValue value) => new R<TValue> { Reference = (value as IReferencableAsValueType<TValue>)?.Reference, Value = value };
    }

    public class R<TValue, TReference> : ReadHandlePassthrough<TValue, TReference>
       where TReference : IReference<TValue>
    {
        public static implicit operator R<TValue, TReference>(TReference reference) 
            => new R<TValue, TReference> { Reference = reference };
        public static implicit operator R<TValue, TReference>(string uri) 
            => new R<TValue, TReference> { Reference = uri.ToReferenceType<TReference>() };
        //public static implicit operator R<TValue, TReference>(TValue value) 
        //=> new R<TValue, TReference> { Reference = ((IReferencable<TReference>)value).Reference, Value = value };


        public static implicit operator R<TValue, TReference>(TValue value)
        {
            if(value is IReferencable<TReference> referencable)
            {
                return new R<TValue, TReference> { Reference = referencable.Reference, Value = value };
            }
            else
            {
                return new R<TValue, TReference> { Reference = default, Value = value };
            }
        }
    }
}
