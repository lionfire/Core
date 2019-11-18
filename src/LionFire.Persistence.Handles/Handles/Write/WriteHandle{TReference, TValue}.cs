//#nullable enable
using System;
using System.Threading.Tasks;
using LionFire.Referencing;
using LionFire.Resolves;

namespace LionFire.Persistence.Handles
{
    public abstract class WriteHandle<TReference, TValue> : WriteHandleBase<TReference, TValue>, IWriteHandle<TValue>
        , INotifyPersists<TValue>
        , INotifyingHandleInternal<TValue>
        , INotifyPersistsInternal<TValue>
        where TReference : class, IReference
        where TValue : class
    {
        protected override Task<IResolveResult<TValue>> ResolveImpl() => throw new NotImplementedException();

        PersistenceSnapshot<TValue> IPersists<TValue>.PersistenceState
             => new PersistenceSnapshot<TValue>
             {
                 Value = ProtectedValue,
                 Flags = Flags,
                 HasValue = HasValue,
             };

        object INotifyingHandleInternal<TValue>.PersistenceLock => persistenceLock;
        readonly object persistenceLock = new object();
        TValue IHandleInternal<TValue>.ProtectedValue { get => ProtectedValue; set => ProtectedValue = value; }
        PersistenceFlags IHandleInternal<TValue>.Flags { set => Flags = value; }

#nullable enable
        public event Action<PersistenceEvent<TValue>>? PersistenceStateChanged;
        void INotifyPersistsInternal<TValue>.RaisePersistenceEvent(PersistenceEvent<TValue> ev) => PersistenceStateChanged?.Invoke(ev);
#nullable disable

        //void INotifyingHandleInternal<TValue>.RaisePersistenceStateChanged(PersistenceEvent<TValue> ev) => PersistenceStateChanged?.Invoke(ev);

        //PersistenceFlags INotifyingHandleInternal<TValue>.Flags { get => Flags; set => Flags = value; }

        public override void MarkDeleted() => this.MutatePersistenceStateAndNotify(() => this.OnUserChangedValue_Write(default));
    }
}
