using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LionFire.Persistence;
using LionFire.Referencing;

namespace LionFire.ObjectBus
{

    public interface IMultiTypeOBase : IOBase
    {
        Task<IRetrieveResult<T>> TryGetName<T>(IReference reference);
    }

    /// <summary>
    /// Performs operations on a certain URI schemas and types of IReferences that it understands (typically just one schema and one type.)
    /// 
    /// </summary>
    /// <remarks>
    /// OBases do not deal with handles, but it may be useful to use handles a layer above this in order to facilitate caching and object reuse.
    /// 
    /// TODO: Make all persistence async!
    /// TODO: Reword GetChildren to List.
    /// </remarks>
    public interface IOBase : ISupportsUriSchemes
    {
        #region Parent

        IOBus OBus { get; }

        #endregion

        #region Read

        //object TryGet(IReference reference, OptionalRef<RetrieveInfo> optionalRef = null);
        //T TryGet<T>(IReference reference, OptionalRef<RetrieveInfo> optionalRef = null) where T : class;

        // RENAME to Retrieve, and instead have EnsureGet which throws on not found
        // TODO BREAKING: make Get an extension method on HBase(this IOBase obase, IReference)
        Task<RetrieveResult<T>> TryGet<T>(IReference reference);


        Task<RetrieveResult<bool>> Exists(IReference reference);

        // FUTURE: flags for hidden, persisted
        //IEnumerable<string> GetChildrenNames(IReference parent, QueryFlags requiredFlags = QueryFlags.None, QueryFlags excludeFlags = QueryFlags.None);

        Task<IEnumerable<string>> GetKeys(IReference parent);
        [Obsolete("Use GetKeys instead")]
        IEnumerable<string> GetChildrenNames(IReference parent); // RENAME to GetKeyNames


        Task<IEnumerable<string>> GetKeysOfType<T>(IReference parent) where T : class, new();
        [Obsolete("Use GetKeysOfType")]
        IEnumerable<string> GetChildrenNamesOfType<T>(IReference parent) where T : class, new();

        Task<IEnumerable<string>> GetKeysOfType(IReference parent, Type type);

        [Obsolete("Use GetKeysOfType")]
        IEnumerable<string> GetChildrenNamesOfType(IReference parent, Type type);

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

        IObjectWatcher GetObjectWatcher(IReference reference);

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
#error NEXT: Change Set() to Set<T>()

        Task<IPersistenceResult> TryDelete<T>(IReference reference);

        /// <returns>true if can be fully deleted, false if no delete can take place, and null if it can be partially deleted</returns>
        Task<IPersistenceResult> CanDelete<T>(IReference reference);

        #endregion
    }

    public static class IOBaseExtensions
    {
        public static Task<IRetrieveResult<object>> TryGet(this IOBase obase, IReference reference) => obase.TryGet<object>(reference);

        public static async Task<IRetrieveResult<object>> Get(this IOBase obase, IReference reference)
        {
            var result = await obase.TryGet(reference);
            if (!result.IsSuccess())
            {
                throw new ObjectNotFoundException("Could not find object with specified reference");
            }
            return result;
        }

        public static async Task Delete<T>(this IOBase obase, IReference reference)
        {
            if ((await obase.TryDelete<T>(reference).ConfigureAwait(false)).WriteCount <= 0)
            {
                throw new ObjectNotFoundException("Delete failed: no object found at specified reference.");
            }
        }
    }

}
