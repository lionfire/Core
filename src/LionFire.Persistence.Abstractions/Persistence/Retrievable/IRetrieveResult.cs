using LionFire.Referencing;
using LionFire.Resolvables;
using LionFire.Data.Async.Gets;
using LionFire.Results;
using System;
using System.IO;
using LionFire.Data;

namespace LionFire.Persistence;

// OLD
///// <summary>
///// Returned for Retrieve or ResolveReference operations (which may do a Retrieve).
///// </summary>
//[Obsolete("TODO - Use IGetResult, and get PersistenceFlags through extension method that casts to ITransferResult")]
//public interface IRetrieveResult<out T> : IGetResult<T> 
//{
//}

#if UNUSED
public static class IRetrieveResultExtensions
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="retrieveResult"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException">If not found</exception>
    public static TValue ValueOrThrow<TValue>(this IRetrieveResult<TValue> retrieveResult)
    {
        if (!retrieveResult.Flags.HasFlag(TransferResultFlags.Found))
        {
            throw new FileNotFoundException();
        }
        return retrieveResult.Value;
    }
}
#endif


//public struct IPersistableSnapshot<T>
//{
//    public T Object { get; set; }
//    public PersistenceState State { get; set; }
//    public object Error { get; set; }
//}

// OLD
///// <summary>
///// See also: IRetrieveHandleResult, which uses a (caching/shareable) handle instead.
///// </summary>
///// <typeparam name="TObject"></typeparam>
///// <remarks>Doesn't use covariance because Tasks don't support covariance yet.</remarks>
//public interface IRetrieveResult<TObject> : IRetrieveResult
//{
//    TObject Object { get; }
//}

//public static class RetrievableStateExtensions
//{

//    public static RetrievableState ToRetrievableState<T>(this IGetResult<T> result, bool objectCanBeDefault)
//    {
//        RetrievableState retrievableState;
//        if (result.Flags.HasFlag(TransferResultFlags.Success) || result.Flags.HasFlag(TransferResultFlags.Found))
//        {
//            if (objectCanBeDefault)
//            {
//                retrievableState = (result.Object == default) ? RetrievableState.RetrievedObject : RetrievableState.RetrievedNullOrDefault;
//            }
//            else
//            {
//                retrievableState = result.Object == default ? RetrievableState.RetrievedObject : RetrievableState.NotFound;
//            }
//        }
//        else if (result.Flags.HasFlag(TransferResultFlags.NotFound))
//        {
//            retrievableState = RetrievableState.NotFound;
//        }
//        else if (result.Flags.HasFlag(TransferResultFlags.Fail))
//        {
//            retrievableState = RetrievableState.Exception;
//        }
//        else if (result.Flags.HasFlag(TransferResultFlags.ProviderNotAvailable))
//        {
//            retrievableState = RetrievableState.NoRetrieverAvailable;
//        }
//        else
//        {
//            throw new ArgumentException($"{nameof(IGetResult<T>)}.{nameof(IRetrieveResult<T>.Flags)} does not have one of the expected Flags");
//        }

//        return retrievableState;
//    }
//}