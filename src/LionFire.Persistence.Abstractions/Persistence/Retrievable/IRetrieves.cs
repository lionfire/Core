using LionFire.Ontology;
using LionFire.Resolvables;
using LionFire.Resolves;
using LionFire.Structures;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    /// <summary>
    /// An interface for directly initiating read-related persistence operations of single objects:
    ///  - Retrieve
    ///  - Exists
    /// 
    /// This is only a marker interface.  IRetrieves&lt;T&gt; should also be implemented.  There is a Retrieve extension method
    /// for this marker interface.
    /// 
    /// Common peer interfaces: IDetects, ILazilyRetrieves&lt;T&gt;
    /// </summary>
    public interface IRetrieves //, IResolvesCovariant<object> // IResolves
    {
        // For a Retrieve method, see IRetrievesExtensions.Retrieve
    }


    public interface IRetrieves<out T> : IRetrieves, IResolves<T>, IDefaultableReadWrapper<T> { }

    //public interface IRetrievesCovariant<out T> : IRetrieves, IResolves<T>, IReadWrapper<T>, IWrapper { }


    public static class IRetrievesExtensions
    {
        /////// <summary>
        /////// Force a retrieve of the reference from the source.  Replace the Object.
        /////// </summary>
        /////// <remarks>Can't return a generic IRetrieveResult due to limitation of the language.</remarks>
        /////// <returns>true if an object was retrieved.  False if object was not found at location of the Reference.  Throws if could not resolve the Reference to a valid source.</returns>
        //[Casts("retrieves.ResolveAsync must return IRetrieveResult<object>", typeof(IRetrieveResult<object>))]
        //public static async Task<IRetrieveResult<object>> Retrieve(this IRetrieves retrieves) => (IRetrieveResult<object>) (await ((IResolves<object>)retrieves).Resolve().ConfigureAwait(false));

        ///// <summary>
        ///// Force a retrieve of the reference from the source.  Replace the Object.
        ///// </summary>
        ///// <remarks>Can't return a generic IRetrieveResult due to limitation of the language.</remarks>
        ///// <returns>true if an object was retrieved.  False if object was not found at location of the Reference.  Throws if could not resolve the Reference to a valid source.</returns>
        [Casts("retrieves.ResolveAsync must return IRetrieveResult<T>", typeof(IRetrieveResult<>))]
        [Obsolete("TODO - use ToRetrieveResult instead")]
        public static async Task<IRetrieveResult<T>> Retrieve<T>(this IRetrieves<T> retrieves) => (IRetrieveResult<T>)await retrieves.Resolve().ConfigureAwait(false); // CAST

        public static IRetrieveResult<T> ToRetrieveResult<T>(this IResolveResult<T> resolveResult)
        {
            if (resolveResult is IRetrieveResult<T> rr) return rr;

            PersistenceResultFlags flags = PersistenceResultFlags.None;

            if (resolveResult.IsSuccess.HasValue)
            {
                flags |= resolveResult.IsSuccess.Value ? PersistenceResultFlags.Success : PersistenceResultFlags.Fail;
            }
            if (resolveResult.HasValue) flags |= PersistenceResultFlags.Found;
            else if (resolveResult.IsSuccess == true) flags |= PersistenceResultFlags.NotFound;

            return new RetrieveResult<T>(resolveResult.Value, flags);
        }

        public static async Task<bool> Exists<T>(this ILazilyResolves<T> resolves)
        {
            if (resolves is IDetects d) return await d.Exists();

            return (await resolves.GetValue()).HasValue;
        }
        public static async Task<bool> Exists<T>(this IResolves<T> resolves)
        {
            if (resolves is ILazilyResolves<T> lazilyResolves) return await lazilyResolves.Exists();

            return (await resolves.Resolve()).HasValue;
        }
    }
}
