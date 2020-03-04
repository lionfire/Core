using System;
using LionFire.Extensions.DefaultValues;

namespace LionFire.Referencing
{
    public static class ReferenceObjectFactory
    {

        private static ObjectType CreateDefault<ObjectType>(IReference reference, bool applyDefaultValues = true)
        {
            ObjectType result;
            if (typeof(ObjectType) == typeof(object))
            {
                if (!(reference is ITypedReference typedReference))
                {
                    throw new ArgumentException("reference must be a ITypedReference when using a non-generic Handle, or when the generic type is object.");
                }

                if (typedReference.Type == null)
                {
                    throw new ArgumentException("Reference.Type must be set when using non-generic Handle, or when the generic type is object.");
                }
                result = (ObjectType)Activator.CreateInstance(typedReference.Type);
            }
            else
            {
                result = (ObjectType)Activator.CreateInstance(typeof(ObjectType));
            }
            if (applyDefaultValues) { DefaultValueUtils.ApplyDefaultValues(result); }
            return result;
        }

        public static ObjectType ConstructDefault<ObjectType>(IReference reference, bool applyDefaultValues = true)
            => CreateDefault<ObjectType>(reference, applyDefaultValues);

    }
}
