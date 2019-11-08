using LionFire.Referencing;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{

    //public interface IDeepCloneable
    //{
    //    object DeepClone();
    //}

    //public interface IDeepCloner<T>
    //{
    //    T DeepClone(T source);
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

    public interface IReadWriteHandlePairBase<T, out TReadHandle, out TWriteHandle> : IReferencable
        where TReadHandle : IReadHandleBase<T>
        where TWriteHandle : IWriteHandleBase<T>
    {
        TReadHandle ReadHandle { get; }
        bool HasReadHandle { get; }

        TWriteHandle WriteHandle { get; }
        bool HasWriteHandle { get; }
    }

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
    public interface IReadWriteHandlePairBase<T> : IReadWriteHandlePairBase<T, IReadHandleBase<T>, IWriteHandleBase<T>>
    {
    }
    
}

