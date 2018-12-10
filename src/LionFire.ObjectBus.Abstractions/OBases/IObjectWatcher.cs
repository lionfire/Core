using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    [Flags]
    public enum WatcherChangeType : uint
    {
        None = 0,
        Modified = 1 << 0,
        Created = 1 << 1,
        Deleted = 1 << 2,
        Renamed = 1 << 3,
        Saved = 1 << 4,
        Loaded = 1 << 5,

        All = 0xFFFFFFFF,
    }

    public delegate void ObjectChangedHandler(IReference reference, WatcherChangeType changeType, IReference renamedFrom);
       

    /// <summary>
    /// Watch individual object
    /// </summary>
    public interface IObjectWatcher : IDisposable
    {
        string Path { get; set; }
        //event Action<IReference, WatcherChangeType, IObjectWatcher> ReferenceEventForWatcher;

        // TODO REVIEW - eliminate ReferenceChangedFor?

        event Action<IObjectWatcher, IReference> ReferenceChangedFor;

        event ObjectChangedHandler ObjectChanged;
    }
}
