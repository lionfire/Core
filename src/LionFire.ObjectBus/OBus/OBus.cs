//#define TRACE_GET
#define TRACE_GET_NOTFOUND

#if NET4
#define WEAKMETADATA // Experimental way to attach various info
#endif
//#define USE_READCACHE
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using LionFire.Collections;
using LionFire.Structures;
using Microsoft.Extensions.Logging;
using LionFire.Referencing;

namespace LionFire.ObjectBus
{
    public static class OBus
    {
        //public static OBus Instance { get { return Singleton<OBus>.Instance; } }
        
        static OBus()
        {
            OBusConfig.Initialize();
            l.Debug("OBus initialized.");
        }

        #region Context

        //[ThreadStatic]
        //private static OBusContext current; // TODO

        #endregion

        #region Mounting

        public static void Mount(string vosPath, IReference reference)
        {
            throw new NotImplementedException();
            //MountManager.Instance.Mount(
        }

        #endregion

        #region ObjectStores

        //public static IEnumerable<Mount> GetObjectStores(this IReference reference)
        //{
        //    return VosMountManager.Instance.GetMountsForPath();
        //}

        //public static IEnumerable<IOBase> GetObjectStores(this IReference reference)
        //{

        //}

        #endregion

        #region References

        internal static bool IsValid(IReference reference)
        {
            return reference.GetOBases().Any();
        }

        #endregion

        #region Weak Metadata

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

        #endregion

        #region Create

        private static T _Create<T>(Func<T> createDefault = null)
        {
            T result;

            if (createDefault != null)
            {
                result = createDefault();
            }
            else
            {
                result = (T)Activator.CreateInstance(typeof(T));
            }

            return result;
        }

        #endregion

        #region Get

        public static T GetAs<T>(IReference reference)
            where T : class
        {
            T result = TryGetAs<T>(reference);
            if (result == null) throw new ObjectNotFoundException();
            return result;
        }

        public static T GetAsOrCreate<T>(IReference reference, Func<T> createDefault = null)
            where T : class
        {
            T result = TryGetAs<T>(reference);
            if (result == null)
            {
                result = _Create(createDefault);

            }
            return result;
        }

        //public static T TryGetAsOrCreate<T>(IReference reference, Func<T> createDefault = null)
        //{
        //    throw new NotImplementedException();
        //}
        

        public static object Get(IReference reference)
        {
            object result = TryGet(reference);
            if (result == null)
            {
                throw new OBusException("Get failed to find object with specified reference");
            }
            return result;
        }

        /// <summary>
        /// Gets the first object found at the reference.
        /// TODO:
        ///  - GetOne - throw if more than one
        ///  - get by precedence
        ///  - merge get, return a MultiType
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static object TryGet(IReference reference, OptionalRef<RetrieveInfo> optionalRef = null)
        {
            //			Log.Info("ZX OBus.TryGet " );
            //			Log.Info("ZX OBus.TryGet " + reference);

#if TRACE_GET
			Log.Trace("TryGet " + reference);
#endif
            if (reference == null) throw new ArgumentNullException("reference");

            IEnumerable<IOBase> os = reference.GetOBases();

#if WEAKMETADATA
#if USE_READCACHE
            RetrievedObj cachedObj;
            if (retrievedObjects.TryGetValue(reference, out cachedObj))
            {
                l.Debug("EXPERIMENTAL - OBus got object from cache: " + reference);
                return cachedObj.Object;
            }
#endif
#endif

            if (reference.Scheme == null) throw new ArgumentNullException("reference.Scheme == null");

            List<object> results = new List<object>();

            foreach (IOBase obase in (IEnumerable)OBaseBroker.GetOBases(reference))
            {
                var obj = obase.TryGet(reference, optionalRef);
                if (obj != null)
                {
#if WEAKMETADATA
#if USE_READCACHE
                    retrievedObjects.Remove(reference);
                    retrievedObjects.Add(reference, new RetrievedObj() { Object = obj });
#endif
#endif
                    return obj;
                }
            }
#if TRACE_GET_NOTFOUND
            lNotFound.Trace("Not found: " + reference);
#endif
            return null;
        }

