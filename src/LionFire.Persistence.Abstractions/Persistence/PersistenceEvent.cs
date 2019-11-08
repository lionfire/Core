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

    public struct PersistenceEvent<T> : ISenderValueChanged<INotifyPersists<T>, IPersistenceSnapshot<T>>
    //: IPersistenceChangeEvent<T>
    {
        public INotifyPersists<T> Sender { get; set; }

        //public PersistenceEventKind Kind { get; set; }

        public IPersistenceSnapshot<T> New { get; set; }
        IPersistenceSnapshot<T> IValueChanged<IPersistenceSnapshot<T>>.New => New;
        public IPersistenceSnapshot<T> Old { get; set; }
        IPersistenceSnapshot<T> IValueChanged<IPersistenceSnapshot<T>>.Old => Old;

        public T NewValue => New.Value;
        public T OldValue => Old.Value;
        public bool ValueChanged => EqualityComparer<T>.Default.Equals(NewValue, OldValue);

        public PersistenceFlags NewFlags => New.Flags;
        public PersistenceFlags OldFlags => Old.Flags;

        public bool FlagsChanged => NewFlags != OldFlags;
        public PersistenceFlags FlagsAdded => NewFlags & ~OldFlags;
        public PersistenceFlags FlagsRemoved => OldFlags & ~NewFlags;
    }
}
