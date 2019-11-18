
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LionFire.ObjectBus.Ex;
using LionFire.ObjectBus.Handles;
using LionFire.Ontology;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Resolves;
using LionFire.Results;

namespace LionFire.ObjectBus.Ex // Extended API
{
    /// <summary>
    /// (Convenience API)
    /// </summary>
    public static class OBusEx
    {
        #region Resolve

        /// <summary>
        /// Validate reference is not null, resolves to an IOBase
        /// </summary>
        /// <returns>The OBase for the reference</returns>
        public static IOBase ResolveOBase(this IReference reference)
        {
            if (reference == null) throw new ArgumentNullException(nameof(reference));

            var obase = reference.TryGetOBase();
            if (obase == null) throw new NotSupportedException($"Could not get IOBase for this reference of type {reference.GetType().Name}");

            return obase;
        }

        #endregion

        #region Read

        #region Get

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static async Task<IRetrieveResult<TObject>> GetRetrieveResult<TObject>(this IReference reference)
            => await ResolveOBase(reference).Get<TObject>(reference).ConfigureAwait(false);

        #endregion

        #endregion
    }
}

namespace LionFire.ObjectBus
{

    /// <summary>
    /// (Convenience API, shortcut compared to IReference.ToHandle())
    /// 
    /// Static-only API, with extension methods.
    /// Performance consideration: 
    ///  - Unbound reference types will be resolved to a bound reference type each time.  (TODO:) Consider using a Handle instead of this interface, which will only resolve the bound reference type once.
    /// </summary>
    /// <remarks>
    /// How things get from a Reference to a concrete IOBase is via DependencyContext.Current.GetService<IReferenceToOBaseService>()
    /// </remarks>
    public static class OBus
    {

        #region Read
        
        #region Get

        //public static async Task<TObject> ToObject<TObject>(this IReference reference) => (await ((OBaseReadHandle<TObject>)reference.ToReadHandle<TObject>()).GetValue().ConfigureAwait(false)).Value;

        public static async Task<TObject> GetObject<TObject>(this IReference reference)
            => (await ((OBaseReadHandle<TObject>)reference.ToReadHandle<TObject>()).GetValue().ConfigureAwait(false)).Value;
            //=> (TObject)(await ((OBaseReadHandle<TObject>)reference.ToReadHandle<TObject>()).Get()).Object;

        #endregion

        #endregion

        #region Write

        #region Set

        public static async Task<ISuccessResult> SetObject<TObject>(this IReference reference, TObject @object) 
            => (await ((OBaseHandle<TObject>)reference.ToWriteHandle<TObject>()).Put(@object));

        #endregion

        #endregion

        #region Delete

        public static async Task<bool> CanDelete<T>(this IReference reference) => (await reference.TryGetOBase().CanDelete<T>(reference).ConfigureAwait(false)).IsPreviewSuccess();

        public static async Task<IPersistenceResult> TryDelete<T>(this IReference reference) => await reference.TryGetOBase().TryDelete<T>(reference).ConfigureAwait(false);
        public static async Task Delete<T>(this IReference reference) => await reference.TryGetOBase().Delete<T>(reference).ConfigureAwait(false);

        public static async Task Delete(this IReference reference) => await reference.TryGetOBase().Delete<object>(reference).ConfigureAwait(false);
        // FUTURE: public static Task<bool> Delete(this IReference reference, object onlyDeleteIfThisObject) => reference.GetOBase().Set(reference, value);

        #endregion

        #region GetChildren

        /// <summary>
        /// Get References for children.  E.g. for filesystem paths, this may be file:/// with the full path for each file.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static Task<IEnumerable<IReference>> ListReferences(this IReference reference) => throw new NotImplementedException();

        /// <summary>
        /// RENAME - List
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static Task<IEnumerable<string>> List(this IReference reference)
        {
            throw new NotImplementedException();
            //var handle = await reference.GetCollectionHandle<object>().GetKeys(reference).ConfigureAwait(false);
        }

        //public static Task<IEnumerable<H<object>>> ListReadHandles(IReferenceEx2 reference)
        //public static Task<IEnumerable<H<object>>> ListHandles(IReferenceEx2 reference)
        //{
        //    try
        //    {
        //        var children = new List<H<object>>();

