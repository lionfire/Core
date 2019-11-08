using LionFire.Persistence;
using LionFire.Referencing;
using System;
using System.Threading.Tasks;

namespace LionFire
{

    public struct HandleEvent
    {
        //public R Handle { get; set; }
    }

    [Flags]
    public enum HandleEventKind : int
    {
        Unspecified = 0,

        ObjectRetrieved = 1 << 1,
        ObjectNotFound = 1 << 2,

        /// <summary>
        /// True if either flag is set.
        /// </summary>
        ResolveSucceeded = ObjectRetrieved | ObjectNotFound,

        ResolveFailed = 1 << 3,

        ReadEvents = ObjectRetrieved | ObjectNotFound | ResolveFailed,

        MarkedForDeletion,
        UnmarkedForDeletion,

        Deleted,

        /// <summary>
        /// An object was written.  All mechanisms must support this.
        /// </summary>
        Saved,

        /// <summary>
        /// An existing object was modified.  Not all mechanisms support this.
        /// </summary>
        Modified,

        /// <summary>
        /// A new object was created.  Not all mechanisms support this.
        /// </summary>
        Created,
    }

    [Flags]
    public enum CollectionHandleEvent : int
    {
        Unspecified = 0,
        Added = 1 << 0,
        Removed = 1 << 1,
        Set = 1 << 2,
        Unset = 1 << 3,
        ReplacedAll = 1 << 4,
    }

    public enum HandleEventLocation
    {
        Unspecified = 0,
        Internal = 1 << 1,
        ToExternal = 1 << 2,
        FromExternal = 1 << 3,
    }

    public class WritableHandleState
    {
        public object ETag { get; set; }
    }
    

    public delegate void ReadHandleEvent<T>(IReadHandle<T> handle, HandleEventKind kind, HandleEventLocation location);


    ///// <summary>
    ///// Used as the base for the IReadHandle interfaces.
    ///// </summary>
    //[Obsolete("Use IResolvable / IStatefulResolvable?")]
    //public interface IResolvableHandle
    //{
    //    /// <summary>
    //    /// True if successfully retrieved the Object or lack thereof.  HasObject may be false if the retrieval mechanism succeeded and found no object at the source location.
    //    /// </summary>
    //    bool IsResolved { get; }
    //    event Action<bool> IsResolvedChanged;

    //    /// <summary>
    //    /// Returns true if retrieval was attempted via get_Object or TryResolveObject, and a non-null object was retrieved.
    //    /// </summary>
    //    bool HasObject { get; }

    //    // REVIEW: Is persistenceContext helpful?
    //    //Task<bool> TryResolveObject();
    //    //Task<bool> TryResolveObject(bool forgetOnFail = false);
    //    //Task<bool> TryResolveObject(object persistenceContext = null, bool forgetOnFail = false);
    //    void ForgetObject();
    //}
}
