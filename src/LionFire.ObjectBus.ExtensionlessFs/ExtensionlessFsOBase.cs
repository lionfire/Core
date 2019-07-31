using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using LionFire.DependencyInjection;
using LionFire.ObjectBus.Filesystem;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Referencing.Persistence;
using LionFire.Structures;

namespace LionFire.ObjectBus.ExtensionlessFs
{
    public interface IOBaseOverlay<TOverlayReference, TUnderlyingReference> : IOverlayOBase
    {
        IReferenceToReferenceResolver ReferenceResolutionStrategy { get; }
        Task<IEnumerable<ReadResolutionResult<TObject>>> GetUnderlyingReferences<TObject>(TOverlayReference overlayReference);
    }

    public abstract class OBaseOverlay<TOverlayReference, TUnderlyingReference> : IOBase, IOBaseOverlay<TOverlayReference, TUnderlyingReference>
        where TOverlayReference : IReference
        where TUnderlyingReference : IReference
    {
        public abstract IOBase UnderlyingOBase { get; }

        public abstract IReferenceToReferenceResolver ReferenceResolutionStrategy { get; }

        #region IOBase Implementation

        public abstract IOBus OBus { get; }

        public abstract IEnumerable<string> UriSchemes { get; }

        public Task<bool?> CanDelete(IReference reference) => throw new NotImplementedException();
        public Task<IRetrieveResult<bool>> Exists(IReference reference) => throw new NotImplementedException();
        public IEnumerable<string> GetChildrenNames(IReference parent) => throw new NotImplementedException();
        public IEnumerable<string> GetChildrenNamesOfType<T>(IReference parent) where T : class, new() => throw new NotImplementedException();
        public IEnumerable<string> GetChildrenNamesOfType(IReference parent, Type type) => throw new NotImplementedException();
        public Task<IEnumerable<string>> GetKeys(IReference parent) => throw new NotImplementedException();
        public Task<IEnumerable<string>> GetKeysOfType<T>(IReference parent) where T : class, new() => throw new NotImplementedException();
        public Task<IEnumerable<string>> GetKeysOfType(IReference parent, Type type) => throw new NotImplementedException();
        public IObjectWatcher GetObjectWatcher(IReference reference) => throw new NotImplementedException();
        public Task Set(IReference reference, object obj, Type type = null, bool allowOverwrite = true) => throw new NotImplementedException();
        public IObservable<OBasePersistenceEvent> Subscribe(Predicate<IReference> filter = null, PersistenceEventSourceKind sourceType = PersistenceEventSourceKind.Unspecified) => throw new NotImplementedException();
        public PersistenceEventKind SupportsEvents(PersistenceEventSourceKind sourceType = PersistenceEventSourceKind.Unspecified) => throw new NotImplementedException();

        public async Task<bool?> TryDelete(IReference reference) 
            => await DoUnderlyingAction<RetrieveResult<T>>(reference, underlyingReference => UnderlyingOBase.TryDelete<T>(underlyingReference));

        public async Task<RetrieveResult<T>> TryGet<T>(TOverlayReference reference) 
            => await DoUnderlyingAction<RetrieveResult<T>>(reference, underlyingReference => UnderlyingOBase.TryGet<T>(underlyingReference));

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
        private async Task<TResult> DoUnderlyingAction<TResult>(TOverlayReference reference, Func<IReference, Task<TResult>> action, 
            bool validateBeforeAction = false, 
            bool throwOnError = true, 
            bool returnOnError = false, 
            bool returnOnFirstSuccess = true, 
            bool allowMultipleSuccess = false, 
            bool requireAllSuccessToSucceed = false
            )
            where TResult : struct, IPersistenceResult
        {
            // OPTIMIZE: Execute underlyings in parallel

            var underlyingReferences = await GetUnderlyingReferences<TResult>(reference);
            var underlyingCount = underlyingReferences.Count();

            if (validateBeforeAction && underlyingCount > 1) throw new NotImplementedException("TODO: Validation/pretend actions before committing them");


            List<TResult> failures = null;

            TResult lastResult = null;

            List<TResult> successes = null;
            TResult? success = null;

            if (underlyingCount == 0)
            {
                return null; // Should be interpreted as NotFound
            }

            foreach (var underlyingReference in underlyingReferences)
            {
                lastResult = await action(reference);

                if (!lastResult.IsSuccess())
                {
                    if (throwOnError && lastResult.IsError())
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
                            else
                            {
                                successes = new List<TResult>();
                                successes.Add(success);
                                success = null;
                                successes.Add(lastResult);
                            }
                        }
                    }
                }
            }

            bool isSuccess = (success != null || successes != null) && (!requireAllSuccessToSucceed || failures == null);

            if (isSuccess)
            {
                if (success != null) return success.Value;
            }

            var combined = new TResult();
            if (isSuccess) combined.Kind = PersistenceResultKind.Success;
            foreach (var successItem in successes)
            {
                combined.Kind |= successItem.Kind;
                combined.WriteCount += successItem.WriteCount;
            }
            combined.Successes = (IEnumerable<IPersistenceResult>)successes;
            combined.Failures = (IEnumerable<IPersistenceResult>)failures;

            return combined;
        }

        #endregion

        public async Task<IEnumerable<ReadResolutionResult<TObject>>> GetUnderlyingReferences<TObject>(TOverlayReference overlayReference)
        {
            return await ReferenceResolutionStrategy.ResolveAll<TObject>(overlayReference).ConfigureAwait(false);
        }

    }

    // FUTURE: Do DI with references to ExtensionlessReferenceResolutionService and in turn serialization strategies.  For now, use singletons.

    public class ExtensionlessFsOBase : OBaseOverlay<ExtensionlessFileReference, LocalFileReference>, IOBaseOverlay<ExtensionlessFileReference, LocalFileReference>
    {
        #region Static

        public static ExtensionlessFsOBase Instance => DependencyContext.Default.GetServiceOrSingleton<ExtensionlessFsOBase>();

        #endregion

        #region Constants

        public override IEnumerable<string> UriSchemes
        {
            get
            {
                yield return "efile";
            }
        }

        #endregion

        #region Relationships

        public override IOBus OBus => ManualSingleton<ExtensionlessFsOBus>.GuaranteedInstance;

        public override IOBase UnderlyingOBase => FsOBase.Instance;

        #endregion

        #region Parameters

        public override IReferenceToReferenceResolver ReferenceResolutionStrategy { get; }

        #endregion

        #region Construction

        public ExtensionlessFsOBase()
        {
            this.ReferenceResolutionStrategy = new ExtensionlessReferenceResolutionService(UnderlyingOBase);
        }

        public ExtensionlessFsOBase(ExtensionlessReferenceResolutionService resolutionService)
        {
            this.ReferenceResolutionStrategy = resolutionService;
        }

        #endregion



    }
}