        //        foreach (IOBase obase in (IEnumerable)ReferenceToOBaseExtensions.GetOBases(reference))
        //        {
        //            foreach (string childName in (IEnumerable)obase.GetChildrenNames(reference))
        //            {
        //                children.Add(reference.GetChild(childName).ToHandle());
        //            }
        //        }
        //        return children;
        //    }
        //    catch (Exception ex)
        //    {
        //        OBusEvents.OnException(OBusOperations.GetChildren, reference, ex);
        //        throw ex;
        //    }
        //}

        #endregion


    }
    
}

#if OLD // TOPORT - if needed but not sure this makes sense anymore.  Brains moved to OBase.
//#define TRACE_GET
#define TRACE_GET_NOTFOUND

#if NET4
#define WEAKMETADATA // Experimental way to attach various info
#endif
//#define USE_READCACHE

        //public static void Mount(string vosPath, IReference reference) => throw new NotImplementedException();//MountManager.Instance.Mount(

        //public static IEnumerable<Mount> GetObjectStores(this IReference reference)
        //{
        //    return VosMountManager.Instance.GetMountsForPath();
        //}

        //public static IEnumerable<IOBase> GetObjectStores(this IReference reference)
        //{
        //}

        //#endregion


#if WEAKMETADATA

        public class RetrievedFrom
        {
            public IReference Reference { get; set; }
        }

        static ConditionalWeakTable<object, RetrievedFrom> retrievedFrom = new ConditionalWeakTable<object, RetrievedFrom>();


        public class RetrievedObj
        {
            public object Object { get; set; }
        }

        static ConditionalWeakTable<IReference, RetrievedObj> retrievedObjects = new ConditionalWeakTable<IReference, RetrievedObj>();
#endif

#region Get
//        /// <summary>
//        /// Gets the first object found at the reference.
//        /// TODO:
//        ///  - GetOne - throw if more than one
//        ///  - get by precedence
//        ///  - merge get, return a MultiType
//        /// </summary>
//        /// <param name="reference"></param>
//        /// <returns></returns>
//        public static object TryGet(IReference reference, OptionalRef<RetrieveInfo> optionalRef = null)
//        {
//            //			Log.Info("ZX OBus.TryGet " );
//            //			Log.Info("ZX OBus.TryGet " + reference);

//#if TRACE_GET
//			Log.Trace("TryGet " + reference);
//#endif
//            if (reference == null)
//            {
//                throw new ArgumentNullException("reference");
//            }

//            IEnumerable<IOBase> os = reference.GetOBases();

//#if WEAKMETADATA
//#if USE_READCACHE
//            RetrievedObj cachedObj;
//            if (retrievedObjects.TryGetValue(reference, out cachedObj))
//            {
//                l.Debug("EXPERIMENTAL - OBus got object from cache: " + reference);
//                return cachedObj.Object;
//            }
//#endif
//#endif

//            if (reference.Scheme == null)
//            {
//                throw new ArgumentNullException("reference.Scheme == null");
//            }

//            List<object> results = new List<object>();

//            foreach (IOBase obase in (IEnumerable)ReferenceToOBaseExtensions.GetOBases(reference))
//            {
//                var obj = obase.TryGet(reference, optionalRef);
//                if (obj != null)
//                {
//#if WEAKMETADATA
//#if USE_READCACHE
//                    retrievedObjects.Remove(reference);
//                    retrievedObjects.Add(reference, new RetrievedObj() { Object = obj });
//#endif
//#endif
//                    return obj;
//                }
//            }
//#if TRACE_GET_NOTFOUND
//            lNotFound.Trace("Not found: " + reference);
//#endif
//            return null;
//        }

//        public static T TryGetAs<T>(IReference reference, OptionalRef<RetrieveInfo> optionalRef = null)
//            where T : class
//        {

//            try
//            {
//                if (reference == null)
//                {
//                    throw new ArgumentNullException("reference");
//                }

//                IEnumerable<IOBase> os = reference.GetOBases();

//                if (reference.Scheme == null)
//                {
//                    throw new ArgumentNullException("reference.Scheme == null");
//                }

//                List<object> results = new List<object>();

//                foreach (IOBase obase in (IEnumerable)ReferenceToOBaseExtensions.GetOBases(reference))
//                {
//                    var obj = obase.TryGet<T>(reference, optionalRef);
//                    T result = obj as T;
//                    if (result != null)
//                    {

