using LionFire.Ontology;
using LionFire.Resolvables;
using LionFire.Resolves;
using LionFire.Results;
using LionFire.Structures;
using MorseCode.ITask;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Persistence;

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
public interface IRetrieves : IResolves //, IResolvesCovariant<object>
{
    // For a Retrieve method, see IRetrievesX.Retrieve
}


public interface IRetrieves<out T> : IRetrieves, IResolves<T>, IDefaultableReadWrapper<T> { }

//public interface IRetrievesCovariant<out T> : IRetrieves, IResolves<T>, IReadWrapper<T>, IWrapper { }


public static class IRetrievesX
{

    public static Type GetRetrieveType(this IRetrieves retrieves)
    {
        var retrievesInterface = retrieves.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IRetrieves<>)).Single();
        return retrievesInterface.GetGenericArguments()[0];
    }

    /// <summary>
    /// Uses reflection to call IResolves&lt;object&gt;.Resolve then upcasts result to IResolveResult&lt;object&gt;.  Consider using IResolves&lt;object&gt;.Resolve directly.
    /// </summary>
    /// <param name="retrieves"></param>
    /// <returns></returns>
    public static async Task<IRetrieveResult<object>> Retrieve(this IRetrieves retrieves)
    {
        var retrievesInterface = retrieves.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IResolves<>)).Single();
        return (await ((ITask<IResolveResult<object>>)retrievesInterface.GetMethod(nameof(IResolves<object>.Resolve)).Invoke(retrieves, null)).ConfigureAwait(false)).ToRetrieveResult();
    }

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
    public static async Task<IRetrieveResult<T>> Retrieve<T>(this IRetrieves<T> retrieves) => (IRetrieveResult<T>)await IResolvesX.Resolve(retrieves).ConfigureAwait(false); // CAST

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

    public static IPersistenceResult ToPersistenceResult(this ISuccessResult successResult)
    {
        if (successResult is IPersistenceResult pr) return pr;

        PersistenceResultFlags flags = PersistenceResultFlags.None;

        if (successResult.IsSuccess.HasValue)
        {
            flags |= successResult.IsSuccess.Value ? PersistenceResultFlags.Success : PersistenceResultFlags.Fail;
        }

        return new PersistenceResult() { Flags = flags };
    }

    //public static async Task<bool> Exists<T>(this ILazilyResolves<T> resolves)
    //{
    //}
    public static async Task<bool> Exists<T>(this IResolves<T> resolves)
    {
        if (resolves is ILazilyResolves<T> lazilyResolves)
        {
            if (lazilyResolves is IDetects d) return await d.Exists();

            return (await lazilyResolves.TryGetValue()).HasValue;
            //return await lazilyResolves.Exists();
        }

        return (await IResolvesX.Resolve(resolves)).HasValue;
    }
    public static async Task<bool> Exists<T>(this IResolves resolves)
    {
        if (resolves is ILazilyResolves<T> lazilyResolves)
        {
            if (lazilyResolves is IDetects d) return await d.Exists();

            return (await lazilyResolves.TryGetValue()).HasValue;
            //return await lazilyResolves.Exists();
        }

        throw new NotSupportedException("TODO: Resolve<T>");
        //return (await IResolvesExtensions.Resolve<T>(resolves)).HasValue;
    }
}
