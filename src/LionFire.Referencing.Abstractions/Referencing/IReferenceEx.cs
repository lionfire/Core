using LionFire.Referencing.Resolution;
using System.Collections.Generic;

namespace LionFire.Referencing
{
    // REVIEW File

    public interface IResolvingReference
    {
        IHandleResolver HandleResolver { get; }
    }

    public interface IReferenceEx2 : IReference
    {
        /// <summary>
        /// Clones the reference and appends the path with the specified child name
        /// </summary>
        /// <param name="childName"></param>
        /// <returns></returns>
        IReference GetChild(string subPath);
        //IReference GetChildSubpath(params string[] subpath);
        IReference GetChildSubpath(IEnumerable<string> subpath);
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

#if !AOT
        IHandle<T> GetHandle<T>(T obj = null)
            where T : class;//, new();
#endif
    }
}