        public static T TryGetAs<T>(IReference reference, OptionalRef<RetrieveInfo> optionalRef = null)
            where T : class
        {

            try
            {
                if (reference == null) throw new ArgumentNullException("reference");

                IEnumerable<IOBase> os = reference.GetOBases();

                if (reference.Scheme == null) throw new ArgumentNullException("reference.Scheme == null");

                List<object> results = new List<object>();

                foreach (IOBase obase in (IEnumerable)OBaseBroker.GetOBases(reference))
                {
                    var obj = obase.TryGet<T>(reference, optionalRef);
                    T result = obj as T;
                    if (result != null)
                    {

                        return result;
                        //results.Add(obj);
                    }
                }
#if TRACE_GET_NOTFOUND
                lNotFound.Trace($"Not found<{typeof(T).Name}>: " + reference);
#endif
                return null;
            }
            catch (Exception ex)
            {
                OBusEvents.OnException(OBusOperations.Get, reference, ex);
                throw ex;
            }
        }

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

        
        public static void Set(IReference reference, object value, bool preview = false)
        {
            try
            {
                OBusEvents.OnSaving(value); // TODO: Only call once at top level OBase

                var obp = OBaseProviderBroker.GetOBaseProvider(reference);
                obp.Set(reference, value);
            }
            catch (Exception ex)
            {
                OBusEvents.OnException(OBusOperations.Set, reference, ex);
                throw ex;
            }
        }

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

        public static IEnumerable<IHandle> GetChildren(IReferenceEx2 reference)
        {
            try
            {
                var children = new List<H<object>>();

                foreach (IOBase obase in (IEnumerable)OBaseBroker.GetOBases(reference))
                {
                    foreach (string childName in (IEnumerable)obase.GetChildrenNames(reference))
                    {
                        children.Add(reference.GetChild(childName).ToHandle());
                    }
                }
                return children;
            }
            catch (Exception ex)
            {
                OBusEvents.OnException(OBusOperations.GetChildren, reference, ex);
                throw ex;
            }
        }

#if !AOT
        public static IEnumerable<IHandle<T>> GetChildrenOfType<T>(IReference reference)
            where T : class//, new()
        {
            try
            {
                List<IHandle<T>> hChildren = new List<IHandle<T>>();

                // TODO: Where object exists??
                foreach (var obase in OBaseBroker.GetOBases(reference))
                {
                    hChildren.AddRange(obase.GetChildrenNames(reference).Select(childName => reference.GetChild(childName).GetHandle<T>(null))
                        .Where(h => h.QueryExistance())
                        );
                }
                return hChildren;
            }
            catch (Exception ex)
            {
                OBusEvents.OnException(OBusOperations.GetChildren, reference, ex);
                throw;
            }
        }
#endif
        public static IEnumerable<IHandle> GetChildrenOfType(IReference reference, Type T)
        {
            try
            {
                var hChildren = new List<IHandle>();

                foreach (IOBase obase in (IEnumerable)OBaseBroker.GetOBases(reference))
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

                foreach (IOBase obase in (IEnumerable)OBaseBroker.GetOBases(reference))
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

                foreach (IOBase obase in (IEnumerable)OBaseBroker.GetOBases(reference))
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

                foreach (IOBase obase in (IEnumerable)OBaseBroker.GetOBases(reference))
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

        #region Exists

        public static bool Exists(IReference reference)
        {
            foreach (IOBase obase in (IEnumerable)OBaseBroker.GetOBases(reference))
            {
                if (obase.Exists(reference)) return true;
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

            foreach (IOBase obase in (IEnumerable)OBaseBroker.GetOBases(reference))
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
            foreach (IOBase obase in (IEnumerable)OBaseBroker.GetOBases(reference))
            {
                result |= obase.TryDelete(reference, preview);
                if (result) break;
            }

            return result;
        }

        public static void Delete(IReference reference)
        {
            if (!TryDelete(reference))
            {
                throw new ObjectNotFoundException("Delete failed: no object found at specified reference.");
            }
        }

        #endregion

    }
}
