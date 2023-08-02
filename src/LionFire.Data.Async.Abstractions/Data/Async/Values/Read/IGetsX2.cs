using LionFire.Ontology;
using LionFire.Resolvables;
using LionFire.Data.Gets;
using LionFire.Results;
using LionFire.Structures;
using MorseCode.ITask;
using System;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Persistence;
using LionFire.Data;

namespace LionFire.ExtensionMethods.Data;

public static class IGetsX2
{

    public static Type GetRetrieveType(this IStatelessGets gets)
    {
        var retrievesInterface = gets.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IStatelessGets<>)).Single();
        return retrievesInterface.GetGenericArguments()[0];
    }

    /// <summary>
    /// Uses reflection to call IGets&lt;object&gt;.Resolve then upcasts result to IGetResult&lt;object&gt;.  Consider using IGets&lt;object&gt;.Resolve directly.
    /// </summary>
    /// <param name="retrieves"></param>
    /// <returns></returns>
    public static async Task<IGetResult<object>> GetUnknownType(this IStatelessGets retrieves)
    {
        var retrievesInterface = retrieves.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IStatelessGets<>)).Single();
        return (await ((ITask<IGetResult<object>>)retrievesInterface.GetMethod(nameof(IStatelessGets<object>.Get)).Invoke(retrieves, null)).ConfigureAwait(false));
    }

    /////// <summary>
    /////// Force a retrieve of the reference from the source.  Replace the Object.
    /////// </summary>
    /////// <remarks>Can't return a generic IGetResult due to limitation of the language.</remarks>
    /////// <returns>true if an object was retrieved.  False if object was not found at location of the Reference.  Throws if could not resolve the Reference to a valid source.</returns>
    //[Casts("lazilyiGets.ResolveAsync must return IGetResult<object>", typeof(IGetResult<object>))]
    //public static async Task<IGetResult<object>> Retrieve(this IRetrieves lazilyiGets) => (IGetResult<object>) (await ((IGets<object>)lazilyiGets).Get().ConfigureAwait(false));

    ///// <summary>
    ///// Force a retrieve of the reference from the source.  Replace the Object.
    ///// </summary>
    ///// <remarks>Can't return a generic IGetResult due to limitation of the language.</remarks>
    ///// <returns>true if an object was retrieved.  False if object was not found at location of the Reference.  Throws if could not resolve the Reference to a valid source.</returns>
    [Casts("retrieves.ResolveAsync must return IGetResult<T>", typeof(IGetResult<>))]
    [Obsolete("TODO - use ToRetrieveResult instead")]
    public static async Task<IGetResult<T>> Retrieve<T>(this ILazilyGets<T> lazilyGets) => (IGetResult<T>)await IGetsX2.GetUnknownType(lazilyGets).ConfigureAwait(false); // CAST

    public static TransferResultFlags GetTransferResultFlags(this ITransferResult getResult)
    {
        if (getResult is ITransferResult pr) { return pr.Flags; }
        return TransferResultFlags.None;
    }

    public static bool Exists<T>(this ILazyGetResult<T> r)
    {
        return r.Flags.HasFlag(TransferResultFlags.Found); // REVIEW: Ensure Found is always set in all relevant scenarios
    }

    // TODO: REVIEW all these Exists methods for correctness and optimality
    public static async Task<bool> Exists<T>(this ILazilyGets<T> lazilyGets) 
    {
        if (lazilyGets is ILazilyDetects<T> ld) return (await ld.TryGetExistsWithValue().ConfigureAwait(false)).Exists<T>();

        var queryValue = lazilyGets.QueryValue();
        if (queryValue != null) return queryValue.Exists<T>();

        if (lazilyGets is IDetects d) return await d.Exists().ConfigureAwait(false);

        return (await lazilyGets.GetIfNeeded()).HasValue;
        //return await lazilyGets.Exists();
    }
    public static async Task<bool> Exists<T>(this IStatelessGets<T> gets)
    {
        if (gets is ILazilyGets<T> lazilyGets)
        {
            return await Exists(lazilyGets).ConfigureAwait(false);
        }

        return (await gets.Get()).ValidateSuccess().HasValue;
    }
    public static async Task<bool> Exists<T>(this IStatelessGets resolves)
    {
        if (resolves is ILazilyGets<T> lazilyResolves)
        {
            if (lazilyResolves is IDetects d) return await d.Exists();

            return (await lazilyResolves.GetIfNeeded()).HasValue;
            //return await lazilyGets.Exists();
        }

        throw new NotSupportedException("TODO: Resolve<T>");
        //return (await IResolvesExtensions.Resolve<T>(resolves)).HasValue;
    }
}
