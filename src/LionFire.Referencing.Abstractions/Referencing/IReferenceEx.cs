using LionFire.Collections;
using System;
using System.Reflection;

namespace LionFire.Referencing
{

    public static class IReferenceExtensions
    {
        public static IReference<T> OfType<T>(this IReference reference) => new TypedReference<T>(reference);

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
    }

    public interface IOBaseReference : IReferenceWithLocation
    {
#if LEGACY // TOMIGRATE
        /// <summary>
        /// Specialized implementations of IReference may be tied to a particular ObjectStoreProvider: Eg. FileReference may be tied to FilesystemObjectStoreProvider
        /// </summary>
        IOBaseProvider DefaultObjectStoreProvider { get; }
#endif
    }

    public interface IReferenceEx : IReference
    //#if AOT
    // IROStringKeyed
    //#else
    //IKeyed<string>
    //#endif
    {

        string Uri { get; }

        string Name { get; }


        //string Dimension { get; set; } // What is this???  Package or something

        /// <summary>
        /// REVIEW: Consider making this bool? and returning null if host unspecified
        /// </summary>
        bool IsLocalhost
        {
            get;
        }

        ///// <summary>
        ///// For Reference types that are aliases to other References, this will return the target.
        ///// This is invoked by the HandleFactory.
        ///// </summary>
        ///// <returns></returns>
        //IReference Resolve();

//#if !AOT
//        IHandle<T> GetHandle<T>(T obj = null)
//            where T : class;//, new();
//#endif
    }
}
