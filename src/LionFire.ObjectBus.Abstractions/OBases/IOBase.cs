using System;
using System.Collections.Generic;
using LionFire.Persistence;
using LionFire.Referencing;

namespace LionFire.ObjectBus
{

    public interface IWritableOBase : IOBase
    {
        void Set(IReference reference, object obj, bool allowOverwrite = true);
        bool TryDelete(IReference reference, bool preview = false);

        /// <returns>true if can be fully deleted, false if no delete can take place, and null if it can be partially deleted</returns>
        bool? CanDelete(IReference reference);
    }

    public interface IHasPersistenceEvents
    {
        PersistenceEventType SupportsEvents(PersistenceEventSourceType sourceType = PersistenceEventSourceType.Unspecified);
        event IObservable<OBasePersistenceEvent> Subscribe(Predicate < IReference > filter = null, PersistenceEventSourceType sourceType = PersistenceEventSourceType.Unspecified);
    }

    public struct OBasePersistenceEvent
    {
        //public IReference Reference { get; set; }

        #region Reference

        public IReference Reference
        {
            get { return reference ?? Handle?.Reference; }
            set { if (handle!=null) { throw new AlreadyException("Cannot set if Handle is already set."); } reference = value; }
        }
        private IReference reference;

        #endregion

        #region Object

        public object Object
        {
            get { return obj ?? Handle?.Object; }
            set { obj = value; }
        }
        private object obj;

        #endregion

        #region Handle

        public IHandle Handle
        {
            get { return handle /* ?? Reference?.ToHandle()*/; }
            set { if (reference != null) { throw new AlreadyException("Cannot set if Reference is already set."); }  handle = value; }
        }
        private IHandle handle;

        #endregion
    }


    public interface IOBase : IUriProvider
    {
        //T Get<T>(IReference reference);
        //T TryGet<T>(IReference reference);

        object TryGet(IHandle handle);

        object TryGet(IReference reference, OptionalRef<RetrieveInfo> optionalRef = null);
        T TryGet<T>(IReference reference, OptionalRef<RetrieveInfo> optionalRef = null) where T : class;


        // FUTURE: flags for hidden, persisted
        //IEnumerable<string> GetChildrenNames(IReference parent, QueryFlags requiredFlags = QueryFlags.None, QueryFlags excludeFlags = QueryFlags.None);

        IEnumerable<string> GetChildrenNames(IReference parent);
        IEnumerable<string> GetChildrenNamesOfType<T>(IReference parent) where T : class, new();
        IEnumerable<string> GetChildrenNamesOfType(Type type, IReference parent);

        //event Action<string> Changed;

#if !AOT
        H<T> GetHandle<T>(IReference reference) where T : class;

#endif

        // Prefer IHandle.GetSubpath.  Default implementation of that uses this:
        //IHandle<T> GetHandleSubpath<T>(IReference reference, params string[] subpathChunks) where T : class;

        bool Exists(IReference reference);

        ///// <summary>
        ///// FUTURE? Or ObjectWatcher that parallels FileSystemWatcher? Set changeTypes to None to disable
        ///// </summary>
        ///// <param name="handler"></param>
        ///// <param name="reference"></param>
        ///// <param name="changeTypes"></param>
        //void MonitorEvents(ObjectChangedHandler handler, IReference reference, ChangeTypes changeTypes = ChangeTypes.All);

        IObjectWatcher GetWatcher(IReference reference);
    }

    //public class ObjectWatcher
    //{
    //public IReference Reference{get;set;}
    //}

    public static class IOBaseExtensions
    {
        public static object Get(this IOBase obase, IReference reference, OptionalRef<RetrieveInfo> optionalRef = null)
        {
            object result = obase.TryGet(reference, optionalRef);
            if (result == null)
            {
                throw new ObjectNotFoundException("Could not find object with specified reference");
            }
            return result;
        }
    }

}
