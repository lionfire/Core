using LionFire.Referencing;
using LionFire.Resolvables;
using LionFire.Resolves;
using LionFire.Results;
using System;
using System.IO;

namespace LionFire.Persistence
{
    
    /// <summary>
    /// Returned for Retrieve or ResolveReference operations (which may do a Retrieve).
    /// </summary>
    public interface IRetrieveResult<out T> : IResolveResult<T>, IPersistenceResult
        , ILazyResolveResult<T> // REVIEW - is it correct to have ILazyResolveResult<T> here?
    {
    }

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
            if (!retrieveResult.Flags.HasFlag(PersistenceResultFlags.Found))
            {
                throw new FileNotFoundException();
            }
            return retrieveResult.Value;
        }
    }


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

    //    public static RetrievableState ToRetrievableState<T>(this IRetrieveResult<T> result, bool objectCanBeDefault)
    //    {
    //        RetrievableState retrievableState;
    //        if (result.Flags.HasFlag(PersistenceResultFlags.Success) || result.Flags.HasFlag(PersistenceResultFlags.Found))
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
    //        else if (result.Flags.HasFlag(PersistenceResultFlags.NotFound))
    //        {
    //            retrievableState = RetrievableState.NotFound;
    //        }
    //        else if (result.Flags.HasFlag(PersistenceResultFlags.Fail))
    //        {
    //            retrievableState = RetrievableState.Exception;
    //        }
    //        else if (result.Flags.HasFlag(PersistenceResultFlags.ProviderNotAvailable))
    //        {
    //            retrievableState = RetrievableState.NoRetrieverAvailable;
    //        }
    //        else
    //        {
    //            throw new ArgumentException($"{nameof(IRetrieveResult<T>)}.{nameof(IRetrieveResult<T>.Flags)} does not have one of the expected Flags");
    //        }

    //        return retrievableState;
    //    }
    //}
}