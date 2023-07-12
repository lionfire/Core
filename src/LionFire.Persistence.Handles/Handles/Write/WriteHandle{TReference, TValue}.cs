//#nullable enable
using System;
using System.Threading.Tasks;
using LionFire.Referencing;
using LionFire.Data.Gets;
using MorseCode.ITask;
using LionFire.Data.Sets;

namespace LionFire.Persistence.Handles;

public abstract class WriteHandle<TReference, TValue> 
    : WriteHandleBase<TReference, TValue>, IWriteHandle<TValue>
    , INotifyPersists<TValue>
    , INotifyingHandleInternal<TValue>
    , IStagesSetWithPersistenceFlags<TValue>

    , INotifyPersistsInternal<TValue>
    where TReference : IReference<TValue>
    //where TValue : class
{
    protected WriteHandle() { }
    protected WriteHandle(TReference reference) : base(reference) { }
    protected WriteHandle(TReference reference, TValue prestagedValue = default) : base(reference, prestagedValue) { }

    protected override ITask<IGetResult<TValue>> GetImpl(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    IPersistenceSnapshot<TValue> IPersists<TValue>.PersistenceState
         => new PersistenceSnapshot<TValue>
         {
             Value = StagedValue,
             Flags = Flags,
             HasValue = HasValue,
         };

    object INotifyingHandleInternal<TValue>.PersistenceLock => persistenceLock;
    readonly object persistenceLock = new object();
    
    PersistenceFlags IHandleInternal<TValue>.Flags { set => Flags = value; }

#nullable enable
    public event Action<PersistenceEvent<TValue>>? PersistenceStateChanged;
    void INotifyPersistsInternal<TValue>.RaisePersistenceEvent(PersistenceEvent<TValue> ev) => PersistenceStateChanged?.Invoke(ev);
#nullable disable

    //void INotifyingHandleInternal<TValue>.RaisePersistenceStateChanged(PersistenceEvent<TValue> ev) => PersistenceStateChanged?.Invoke(ev);

    //PersistenceFlags INotifyingHandleInternal<TValue>.Flags { get => Flags; set => Flags = value; }

    public override void MarkDeleted() => this.StageSetAndNotify(() => this.StageValue_Write_Old(default));
}
