using LionFire.Events;
using System;
using System.Collections.Generic;

namespace LionFire.Persistence
{
    //public struct PersistenceEvent<T>
    //: ISenderValueChanged<INotifyPersisted<T>, IPersistenceSnapshot<T>>
    ////: IPersistenceChangeEvent<T>
    //{
    //}

    public struct PersistenceEvent<TValue> : ISenderValueChanged<INotifyPersists<TValue>, IPersistenceSnapshot<TValue>>
    //: IPersistenceChangeEvent<T>
        //where TValue : class
    {
        public INotifyPersists<TValue> Sender { get; set; }

        //public PersistenceEventKind Kind { get; set; }

        public IPersistenceSnapshot<TValue> New { get; set; }
        IPersistenceSnapshot<TValue> IValueChanged<IPersistenceSnapshot<TValue>>.New => New;
        public IPersistenceSnapshot<TValue> Old { get; set; }
        IPersistenceSnapshot<TValue> IValueChanged<IPersistenceSnapshot<TValue>>.Old => Old;

        public TValue NewValue => New.Value;
        public TValue OldValue => Old.Value;
        public bool ValueChanged => EqualityComparer<TValue>.Default.Equals(NewValue, OldValue);

        public PersistenceFlags NewFlags => New.Flags;
        public PersistenceFlags OldFlags => Old.Flags;

        public bool FlagsChanged => NewFlags != OldFlags;
        public PersistenceFlags FlagsAdded => NewFlags & ~OldFlags;
        public PersistenceFlags FlagsRemoved => OldFlags & ~NewFlags;
    }
}
