using LionFire.Referencing;
using System;
using System.Collections.Generic;

namespace LionFire.Persistence
{

    //public interface IDeepCloneable
    //{
    //    object DeepClone();
    //}

    //public interface IDeepCloner<TValue>
    //{
    //    TValue DeepClone(TValue source);
    //}

    /*
 * Purpose
locally hold latest value (maybe synced) from store
have a separate working copy version in memory, ready to write back to the store
IReadOnlyHandle
IWriteOnlyHandle
Features:
Revert
Round-trip verification
Dependencies
IDeepClonable
ensures deep clone
IComparable
*/

    /// <summary>
    /// TODO: Maybe scrap this, and extend IHandle interface.
    /// 
    /// Holds a Local Handle to be used as a workspace for preparing changes to be written back to the data source, as well as a Remote Handle to hold a read-only
    /// Value from the data store that is not altered and kept only for reference.
    /// 
    /// Suggestions:
    /// 
    ///  - T should ideally implement IDeepCloneable or have a IDeepCloner&lt;T&gt; or IDeepCloner&lt;object&gt; service registered; otherwise, 
    ///    If T is not IDeepCloneable and there is no IDeepCloner, and the LocalHandle Value is requested, the RemoteHandle's Value (if-any) will be forgotten
    ///
    /// Features:
    /// 
    ///  - Revert Local changes without re-retrieving from data store
    ///  - Applications can use =, Equals, or IComparer&lt;T&gt; to compare the Values of each.  (See IDualHandleExtensions for helper methods.)
    ///  - Applications can compare local Value properties to remote Value properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDualHandle<T> : IReferencable
    {
        H<T> LocalHandle { get; }
        RH<T> RemoteHandle { get; }

        /// <summary>
        /// Revert (forget) changes to the local handle, replacing the LocalHandle's Value with a cloned copy of the last-known RemoteHandle's Value (if any).
        /// </summary>
        void Revert();

        /// <summary>
        /// If true, when the Local Value is gotten, it will attempt to retrieve the Remote Value as a starting point.
        /// </summary>
        bool GetLocalValueFromRemote { get; }

        bool? IsLocalEqualToRemote { get; }
    }

    public static class IDualHandleExtensions
    {

        /// <summary>
        /// Compares LocalHandle.Value to RemoteHandle.Value.
        /// If either handle is missing, or has HasValue == false, -2 is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handle"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static int Compare<T>(this IDualHandle<T> handle, IComparer<T> comparer = null) {
            if (handle.LocalHandle == null || !handle.LocalHandle.HasValue) return -2;
            if (handle.RemoteHandle == null || !handle.RemoteHandle.HasValue) return -2;
            return (comparer ?? Comparer<T>.Default).Compare(handle.LocalHandle.Value, handle.RemoteHandle.Value);
        }
        public static bool IsDeepCloneable<T>(this IDualHandle<T> handle) { throw new NotImplementedException(); }
    }
}

