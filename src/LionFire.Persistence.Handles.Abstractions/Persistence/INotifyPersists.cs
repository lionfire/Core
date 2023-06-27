﻿
namespace LionFire.Persistence;


// TODO TODESIGN: Can OBus use this?
//public delegate void PersistenceEventHandler(object source, PersistenceEventKind kind, object itemLocation, object context = null);
public interface INotifyPersists<TValue> : IPersists<TValue>
    //where TValue : class
{
    //event PersistenceEventHandler PersistenceEvent;

    event Action<PersistenceEvent<TValue>> PersistenceStateChanged;

    ///// <summary>
    ///// Default: null, will listen if there are attached to PersistenceEvent.
    ///// False: cease listening even if there are handlers of PersistenceEvent.
    ///// </summary>
    //bool? ListeningToPersistenceEvents { get; set; }
}