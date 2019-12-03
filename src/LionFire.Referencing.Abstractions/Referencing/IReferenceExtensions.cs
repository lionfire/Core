using LionFire.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LionFire.Referencing
{
    public static class IReferenceExtensions
    {
        public static string Name(this IReference reference) => LionPath.GetName(reference.Path);

        public static IReference<T> OfType<T>(this IReference reference) => new TypedReference<T>(reference);

        #region GetChild

        public static IReference GetChild(this IReference reference, string subPath)
        {
            if (reference is ICloneableReference cr)
            {
                return cr.CloneWithPath(LionPath.Combine(reference.Path, subPath));
            }

            var uri = new Uri(reference.Key);
            var newUri = new Uri(uri, subPath);

            var mi = FromUriMethods[reference.GetType()];
            if (mi != null)
            {
                return (IReference)mi.Invoke(null, new object[] { newUri });
            }

            throw ThrowUnsupported(reference, nameof(GetChild));
        }

        #endregion

        #region GetChildSubpath

        public static IReference GetChildSubpath(this IReference reference, params string[] subPath) => reference.GetChildSubpath((IEnumerable<string>)subPath);

        //[Untested]
        public static IReference GetChildSubpath(this IReference reference, IEnumerable<string> subPath)
        {
            if (reference is ICloneableReference cr)
            {
                return cr.CloneWithPath(LionPath.Combine(reference.Path, subPath));
            }

            var uri = new Uri(reference.Key);
            var newUri = new Uri(uri, subPath.Aggregate((x, y) => x + "/" + y));

            var mi = FromUriMethods[reference.GetType()];
            if (mi != null)
            {
                return (IReference)mi.Invoke(null, new object[] { newUri });
            }

            throw ThrowUnsupported(reference, nameof(GetChildSubpath));
        }

        private static NotSupportedException ThrowUnsupported(IReference reference, string methodName) =>
            new NotSupportedException($"To use {methodName}, reference type ${reference.GetType().FullName} must implement ICloneableReference or have a FromUri method accepting a single parameter of type Uri.");

        #endregion

        #region (Private)

        private static readonly ConcurrentDictionaryCache<Type, MethodInfo> FromUriMethods =
            new ConcurrentDictionaryCache<Type, MethodInfo>(type =>
            type.GetMethod("FromUri", new Type[] { typeof(string) })
            ?? type.GetMethod(type.Name, new Type[] { typeof(string) })
            );

#if FUTURE // Also support FromPath instead of 
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
#endif

        #endregion

        public static NotFoundException NotFoundException(this IReference reference)
        {
            return new NotFoundException($"Could not find {reference.ToString()}")
            {
                Reference = reference,
            };
        }

        /// <summary>
        /// Upcast to ITypedReference and return ITypedReference.Type.  Returns null if none available.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static Type ReferenceType(this IReference reference)
        {
            if (reference is ITypedReference tr && tr.Type != null)
            {
                return tr.Type;
            }
            return null;
        }

    }
}
