using LionFire.Ontology;
using LionFire.Resolvables;
using LionFire.Data.Async.Gets;
using LionFire.Results;
using LionFire.Structures;
using MorseCode.ITask;
using System;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Persistence;

namespace LionFire.Data.Async.Gets;

public static class IGetsX2
{

    public static Type GetRetrieveType(this IGets gets)
    {
        var retrievesInterface = gets.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IGets<>)).Single();
        return retrievesInterface.GetGenericArguments()[0];
    }

    /// <summary>
    /// Uses reflection to call IGets&lt;object&gt;.Resolve then upcasts result to IGetResult&lt;object&gt;.  Consider using IGets&lt;object&gt;.Resolve directly.
    /// </summary>
    /// <param name="retrieves"></param>
    /// <returns></returns>
    public static async Task<IGetResult<object>> GetUnknownType(this IGets retrieves)
    {
        var retrievesInterface = retrieves.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IGets<>)).Single();
        return (await ((ITask<IGetResult<object>>)retrievesInterface.GetMethod(nameof(IGets<object>.Get)).Invoke(retrieves, null)).ConfigureAwait(false));
    }

    /////// <summary>
    /////// Force a retrieve of the reference from the source.  Replace the Object.
    /////// </summary>
    /////// <remarks>Can't return a generic IGetResult due to limitation of the language.</remarks>
    /////// <returns>true if an object was retrieved.  False if object was not found at location of the Reference.  Throws if could not resolve the Reference to a valid source.</returns>
    //[Casts("lazilyiGets.ResolveAsync must return IGetResult<object>", typeof(IGetResult<object>))]
    //public static async Task<IGetResult<object>> Retrieve(this IRetrieves lazilyiGets) => (IGetResult<object>) (await ((IGets<object>)lazilyiGets).Resolve().ConfigureAwait(false));

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
        if(getResult is ITransferResult pr) { return pr.Flags; }
        return TransferResultFlags.None;
    }

    // OLD - IGetResult now contains what IRetrieveResult did
    //public static IRetrieveResult<T> ToRetrieveResult<T>(this IGetResult<T> resolveResult)
    //{
    //    if (resolveResult is IGetResult<T> rr) return rr;

    //    TransferResultFlags flags = TransferResultFlags.None;

    //    if (resolveResult.IsSuccess.HasValue)
    //    {
    //        flags |= resolveResult.IsSuccess.Value ? TransferResultFlags.Success : TransferResultFlags.Fail;
    //    }
    //    if (resolveResult.HasValue) flags |= TransferResultFlags.Found;
    //    else if (resolveResult.IsSuccess == true) flags |= TransferResultFlags.NotFound;

    //    return new RetrieveResult<T>(resolveResult.Value, flags);
    //}

    public static ITransferResult ToPersistenceResult(this ISuccessResult successResult)
    {
        if (successResult is ITransferResult pr) return pr;

        TransferResultFlags flags = TransferResultFlags.None;

        if (successResult.IsSuccess.HasValue)
        {
            flags |= successResult.IsSuccess.Value ? TransferResultFlags.Success : TransferResultFlags.Fail;
        }

        return new TransferResult() { Flags = flags };
    }

    //public static async Task<bool> Exists<T>(this ILazilyGets<T> resolves)
    //{
    //}
    public static async Task<bool> Exists<T>(this IGets<T> gets)
    {
        if (gets is ILazilyGets<T> lazilyGets)
        {
            if (lazilyGets is IDetects d) return await d.Exists();

            return (await lazilyGets.GetIfNeeded()).HasValue;
            //return await lazilyGets.Exists();
        }

        return (await gets.Get()).ValidateSuccess().HasValue;
    }
    public static async Task<bool> Exists<T>(this IGets resolves)
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
