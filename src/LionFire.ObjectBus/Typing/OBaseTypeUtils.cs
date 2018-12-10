using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Referencing;

namespace LionFire.ObjectBus.Typing
{
    public static class OBaseTypeUtils
    {

        public static IEnumerable<string> GetEncapsulatedTypeNames(Type ResultType)
        {
            yield return ResultType.Name;
            yield return ResultType.FullName;
        }

        public static IEnumerable<Func<string, string>> EncapsulatedFileNameConverters
        {
            get
            {
                yield return x => "(" + x + ")";
                //yield return x => "_" + x;
                //yield return x => x + ".";
                //yield return x => "." + x; // No, means hidden
            }
        }


        public static IEnumerable<TReference> GetEncapsulatedPaths<TReference>(TReference reference, Type ResultType)
            where TReference : IReference
        {
            foreach (var typeName in GetEncapsulatedTypeNames(ResultType))
            {
                foreach (var fileName in EncapsulatedFileNameConverters.Select(converter => converter(typeName)))
                {
                    yield return (TReference)reference.GetChild(fileName);
                }
            }
        }

        public static object TryConvertToType(object obj, Type ResultType) 
        {
            //                ResultType result = obj as ResultType;
            //                if (obj != null && result == null)

            if (obj != null && !ResultType.IsAssignableFrom(obj.GetType()))
            {
                //l.Debug("Retrieved object of type '" + obj.GetType().FullName + "' but it cannot be cast to the desired type: " + ResultType.FullName);
                return null;
            }
            return obj;
        }
    }
}
