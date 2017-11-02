using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence
{
    public enum PersistenceEventKind
    {
        Unspecified,
        Changed,
        Added,
        Removed,
        Renamed,
    }

    public delegate void PersistenceEventHandler(object source, PersistenceEventKind kind, object itemLocation, object context = null);

    public interface INotifyPersistence
    {
        event PersistenceEventHandler PersistenceEvent;

        /// <summary>
        /// Default: null, will listen if there are attached to PersistenceEvent.
        /// False: cease listening even if there are handlers of PersistenceEvent.
        /// </summary>
        bool? ListeningToPersistenceEvents { get; set; }
    }


}