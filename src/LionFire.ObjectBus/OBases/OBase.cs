#define TRAGE_GET
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LionFire.ObjectBus.Handles;
using LionFire.Persistence;
using LionFire.Referencing;
using Microsoft.Extensions.Logging;

namespace LionFire.ObjectBus
{

    //public abstract class MultiTypeOBase<TReference> : OBase<TReference>, IMultiTypeOBase
    //    where TReference : class, IReference
    //{
    //    public abstract Task<IRetrieveResult<T>> TryGetName<T>(TReference reference);
    //}

    /// <summary>
    /// Single reference type
    /// </summary>
    /// <typeparam name="TReference"></typeparam>
    public abstract class OBase<TReference> : IOBase
    where TReference : class, IReference
        //where HandleInterfaceType : IHandle
    {
        public abstract IOBus OBus { get; }

        #region Uri

        public abstract IEnumerable<string> UriSchemes { get; }

        #endregion

        #region Read

        #region Get


        public abstract Task<IRetrieveResult<T>> TryGet<T>(TReference reference);
        //Task<IRetrieveResult<T>> IOBase.TryGet<T>(IReference reference, Type type) => TryGet(ConvertToReferenceType(reference), type);

        Task<IRetrieveResult<T>> IOBase.Get<T>(IReference reference) => TryGet<T>(ConvertToReferenceType(reference));


        //public abstract ResultType TryGet<ResultType>(TReference reference, OptionalRef<RetrieveInfo> optionalRef = null)
        //where ResultType : class;
        //public abstract object TryGet(TReference reference, Type resultType, OptionalRef<RetrieveInfo> optionalRef = null);

        #region Dig out Object Type from TReference that implements  ITypedReference

        public virtual Task<IRetrieveResult<object>> TryGet(TReference reference)
        {
            if (reference is ITypedReference typedReference && typedReference.Type != null)
            {
                var mi = _TryGetMethodInfo.MakeGenericMethod(typedReference.Type);
                return (Task<IRetrieveResult<object>>)mi.Invoke(this, new object[] { reference });
            }
            else
            {
                return TryGet<object>(reference);
            }
        }

        #endregion

        protected MethodInfo _TryGetMethodInfo
        {
            get
            {
                if (tryGetMethodInfo == null)
                {
                    tryGetMethodInfo = GetType().GetMethods().Where(mi => mi.Name == "TryGet" && mi.ContainsGenericParameters).First();
                }
                return tryGetMethodInfo;
            }
        }
        private MethodInfo tryGetMethodInfo;

        //public virtual ResultType TryGet<ResultType>(IReference reference, OptionalRef<RetrieveInfo> optionalRef = null)
        //    where ResultType : class
        //{
        //    if (reference == null)
        //    {
        //        return null;
        //    }

        //    TReference r = reference as TReference;
        //    if (r == null)
        //    {
        //        throw new ArgumentException("Reference type not supported by this OBase: " + reference.GetType().FullName);
        //    }

        //    return TryGet<ResultType>(r, optionalRef);
        //}

        //public object TryGet(IReference reference, OptionalRef<RetrieveInfo> optionalRef = null) => TryGet(ConvertToReferenceType(reference), optionalRef);

        //public virtual object TryGet(HandleInterfaceType handle)
        //{
        //    return TryGet(handle.Reference);
        //}

        //public object TryGet(IHandle handle)
        //{
        //    //return TryGet(ConvertToHandleType(handle));
        //    return TryGet(ConvertToReferenceType(handle.Reference));
        //}

        #region Exists

        /// <remarks>
        /// Default implementation does a retrieve.  Override this to use a faster implementation that may be able to skip deserialization.
        /// </remarks>
        /// <param name="reference"></param>
        /// <returns></returns>
        public virtual async Task<(bool exists, IPersistenceResult result)> Exists(TReference reference)
        {
            var result = (await TryGet(reference));
            return (result.IsFound(), result);
        }

        public Task<(bool exists, IPersistenceResult result)> Exists(IReference reference)
            => Exists(ConvertToReferenceType(reference));

        #endregion

        #endregion

        #region List

        public abstract Task<IEnumerable<string>> List<T>(TReference parent)
            where T : class, new();
        Task<IEnumerable<string>> IOBase.List<T>(IReference parent) => List<object>(ConvertToReferenceType(parent));

        #endregion

        #endregion

        #region Write

        #region Set

        Task<IPersistenceResult> IOBase.Set<T>(IReference reference, T obj, bool allowOverwrite) => Set(ConvertToReferenceType(reference), obj, allowOverwrite);

        protected abstract Task<IPersistenceResult> SetImpl<T>(TReference reference, T obj, bool allowOverwrite = true);

        public virtual async Task<IPersistenceResult> Set<T>(TReference reference, T obj, bool allowOverwrite = true)
        {
            try
            {
                // TODO MULTILEVEL: Only call events once at top level OBase?  Create a transaction and add that transaction here as an optional parameter.
                // Set(TReference reference, object obj, bool allowOverwrite = true, bool preview = false, ObjectBusPersistenceOperation op = null)
                // { if(!op.SuppressEvents)  OBaseEvents.OnSaving(obj); 

                OBaseEvents.OnSaving(obj);

                return await SetImpl<T>(reference, obj, allowOverwrite).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                OBaseEvents.OnException(OBusOperations.Set, reference, ex);
                throw ex;
            }
        }

        //public virtual async Task Set<T>(TReference reference, T obj, bool allowOverwrite = true, bool preview = false)
        //{
        //    try
        //    {
        //        // TODO MULTILEVEL: Only call events once at top level OBase?  Create a transaction and add that transaction here as an optional parameter.
        //        // Set(TReference reference, object obj, bool allowOverwrite = true, bool preview = false, ObjectBusPersistenceOperation op = null)
        //        // { if(!op.SuppressEvents)  OBaseEvents.OnSaving(obj); 

        //        OBaseEvents.OnSaving(obj);

        //        await _Set<T>(reference, obj, allowOverwrite, preview).ConfigureAwait(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        OBaseEvents.OnException(OBusOperations.Set, reference, ex);
        //        throw ex;
        //    }
        //}

        #endregion

        #region Delete

        public abstract Task<IPersistenceResult> CanDelete<T>(TReference reference);

        public abstract Task<IPersistenceResult> TryDelete<T>(TReference reference/*, bool preview = false*/);

        #region IReference overloads

        Task<IPersistenceResult> IOBase.CanDelete<T>(IReference reference) => CanDelete<T>(ConvertToReferenceType(reference));
        //public Task<bool?> CanDelete<T>(IReference reference) => CanDelete<T>(ConvertToReferenceType(reference));
        public Task<IPersistenceResult> TryDelete<T>(IReference reference/*, bool preview*/) => TryDelete<T>(ConvertToReferenceType(reference)/*, preview*/);

        #endregion

        #endregion

        #endregion

        #region Handle

        public virtual W<T> GetHandle<T>(IReference reference)
        {
            if (!(reference is TReference)) throw new ArgumentException($"{nameof(reference)} must be a {typeof(TReference).FullName} (TODO: Convert from compatible reference types)");
            return new OBaseHandle<T>(reference, this);
        }
        public virtual RH<T> GetReadHandle<T>(IReference reference)
        {
            if (!(reference is TReference)) throw new ArgumentException($"{nameof(reference)} must be a {typeof(TReference).FullName} (TODO: Convert from compatible reference types)");
            return new OBaseReadHandle<T>(reference, this);
        }

        //// Prefer IHandle.GetSubpath.  Default implementation of that uses this:
        //public virtual IHandle<T> GetHandleSubpath<T>(IReference reference, params string[] subpathChunks) where T : class
        //{
        //    reference.GetChildSubpath(subpathChunks);
        //    return reference.ToHandle<T>();
        //}
        //public virtual IHandle<T> GetHandleSubpath<T>(IHandle handle, params string[] subpathChunks) where T : class
        //{
        //    handle.Reference.GetChildSubpath(subpathChunks);
        //    return reference.ToHandle<T>();
        //}

        #endregion

        #region (Protected) Conversion utils

        protected TReference ConvertToReferenceType(IReference reference)
        {
            if (reference == null)
            {
                return null;
            }

            TReference reft = reference as TReference;
            if (reft == null)
            {
                throw new ArgumentException($"Unsupported reference type '{reference?.GetType().Name}'.  Supported: '{typeof(TReference).Name}'");
            }

            return reft;
        }

        //protected virtual HandleInterfaceType ConvertToHandleType(IHandle handle)
        //{
        //    if (handle == null) return null;
        //    HandleInterfaceType reft = handle as HandleInterfaceType;
        //    if (reft == null) throw new ArgumentException("Unsupported Handle type");
        //    return reft;
        //}

        #endregion

        #region Watcher / Events

        public PersistenceEventKind SupportsEvents(PersistenceEventSourceKind sourceType = PersistenceEventSourceKind.Unspecified) => throw new NotImplementedException();
        public IObservable<OBasePersistenceEvent> Subscribe(Predicate<IReference> filter = null, PersistenceEventSourceKind sourceType = PersistenceEventSourceKind.Unspecified) => throw new NotImplementedException();


        public virtual IObjectWatcher GetObjectWatcher(IReference reference) => null;


        #endregion

        #region Misc

        private static readonly ILogger l = Log.Get();

        #endregion

    }
}
