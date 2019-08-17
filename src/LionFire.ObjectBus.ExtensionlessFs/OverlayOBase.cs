using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Persistence.Resolution;

namespace LionFire.ObjectBus.ExtensionlessFs
{
    public abstract class OverlayOBase<TOverlayReference, TUnderlyingReference> : OBase<TOverlayReference>, IOBase, IOverlayOBase<TOverlayReference, TUnderlyingReference>
        where TOverlayReference : class, IReference
        where TUnderlyingReference : IReference
    {
        public abstract IOBase UnderlyingOBase { get; }

        public abstract IReferenceToReferenceResolver ReferenceResolutionStrategy { get; }

        #region Underlying

        public async Task<IEnumerable<ReadResolutionResult<TObject>>> GetUnderlyingReferences<TObject>(TOverlayReference overlayReference)
        {
            return await ReferenceResolutionStrategy.ResolveAll<TObject>(overlayReference).ConfigureAwait(false);
        }

        //private async Task<TResult> DoUnderlyingAction<TResult>(IReference reference, Func<IReference, Task<TResult>> action, Func<ValueTuple<PersistenceResultKind, TResult?, IEnumerable<TResult>>, TResult> consolidate, bool useFirstFoundUnderlyingReference = true)
        //    where TResult : struct, IPersistenceResult
        //    => consolidate(await DoUnderlyingAction(reference, action, useFirstFoundUnderlyingReference));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="reference"></param>
        /// <param name="action"></param>
        /// <param name="useFirstFoundUnderlyingReference"></param>
        /// <param name="allowMultipleSuccess"></param>
        /// <param name="validateBeforeAction">Only applies when there is more than one underlying reference</param>
        /// <returns>Null if no underlying references found, otherwise a TResult representing one or multiple successes or failures.</returns>
        private async Task<TResult> DoUnderlyingAction<TResult, TCreate>(TOverlayReference reference, Func<IReference, Task<TResult>> action,
            bool validateBeforeAction = false,
            bool throwOnError = true,
            bool returnOnError = false,
            bool returnOnFirstSuccess = true,
            bool allowMultipleSuccess = false,
            bool requireAllSuccessToSucceed = false
            )
            where TResult : class, IPersistenceResult
            where TCreate : class, TResult, ITieredPersistenceResult, new()
        {
            // OPTIMIZE: Execute underlyings in parallel

            var underlyingResolutions = await GetUnderlyingReferences<TResult>(reference);
            var underlyingCount = underlyingResolutions.Count();

            if (validateBeforeAction && underlyingCount > 1) throw new NotImplementedException("TODO: Validation/pretend actions before committing them");

            List<TResult> failures = null;

            TResult lastResult = default;

            List<TResult> successes = null;
            TResult success = default;

            if (underlyingCount == 0)
            {
                //return TieredRetrieveResult.NotFound;
                return null;
            }

            foreach (var underlyingResolution in underlyingResolutions)
            {
                lastResult = await action(underlyingResolution.Reference);

                if (!lastResult.IsSuccess())
                {
                    if (throwOnError && lastResult.IsFail())
                    {
                        if (throwOnError) throw new PersistenceException(lastResult);
                    }
                    if (failures == null)
                    {
                        failures = new List<TResult>();
                    }
                    failures.Add(lastResult);
                    if (returnOnError) break;
                    continue;
                }

                // else IsSuccess:

                if (returnOnFirstSuccess) return lastResult;
                else
                {
                    if (success == null && successes == null)
                    {
                        success = lastResult;
                    }
                    else
                    {
                        if (!allowMultipleSuccess) throw new AlreadyException("Multiple successes not allowed for this persistence operation");
                        else
                        {
                            if (successes != null)
                            {
                                successes.Add(lastResult);
                            }
                            else // successes == null && success != null
                            {
                                successes = new List<TResult>
                                {
                                    success,
                                    lastResult,
                                };
                                success = null;
                            }
                        }
                    }
                }
            }

            bool isSuccess = (success != null || successes != null) && (!requireAllSuccessToSucceed || failures == null);

            if (isSuccess)
            {
                if (success != null) return success;
            }

            var combined = new TCreate();
            if (isSuccess) combined.Flags = PersistenceResultFlags.Success;
            foreach (var successItem in successes)
            {

                combined.Flags |= successItem.Flags;
                if (successItem is ITieredPersistenceResult tiered && tiered.RelevantUnderlyingCount > 0) combined.RelevantUnderlyingCount++; // Flatten child WriteCount to one per underlying.
            }
            combined.Successes = (IEnumerable<ITieredPersistenceResult>)successes;
            combined.Failures = (IEnumerable<ITieredPersistenceResult>)failures;

            return (TResult)combined;
        }

        #endregion

        #region Read

        #region Exists

        #endregion

        #region Get

        public override async Task<IRetrieveResult<T>> TryGet<T>(TOverlayReference reference)
            => await DoUnderlyingAction<IRetrieveResult<T>, TieredRetrieveResult<T>>(reference, (async underlyingReference =>
            {
                return new OverlayRetrieveResult<T>(await UnderlyingOBase.Get<T>(underlyingReference));
            }));

        Task<IRetrieveResult<T>> IOBase.Get<T>(IReference reference)
        {
            if (reference is TOverlayReference overlayReference)
            {
                return TryGet<T>(overlayReference);
            }
            return Task.FromResult<IRetrieveResult<T>>(RetrieveResult<T>.InvalidReferenceType);
        }

        #endregion

        #region List

        public override Task<IEnumerable<string>> List<T>(TOverlayReference parent) => throw new NotImplementedException();

        #endregion

        #endregion

        #region Write

        #region Set
        protected override async Task<IPersistenceResult> SetImpl<T>(TOverlayReference reference, T obj, bool allowOverwrite = true)
            => await DoUnderlyingAction<IPersistenceResult, TieredPersistenceResult>(reference, (async underlyingReference =>
            {
                return new OverlayPersistenceResult(await UnderlyingOBase.Set<T>(underlyingReference, obj, allowOverwrite: allowOverwrite));
            }), returnOnFirstSuccess: true);


        #endregion

        #region Delete

        public override Task<IPersistenceResult> CanDelete<T>(TOverlayReference reference) => throw new NotImplementedException();
        public override Task<IPersistenceResult> TryDelete<T>(TOverlayReference reference) => throw new NotImplementedException();

        #endregion

        
        //public async Task<IPersistenceResult> TryDelete(IReference reference) 
        //=> await DoUnderlyingAction<IPersistenceResult, TieredPersistenceResult>(reference, underlyingReference => UnderlyingOBase.TryDelete<object>(underlyingReference));

        #endregion

        #region Events

        #endregion
        
    }
}
