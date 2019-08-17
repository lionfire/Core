using LionFire.Collections;
using LionFire.Referencing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.Collections
{
    //    // C collections inherit RH because they have their own interface for doing add/remove.

    //    //public interface C : RH<INotifyingReadOnlyCollection<object>> { }
    //    public interface C : HC<object> { }


    //    //public interface D<T>
    //    //{
    //    //}


    //        // HC<T> MOVED to RC.cs Temporarily

    //    public enum CollectionOperationType
    //    {
    //        Add,
    //        Remove,
    //    }
    //    public class CollectionOperation
    //    {
    //        public CollectionOperationType Type { get; set; }

    //    }

    //    // Default collection type:
    //    // 

    //    // Optional collection interfaces:
    //    // - IHasIndexer

    //    //public interface IIndexableCollection
    //    //{
    //    //    void InsertAt(int index, IHandleBase handle);
    //    //    void RemoveAt(int index, IHandleBase handle);
    //    //    H<T> ElementAt(int index);
    //    //}

    //    public interface IIndexableCollection<T>
    //    {
    //        void InsertAt(int index, H<T> handle);
    //        void RemoveAt(int index, H<T> handle);
    //        H<T> ElementAt(int index);
    //    }


    //    // RC Collection types:
    //    // - IEnumerable
    //    // - ReadOnlyList
    //    // - ReadOnlyDictionary<string,T>

    //    // C Collection types:
    //    // - name value dict
    //    // - list (ordered)
    //    // - sortedlist
    //    // - set
    //    // - bag

    //    // Idea:
    //    // - for Dict, have a D<> and RD<> class
    //    // - Make D's like normal filesystem directories

    //    // - C like .NET ICollection: Add, Remove, Count

    //    // SortedValuesList is a hybrid C and D -- need a Func to get key from items, and an optional comparator
    //    // - Value Comparer: invoke key selector and sort numerically/alphabetically
}
