using LionFire.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LionFire.Referencing
{

    public static class IReferenceExtensions
    {
        public static IReference<T> OfType<T>(this IReference reference) => new TypedReference<T>(reference);

        public static IReference GetChild(this IReference reference, string subPath)
        {
            if (reference is IReferenceEx2 rex)
            {
                return rex.GetChild(subPath);
            }

            var uri = new Uri(reference.Key);
            var newUri = new Uri(uri, subPath);

            var mi = FromUriMethods[reference.GetType()];
            if (mi != null)
            {
                return (IReference)mi.Invoke(null, new object[] { newUri });
            }

            return null;
        }

        public static IReference GetChildSubpath(this IReference reference, params string[] subPath) => GetChildSubpath(reference, (IEnumerable<string>) subPath);

        //[Untested]
        public static IReference GetChildSubpath(this IReference reference, IEnumerable<string> subPath)
        {
            if (reference is IReferenceEx2 rex)
            {
                return rex.GetChildSubpath(subPath);
            }

            var uri = new Uri(reference.Key);
            var newUri = new Uri(uri, subPath.Aggregate((x,y)=> x + "/" + y));

            var mi = FromUriMethods[reference.GetType()];
            if (mi != null)
            {
                return (IReference)mi.Invoke(null, new object[] { newUri });
            }
            return null;
        }

        #region (Private)

        private static readonly ConcurrentDictionaryCache<Type, MethodInfo> FromUriMethods =
            new ConcurrentDictionaryCache<Type, MethodInfo>(type =>
            type.GetMethod("FromUri", new Type[] { typeof(string) })
            ?? type.GetMethod(type.Name, new Type[] { typeof(string) })
            );

        private static readonly ConcurrentDictionaryCache<Type, Func<IReference, string, IReference>> FromUriMethods2 =
            new ConcurrentDictionaryCache<Type, Func<IReference, string, IReference>>(type =>
            {
                MethodInfo mi;

                if ((mi = type.GetMethod("FromUri", new Type[] { typeof(string) })) != null)
                {
                    return (reference, subPath) =>
                    {
                        var uri = new Uri(reference.Key);
                        var newUri = new Uri(uri, subPath);

                        return (IReference)mi.Invoke(null, new object[] { newUri });
                    };
                }
                else if ((mi = type.GetMethod("FromStringUri", new Type[] { typeof(string) })) != null)
                {
                    return (reference, subPath) =>
                    {
                        var uri = new Uri(reference.Key);
                        var newUri = new Uri(uri, subPath);

                        return (IReference)mi.Invoke(null, new object[] { newUri.ToString() });
                    };
                }
                else if ((mi = type.GetMethod("FromPath", new Type[] { typeof(string) })) != null)
                {
                    return (reference, subPath) =>
                    {
                        var uri = new Uri(reference.Key);
                        var newUri = new Uri(uri, subPath);

                        return (IReference)mi.Invoke(null, new object[] { newUri.AbsolutePath });
                    };
                }
                else
                {
                    return (reference, subPath) =>
                        throw new NotImplementedException($"This reference type ({reference.GetType().Name}) does not implement one of: FromUri FromStringUri FromPath.");
                }
            });

        #endregion

        
    }

}
