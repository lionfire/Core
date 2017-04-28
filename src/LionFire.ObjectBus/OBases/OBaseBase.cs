#define TRAGE_GET
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LionFire.ObjectBus
{
    public abstract class OBaseBase<ReferenceType> : IOBase
        where ReferenceType : class, IReference
    //where HandleInterfaceType : IHandle
    {
        #region Uri

        public abstract string[] UriSchemes
        {
            get;
        }

        #endregion

        #region Get

        public virtual object TryGet(ReferenceType reference, OptionalRef<RetrieveInfo> optionalRef = null)
        {
            //l.Trace(" TryGet " + reference);
            if (reference.Type != null)
            {
                var mi = _TryGetMethodInfo.MakeGenericMethod(reference.Type);
                return mi.Invoke(this, new object[] { reference, optionalRef });
            }
            else
            {
                //                l.Trace("ZX Invoking TryGet<object> - " + reference);
                return TryGet<object>(reference);
            }
        }

        protected MethodInfo _TryGetMethodInfo
        {
            get
            {
                if (tryGetMethodInfo == null)
                {
                    tryGetMethodInfo = this.GetType().GetMethods().Where(mi => mi.Name == "TryGet" && mi.ContainsGenericParameters).First();
                }
                return tryGetMethodInfo;
            }
        }
        private MethodInfo tryGetMethodInfo;

        public virtual ResultType TryGet<ResultType>(IReference reference, OptionalRef<RetrieveInfo> optionalRef = null)
            where ResultType : class
        {
            if (reference == null) return null;
            ReferenceType r = reference as ReferenceType;
            if (r == null) throw new ArgumentException("Reference type not supported by this OBase: " + reference.GetType().FullName);
            return TryGet<ResultType>(r, optionalRef);
        }

        public abstract ResultType TryGet<ResultType>(ReferenceType reference, OptionalRef<RetrieveInfo> optionalRef = null)
            where ResultType : class;

        public abstract object TryGet(ReferenceType reference, Type resultType, OptionalRef<RetrieveInfo> optionalRef = null);

        public object TryGet(IReference reference, OptionalRef<RetrieveInfo> optionalRef = null)
        {
            return TryGet(ConvertToReferenceType(reference), optionalRef);
        }

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

        bool IOBase.Exists(IReference reference)
        {
            return Exists(ConvertToReferenceType(reference));
        }
        public virtual bool Exists(ReferenceType reference)
        {
            object obj = TryGet(reference);
            bool result = obj != null;
            return result;
        }

        #endregion

        #endregion

        #region Set

        public abstract void Set(ReferenceType reference, object obj, bool allowOverwrite = true, bool preview = false);
        public void Set(IReference reference, object obj, bool allowOverwrite = true)
        {
            Set(ConvertToReferenceType(reference), obj, allowOverwrite);
        }

        #endregion

        #region Delete

        public abstract bool? CanDelete(ReferenceType reference);
        bool? IOBase.CanDelete(IReference reference)
        {
            return CanDelete(ConvertToReferenceType(reference));
        }

        public abstract bool TryDelete(ReferenceType reference, bool preview = false);

        bool IOBase.TryDelete(IReference reference, bool preview)
        {
            return TryDelete(ConvertToReferenceType(reference), preview);
        }

        #endregion

        #region Children

        public abstract IEnumerable<string> GetChildrenNames(ReferenceType parent);

        public IEnumerable<string> GetChildrenNames(IReference parent)
        {
            return GetChildrenNames(ConvertToReferenceType(parent));
        }

        public abstract IEnumerable<string> GetChildrenNamesOfType<T>(ReferenceType parent)
            where T : class, new();

        public IEnumerable<string> GetChildrenNamesOfType<T>(IReference parent)
            where T : class, new()
        {
            return GetChildrenNamesOfType<T>(ConvertToReferenceType(parent));
        }

        public IEnumerable<string> GetChildrenNamesOfType(Type type, IReference parent)
        {
            return GetChildrenNamesOfType(type, ConvertToReferenceType(parent));
        }

        #endregion

        #region GetHandle
//#if !AOT // TOAOT
        public virtual IHandle<T> GetHandle<T>(IReference reference) where T : class
        {
            // Uses HandleFactory
            return reference.GetHandle<T>();
        }
//#endif

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

        protected ReferenceType ConvertToReferenceType(IReference reference)
        {
            if (reference == null) return null;
            ReferenceType reft = reference as ReferenceType;
            if (reft == null) throw new ArgumentException("Unsupported reference type");
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

        #region Watcher

        public virtual IObjectWatcher GetWatcher(IReference reference) { return null; }

        #endregion

        #region Misc

        private static ILogger l = Log.Get();

        #endregion

    }
}
