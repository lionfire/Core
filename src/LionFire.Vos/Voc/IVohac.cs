using LionFire.Collections;
using LionFire.Persistence;
using System.Collections.Generic;

namespace LionFire.Persistence.Collections
{
    // was IVohac<T>

    // TODO: Make an Async interface, and IResolvable

    /// <remarks>
    /// "Vob Handle Collection" - maybe change the name or combine with IVoc
    /// Maybe make this a Dictionary&lt;name,VobReadHandle`T&gt;
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public interface IReadHandleCollection<T> : INotifyingList<IReadHandle<T>>
        where T : class, new()
    {
        IReadHandle<T> this[string name] { get; }
        void RefreshCollection();

        //IEnumerable<string> ChildPaths { get; } // OLD Seems redundant. Why did I have this?
        IEnumerable<string> Names { get; } // RENAME Keys?
        IEnumerable<IReadHandle<T>> Handles { get; } // RENAME Values?

    }
}
