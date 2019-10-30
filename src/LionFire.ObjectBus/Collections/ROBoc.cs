using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Collections;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;

namespace LionFire.ObjectBus.Handles
{
    public enum OBocTrackingState
    {
        Unspecified,
        Unchanged = 1 << 0,
        Added = 1 << 1,
        Removed = 1 << 2,
    }

    /// <summary>
    /// OBus Object Collection contains a collection of list entries (OBaseListEntry) representing the items within the folder at a OBase reference.  
    /// (Examples: filesystem directory, database table.)
    /// </summary>
    /// <typeparam name="TListEntry"></typeparam>
    public abstract class ROBoc<T, TListEntry> : ReadHandle<INotifyingReadOnlyCollection<TListEntry>>, RC<T, TListEntry>
        where TListEntry : ICollectionEntry
    {
        #region Construction

        public ROBoc() : this(null) { }

        /// <param name="reference">(Can be null)</param>
        public ROBoc(IReference reference) : base(reference)
        {
        }

        #endregion

        public RH<INotifyingReadOnlyCollection<T>> Handle { get; protected set; }
        public INotifyingReadOnlyCollection<TListEntry> Entries => Value;

        public int Count => Entries.Count;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public abstract IEnumerator<T> GetEnumerator();

    }
}
