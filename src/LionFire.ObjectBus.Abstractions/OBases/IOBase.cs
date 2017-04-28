using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    //public enum QueryFlags
    //{
    //    None = 0,
    //    Hidden = 1 << 0,
    //    Persisted = 1 << 1,
    //}

    public class RetrieveInfo // MOVE
    {
        
        public IOBase UltimateOBase { get; set; }
        public IReference UltimateReference { get; set; }
    }

    
    public interface IOBase : IUriProvider
    {
        //T Get<T>(IReference reference);
        //T TryGet<T>(IReference reference);

        object TryGet(IHandle handle);

        object TryGet(IReference reference, OptionalRef<RetrieveInfo> optionalRef = null);
        T TryGet<T>(IReference reference, OptionalRef<RetrieveInfo> optionalRef = null) where T : class;
        void Set(IReference reference, object obj, bool allowOverwrite = true);
        bool TryDelete(IReference reference, bool preview = false);

        /// <returns>true if can be fully deleted, false if no delete can take place, and null if it can be partially deleted</returns>
        bool? CanDelete(IReference reference);

        // FUTURE: flags for hidden, persisted
        //IEnumerable<string> GetChildrenNames(IReference parent, QueryFlags requiredFlags = QueryFlags.None, QueryFlags excludeFlags = QueryFlags.None);

        IEnumerable<string> GetChildrenNames(IReference parent);
        IEnumerable<string> GetChildrenNamesOfType<T>(IReference parent) where T : class, new();
        IEnumerable<string> GetChildrenNamesOfType(Type type, IReference parent);

        //event Action<string> Changed;

#if !AOT
        IHandle<T> GetHandle<T>(IReference reference) where T : class;
        
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
        public static object Get(this IOBase obase, IReference reference)
        {
            object result = obase.TryGet(reference);
            if (result == null)
            {
                throw new ObjectNotFoundException("Could not find object with specified reference");
            }
            return result;
        }
    }

}
