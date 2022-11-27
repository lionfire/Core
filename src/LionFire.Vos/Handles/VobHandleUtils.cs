using System;
using LionFire.Referencing;

namespace LionFire.Vos
{
    public static class VobHandleUtils
    {
        public static void ValidateReference<T>(this IReference reference)
        {
            var typedReference = reference as ITypedReference;

            if (typedReference.Type != null)
            {
                // TODO FIXME: Move this check into reference.ToReadHandle / VobHandle ctor

                //if (!reference.Type.IsAssignableFrom(typeof(T)))
                //{
                //    throw new ArgumentException("!reference.Type.IsAssignableFrom(typeof(ResultType))");
                //}
                if (!typeof(T).IsAssignableFrom(typedReference.Type))
                {
                    throw new ArgumentException("!typeof(TValue).IsAssignableFrom(reference.Type))");
                }
            }
        }

    }
}