//                        return result;
//                        //results.Add(obj);
//                    }
//                }
//#if TRACE_GET_NOTFOUND
//                lNotFound.Trace($"Not found<{typeof(T).Name}>: " + reference);
//#endif
//                return null;
//            }
//            catch (Exception ex)
//            {
//                OBusEvents.OnException(OBusOperations.Get, reference, ex);
//                throw ex;
//            }
//        }

        //    public static IHandle<T> GetHandle<T>(IReference reference, params string[] childChunks)
        //where T : class
        //    {
        //        reference.Path
        //    }


        //OLD
        ///// <summary>
        ///// Gets the first object found at the reference.
        ///// TODO:
        /////  - GetOne - throw if more than one
        /////  - get by precedence
        /////  - merge get, return a MultiType
        ///// </summary>
        ///// <param name="reference"></param>
        ///// <returns></returns>
        //public static object TryGet(IHandle reference)
        //{
        //    if (reference == null) throw new ArgumentNullException("reference");

        //    IEnumerable<IOBase> os = reference.GetOBases();

        //    if (reference.Scheme == null) throw new ArgumentNullException("reference.Scheme == null");

        //    List<object> results = new List<object>();

        //    foreach (var obase in OBaseBroker.GetOBases(reference))
        //    {
        //        var obj = obase.TryGet(reference);
        //        if (obj != null)
        //        {
        //            return obj;
        //        }
        //    }
        //    return null;
        //}

        //OLD
        //public static T TryGetAs<T>(IHandle reference)
        //    where T : class
        //{
        //    if (reference == null) throw new ArgumentNullException("reference");

        //    IEnumerable<IOBase> os = reference.GetOBases();

        //    if (reference.Scheme == null) throw new ArgumentNullException("reference.Scheme == null");

        //    List<object> results = new List<object>();

        //    foreach (var obase in OBaseBroker.GetOBases(reference))
        //    {
        //        var obj = obase.TryGet(reference);
        //        T result = obj as T;
        //        if (result != null)
        //        {
        //            return result;
        //            //results.Add(obj);
        //        }
        //    }
        //    return null;
        //}

#endregion

#region Set

        public static void SetAs<T>(IReference reference, T value)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                OBusEvents.OnException(OBusOperations.Set, reference, ex);
                throw ex;
            }
        }

#endregion

#region GetChildren

        // IDEA: MEMOPTIMIZE Memory Efficient storage of path Chunks.  Store path chunks, and use yield return in IReference.PathChunks that is hierarchical?

        
#if !AOT
        // TODO: async
        public static IEnumerable<H<T>> GetChildrenOfType<T>(IReference reference, bool verifyExistAsType = true)
            where T : class//, new()
        {
            //try
            //{
                //var hChildren = new List<H<T>>();

                foreach (var obase in ReferenceToOBaseExtensions.GetOBases(reference))
                {
                    var en = obase
                        .GetChildrenNames(reference)
                        .Select(childName => reference.GetChild(childName).OfType<T>().GetHandle());

                    if (verifyExistAsType)
                    {
                        en = en.Where(h => h.Exists().Result);
                    }

                    foreach (var e in en)
                    {
                        yield return e;
                    }

                    //hChildren.AddRange(en);
                }
                //return hChildren;
            //}
            //catch (Exception ex)
            //{
            //    OBusEvents.OnException(OBusOperations.GetChildren, reference, ex);
            //    throw;
            //}
        }

        //// TODO: async
        //public static IEnumerable<H<T>> GetChildrenOfType<T>(IReference reference, bool verifyExistAsType = true)
        //    where T : class//, new()
        //{
        //    try
        //    {
        //        var hChildren = new List<H<T>>();

        //        // TODO: Where object exists??
        //        foreach (var obase in OBaseBroker.GetOBases(reference))
        //        {
        //            var en = obase
        //                .GetChildrenNames(reference)
        //                .Select(childName => reference.GetChild(childName).OfType<T>().GetHandle());

        //            if (verifyExistAsType)
        //            {
        //                en = en.Where(h => h.Exists().Result);
        //            }

        //            hChildren.AddRange(en);
        //        }
        //        return hChildren;
        //    }
        //    catch (Exception ex)
        //    {
        //        OBusEvents.OnException(OBusOperations.GetChildren, reference, ex);
        //        throw;
        //    }
        //}
