#define TRAGE_GET
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LionFire.Persistence;
using LionFire.Referencing;
using Microsoft.Extensions.Logging;

namespace LionFire.ObjectBus
{
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

        public abstract IEnumerable<string> UriSchemes
        {
            get;
        }

        #endregion

        #region Read

        #region Get

        public abstract Task<IRetrieveResult<T>> TryGet<T>(TReference reference);
        public abstract Task<IRetrieveResult<object>> TryGet(TReference reference, Type type);

        Task<IRetrieveResult<T>> IOBase.TryGet<T>(IReference reference) => TryGet<T>(ConvertToReferenceType(reference));

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

        public object TryGet(IHandle handle)
        {
            //return TryGet(ConvertToHandleType(handle));
            return TryGet(ConvertToReferenceType(handle.Reference));
        }

        #region Exists

        Task<IRetrieveResult<bool>> IOBase.Exists(IReference reference) => Exists(ConvertToReferenceType(reference));
        public virtual async Task<IRetrieveResult<bool>> Exists(TReference reference)
        {
            var result = new RetrieveResult<bool>();
            var getResult = await TryGet(reference);
            result.IsSuccess = getResult.IsSuccess;
            result.Result = getResult.Result != null;
            return result;
        }

        #endregion

        #endregion

        #region Children

        public abstract Task<IEnumerable<string>> GetKeys(TReference parent);
        Task<IEnumerable<string>> IOBase.GetKeys(IReference parent) => GetKeys(ConvertToReferenceType(parent));

        public abstract IEnumerable<string> GetChildrenNames(TReference parent);

        public IEnumerable<string> GetChildrenNames(IReference parent) => GetChildrenNames(ConvertToReferenceType(parent));

        public abstract IEnumerable<string> GetChildrenNamesOfType<T>(TReference parent)
            where T : class, new();

        public IEnumerable<string> GetChildrenNamesOfType<T>(IReference parent)
            where T : class, new() => GetChildrenNamesOfType<T>(ConvertToReferenceType(parent));

        public IEnumerable<string> GetChildrenNamesOfType(Type type, IReference parent) => GetChildrenNamesOfType(type, ConvertToReferenceType(parent));

        #endregion

        #endregion

        #region Write

        Task IOBase.Set(IReference reference, object obj, Type type, bool allowOverwrite) => Set(ConvertToReferenceType(reference), obj, type, allowOverwrite);

        protected abstract Task _Set(TReference reference, object obj, Type type = null, bool allowOverwrite = true, bool preview = false);

        public virtual async Task Set(TReference reference, object obj, Type type = null, bool allowOverwrite = true/*, bool preview = false*/)
        {
            try
            {
                // TODO MULTILEVEL: Only call events once at top level OBase?  Create a transaction and add that transaction here as an optional parameter.
                // Set(TReference reference, object obj, bool allowOverwrite = true, bool preview = false, ObjectBusPersistenceOperation op = null)
                // { if(!op.SuppressEvents)  OBaseEvents.OnSaving(obj); 

                OBaseEvents.OnSaving(obj);

                await _Set(reference, obj, obj?.GetType() ?? typeof(object), allowOverwrite/*, preview*/).ConfigureAwait(false);
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

        #region Delete

        public abstract Task<bool?> CanDelete(TReference reference);
        public abstract Task<bool?> TryDelete(TReference reference/*, bool preview = false*/);

        public Task<bool?> CanDelete(IReference reference) => CanDelete(ConvertToReferenceType(reference));
        public Task<bool?> TryDelete(IReference reference/*, bool preview*/) => TryDelete(ConvertToReferenceType(reference)/*, preview*/);

        #endregion

        #endregion

        #region Handle

        public abstract H<T> GetHandle<T>(IReference reference);// where T : class => HandleProvider<T>.GetHandle(reference);
        public abstract RH<T> GetReadHandle<T>(IReference reference);

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
                throw new ArgumentException("Unsupported reference type");
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
