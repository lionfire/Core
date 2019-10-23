using LionFire.Ontology;
using LionFire.Resolvables;
using LionFire.Resolves;
using LionFire.Structures;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    public interface IRetrieves<out T> : IRetrieves, IReadWrapper<T>, IWrapper { }

    /// <summary>
    /// An interface for directly initiating read-related persistence operations of single objects:
    ///  - Retrieve
    ///  - Exists
    /// </summary>
    public interface IRetrieves : IResolves, IDetects
    {
        // For a Retrieve method, see IRetrievesExtensions.Retrieve
    }

    public static class IRetrievesExtensions
    {
        ///// <summary>
        ///// Force a retrieve of the reference from the source.  Replace the Object.
        ///// </summary>
        ///// <remarks>Can't return a generic IRetrieveResult due to limitation of the language.</remarks>
        ///// <returns>true if an object was retrieved.  False if object was not found at location of the Reference.  Throws if could not resolve the Reference to a valid source.</returns>
        [Casts("retrieves.ResolveAsync must return IRetrieveResult<object>", typeof(IRetrieveResult<object>))]
        public static async Task<IRetrieveResult<object>> Retrieve(this IRetrieves retrieves) => (IRetrieveResult<object>) await retrieves.Resolve();

        ///// <summary>
        ///// Force a retrieve of the reference from the source.  Replace the Object.
        ///// </summary>
        ///// <remarks>Can't return a generic IRetrieveResult due to limitation of the language.</remarks>
        ///// <returns>true if an object was retrieved.  False if object was not found at location of the Reference.  Throws if could not resolve the Reference to a valid source.</returns>
        [Casts("retrieves.ResolveAsync must return IRetrieveResult<T>", typeof(IRetrieveResult<>))]
        public static async Task<IRetrieveResult<T>> Retrieve<T>(this IRetrieves retrieves) => (IRetrieveResult<T>)await retrieves.Resolve();
    }
}
