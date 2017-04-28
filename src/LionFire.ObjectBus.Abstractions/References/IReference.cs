using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Collections;
using LionFire.Structures;

namespace LionFire.ObjectBus
{

    public interface IReference :
#if AOT
 IROStringKeyed
#else
IKeyed<string>
#endif
    {
        /// <summary>
        /// Specialized implementations of IReference may be tied to a particular ObjectStoreProvider: Eg. FileReference may be tied to FilesystemObjectStoreProvider
        /// </summary>
        IOBaseProvider DefaultObjectStoreProvider { get; }

        string Uri { get; }
        string Scheme { get; }
        string Host { get; }
        string Port { get; }
        string Path { get; }
        string Name { get; }
        string Package { get; }

        /// <summary>
        /// Specifies OBaseProvider-specific detail about location. E.g. for VosReference, specifies a particular store name of a mount.
        /// For filesystem, this is ignored.  For DbReference, this specifies the database name.
        /// </summary>
        string Location { get; }

        //string Dimension { get; set; } // What is this???  Package or something

        string TypeName { get; }
        Type Type { get; }

        bool IsLocalhost
        {
            get;
        }

        /// <summary>
        /// Clones the reference and appends the path with the specified child name
        /// </summary>
        /// <param name="childName"></param>
        /// <returns></returns>
        IReference GetChild(string subPath);
        //IReference GetChildSubpath(params string[] subpath);
        IReference GetChildSubpath(IEnumerable<string> subpath);


        ///// <summary>
        ///// For Reference types that are aliases to other References, this will return the target.
        ///// This is invoked by the HandleFactory.
        ///// </summary>
        ///// <returns></returns>
        //IReference Resolve();

#if !AOT
        IHandle<T> GetHandle<T>(T obj = null)
            where T : class;//, new();
#endif
    }


    public static class IReferenceExtenensions
    {
        public static IReference GetChildSubpath(this IReference reference, params string[] subpath)
        {
            return reference.GetChildSubpath((IEnumerable<string>)subpath);
        }
    }
}
