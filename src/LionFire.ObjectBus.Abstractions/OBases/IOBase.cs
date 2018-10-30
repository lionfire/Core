using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LionFire.Persistence;
using LionFire.Referencing;

namespace LionFire.ObjectBus
{

    /// <summary>
    /// TODO: All persistence async! (?)
    /// </summary>
    public interface IOBase : ISupportsUriSchemes
    {
        #region Parent

        IOBus OBus { get; }

        #endregion

        #region Read

        //object TryGet(IReference reference, OptionalRef<RetrieveInfo> optionalRef = null);
        //T TryGet<T>(IReference reference, OptionalRef<RetrieveInfo> optionalRef = null) where T : class;

        Task<IRetrieveResult<T>> TryGet<T>(IReference reference);

        Task<IRetrieveResult<bool>> Exists(IReference reference);

        // FUTURE: flags for hidden, persisted
        //IEnumerable<string> GetChildrenNames(IReference parent, QueryFlags requiredFlags = QueryFlags.None, QueryFlags excludeFlags = QueryFlags.None);

        IEnumerable<string> GetChildrenNames(IReference parent);
        IEnumerable<string> GetChildrenNamesOfType<T>(IReference parent) where T : class, new();
        IEnumerable<string> GetChildrenNamesOfType(Type type, IReference parent);

        //event Action<string> Changed;

        // Prefer IHandle.GetSubpath.  Default implementation of that uses this:
        //IHandle<T> GetHandleSubpath<T>(IReference reference, params string[] subpathChunks) where T : class;
        
        ///// <summary>
        ///// FUTURE? Or ObjectWatcher that parallels FileSystemWatcher? Set changeTypes to None to disable
        ///// </summary>
        ///// <param name="handler"></param>
        ///// <param name="reference"></param>
        ///// <param name="changeTypes"></param>
        //void MonitorEvents(ObjectChangedHandler handler, IReference reference, ChangeTypes changeTypes = ChangeTypes.All);

        #endregion

        #region Events

        IObjectWatcher GetWatcher(IReference reference);

        PersistenceEventKind SupportsEvents(PersistenceEventSourceKind sourceType = PersistenceEventSourceKind.Unspecified);
        IObservable<OBasePersistenceEvent> Subscribe(Predicate<IReference> filter = null, PersistenceEventSourceKind sourceType = PersistenceEventSourceKind.Unspecified);

        #endregion

        #region Write
        
        /// <summary>
        /// Rules for type:
        ///  - leave null to use type of obj (defaults to typeof(object) if null).
        ///  - TBD (when to save type info?  what about multityping?)
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <param name="allowOverwrite"></param>
        /// <returns></returns>
        Task Set(IReference reference, object obj, Type type = null, bool allowOverwrite = true);

        Task<bool> TryDelete(IReference reference, bool preview = false);

        /// <returns>true if can be fully deleted, false if no delete can take place, and null if it can be partially deleted</returns>
        Task<bool?> CanDelete(IReference reference);

        #endregion
    }

    public static class IOBaseExtensions
    {
        public static Task<IRetrieveResult<object>> TryGet(this IOBase obase, IReference reference) => obase.TryGet<object>(reference);

        public static async Task<IRetrieveResult<object>> Get(this IOBase obase, IReference reference)
        {
            var result = await obase.TryGet(reference);
            if (!result.IsSuccess)
            {
                throw new ObjectNotFoundException("Could not find object with specified reference");
            }
            return result;
        }

        public static async Task Delete(this IOBase obase, IReference reference)
        {
            if (!(await obase.TryDelete(reference).ConfigureAwait(false)))
            {
                throw new ObjectNotFoundException("Delete failed: no object found at specified reference.");
            }
        }
    }
 
}