#endif

            // TODO REFACTOR: Use yield return instead of creating list
            // TODO: async version
        public static IEnumerable<H> GetChildrenOfType(IReference reference, Type T)
        {
            try
            {
                var hChildren = new List<H>();

                foreach (IOBase obase in (IEnumerable)ReferenceToOBaseExtensions.GetOBases(reference))
                {
                    hChildren.AddRange(obase.GetChildrenNames(reference).Select(childName => reference.GetChild(childName).ToHandle(null, T)));
                }
                return hChildren;
            }
            catch (Exception ex)
            {
                OBusEvents.OnException(OBusOperations.GetChildren, reference, ex);
                throw;
            }
        }
#region GetChildrenNames

        public static IEnumerable<string> GetChildrenNames(IReference reference)
        {
            try
            {
                List<string> children = new List<string>();

                foreach (IOBase obase in (IEnumerable)ReferenceToOBaseExtensions.GetOBases(reference))
                {
                    children.AddRange(obase.GetChildrenNames(reference));
                }
                return children;
            }
            catch (Exception ex)
            {
                OBusEvents.OnException(OBusOperations.GetChildren, reference, ex);
                throw ex;
            }
        }

        public static IEnumerable<string> GetChildrenNamesOfType<T>(IReference reference)
            where T : class, new()
        {
            try
            {
                List<string> hChildren = new List<string>();

                foreach (IOBase obase in (IEnumerable)ReferenceToOBaseExtensions.GetOBases(reference))
                {
                    foreach (string childName in (IEnumerable)obase.GetChildrenNamesOfType<T>(reference))
                    {
                        hChildren.Add(childName);
                    }
                }
                return hChildren;
            }
            catch (Exception ex)
            {
                OBusEvents.OnException(OBusOperations.GetChildren, reference, ex);
                throw ex;
            }
        }

        public static IEnumerable<string> GetChildrenNamesOfType(Type T, IReference reference)
        {
            try
            {
                List<string> hChildren = new List<string>();

                foreach (IOBase obase in (IEnumerable)ReferenceToOBaseExtensions.GetOBases(reference))
                {
                    foreach (string childName in (IEnumerable)obase.GetChildrenNamesOfType(T, reference))
                    {
                        hChildren.Add(childName);
                    }
                }
                return hChildren;

            }
            catch (Exception ex)
            {
                OBusEvents.OnException(OBusOperations.GetChildren, reference, ex);
                throw ex;
            }
        }

#endregion

#endregion

        private static ILogger lNotFound = Log.Get("LionFire.OBus.NotFound");
        private static ILogger l = Log.Get();

        public static IEnumerable<string> Roots
        {
            get
            {
                foreach(var provider in DependencyContext.Current.GetService<IEnumerable<IOBus>>())
                {
                    // TODO
                    yield return provider.GetType().Name;
                }
            }
        }

#region Exists

        public static bool Exists(IReference reference)
        {
            foreach (IOBase obase in (IEnumerable)ReferenceToOBaseExtensions.GetOBases(reference))
            {
                if (obase.Exists(reference))
                {
                    return true;
                }
            }
            return false;
        }

#endregion

#region Create

        public static void Create(IReference reference, object p)
        {
            if (Exists(reference))
            {
                throw new AlreadyException("Create failed: object already exists at specified reference.");
            }

            throw new NotImplementedException();
        }

#endregion

#region Delete

        public static bool? CanDelete(IReference reference)
        {
            bool? result = false;
            bool isFirst = true; ;

            foreach (IOBase obase in (IEnumerable)ReferenceToOBaseExtensions.GetOBases(reference))
            {
                // REFACTOR - ternary aggregation
                var canDelete = obase.CanDelete(reference);
                if (isFirst)
                {
                    result = canDelete;
                    isFirst = false;
                    continue;
                }

                // result can't be null here
                if (result.Value == true)
                {
                    if (true != canDelete)
                    {
                        result = null;
                        break;
                    }
                }
                else if (result.Value == false)
                {
                    if (false != canDelete)
                    {
                        result = null;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes all data in all OBases at the specified reference
        /// </summary>
        /// <returns>true if something was deleted</returns>
        public static bool TryDelete(IReference reference, bool preview = false)
        {
            bool result = false;
            foreach (IOBase obase in (IEnumerable)ReferenceToOBaseExtensions.GetOBases(reference))
            {
                result |= obase.TryDelete(reference, preview);
                if (result)
                {
                    break;
                }
            }

            return result;
        }

        

#endregion

    }
}
#endif