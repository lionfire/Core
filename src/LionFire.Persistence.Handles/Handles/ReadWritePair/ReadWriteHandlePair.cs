using LionFire.Events;
using LionFire.ExtensionMethods.Resolves;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Data.Gets;
using LionFire.Results;
using LionFire.Structures;
using MorseCode.ITask;
using System;
using System.Threading;
using System.Threading.Tasks;
using LionFire.Data.Sets;

namespace LionFire.Persistence;

public class ReadWriteHandlePairEx<TReference, TValue> : ReadWriteHandlePairBase<TReference, TValue>
    // , IReadWriteHandlePairEx<TValue> TODO
    where TReference : IReference
    where TValue : class
{
}

public class NoopReadWriteHandlePair<TReference, TValue> : ReadWriteHandlePair<TReference, TValue>
    where TReference : IReference
    where TValue : class // Required by UltraMapper
{
    public override TValue Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override bool HasValue => throw new NotImplementedException();

    public override PersistenceFlags Flags => throw new NotImplementedException();

    public override TValue StagedValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override bool HasStagedValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override Task<bool?> Delete() => throw new NotImplementedException();

    public override void DiscardStagedValue()
    {
        throw new NotImplementedException();
    }

    public override void DiscardValue() => throw new NotImplementedException();
    public override void MarkDeleted() => throw new NotImplementedException();
    public override Task<ITransferResult> Set(TValue value, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public override Task<ITransferResult> Set(CancellationToken cancellationToken = default) => throw new NotImplementedException();
}

public abstract class ReadWriteHandlePair<TReference, TValue>
    : ReadWriteHandlePairBase<TReference, TValue, IReadHandle<TValue>, IWriteHandle<TValue>>
    , IReadWriteHandlePair<TValue>
    , IWriteHandle<TValue>
    where TReference : IReference
    where TValue : class // Required by UltraMapper
{

    public abstract TValue Value { get; set; }
    public abstract bool HasValue { get; }
    public abstract PersistenceFlags Flags { get; }
    public abstract TValue StagedValue { get; set; }
    public abstract bool HasStagedValue { get; set; }

    TValue IReadWrapper<TValue>.Value => Value;

    TValue IWriteWrapper<TValue>.Value { set => StagedValue = value; }

    public abstract Task<bool?> Delete();
    public abstract void DiscardStagedValue();
    public abstract void DiscardValue();
    public abstract void MarkDeleted();
    public abstract Task<ITransferResult> Set(CancellationToken cancellationToken = default);
    public abstract Task<ITransferResult> Set(TValue value, CancellationToken cancellationToken = default);

    //public Task<bool?> Delete() => WriteHandle.Delete();
    //public void MarkDeleted() => WriteHandle.MarkDeleted();
    //public Task<ISuccessResult> Put(TValue value) => WriteHandle.Put(value);
}

/// <summary>
/// Automatically clones the ReadHandle.Value to WriteHandle.Value when:
///  - WriteHandle is lazily created and HasReadHandle and ReadHandle.HasValue are true
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class ReadWriteHandlePairBase<TReference, TValue>
    : ReadWriteHandlePairBase<TReference, TValue, IReadHandleBase<TValue>, IWriteHandleBase<TValue>>
    , ISets<TValue>
    , IStagesSet<TValue>
    , IReadWriteHandlePairBase<TValue>
    , IWriteHandleBase<TValue>
    , IWrapper<TValue>
    , IReadWrapper<TValue>
    , IWriteWrapper<TValue>
    where TReference : IReference
    where TValue : class // Required by UltraMapper
{

    #region ReadHandle

    public override IReadHandleBase<TValue> ReadHandle
    {
        get
        {
            if (readHandle == null)
            {
                if (Reference != null)
                {
                    readHandle = UpCast<IReadHandleBase<TValue>, IReadHandle<TValue>>(Reference.GetReadHandle<TValue>());
                    if (readHandle is INotifyChanged<TValue> nvc)
                    {
                        nvc.Changed += OnReadValueChanged;
                    }
                }
            }
            return readHandle;
        }
        protected set
        {
            if (value == null)
            {
                if (readHandle != null && readHandle is INotifyChanged<TValue> nvc)
                {
                    nvc.Changed -= OnReadValueChanged;
                }
            }
            readHandle = value;
        }
    }

    public async Task<TValue> GetReadValue() => await ReadHandle.GetValue().ConfigureAwait(false); // REVIEW - consider a ILazilyGets<TValue> so we can use ILazilyGets<TValue>.GetValue instead of the IDefaultableReadWrapper extension method 


    #endregion

    #region WriteHandle

    public override IWriteHandleBase<TValue> WriteHandle
    {
        get
        {
            if (writeHandle == null)
            {
                if (Reference != null)
                {
                    writeHandle = UpCast<IWriteHandleBase<TValue>, IWriteHandle<TValue>>(Reference.TryGetWriteHandle<TValue>());
                    if (readHandle != null)
                    {
                        var getResult = readHandle.GetValueWithoutRetrieve();
                        if (getResult.HasValue)
                        {
                            CloneValueToWriteHandleValue(getResult.Value);
                        }
                    }
                }
            }
            return writeHandle;
        }
        protected set
        {
            writeHandle = value;
        }
    }

    public void SetWriteValue(TValue value)
    {
        if (writeHandle == null)
        {
            if (Reference != null)
            {
                writeHandle = Reference.TryGetWriteHandle<TValue>();
            }
        }
        writeHandle.Value = value;
    }

    private void OnReadValueChanged(IValueChanged<TValue> obj)
    {
        if (writeHandle != null && !writeHandle.HasValue)
        {
            CloneValueToWriteHandleValue(obj.New);
        }
    }

    #endregion

    #region Handle pass-thru

    public PersistenceFlags Flags => ReadHandle.Flags | WriteHandle.Flags; // TODO: Mask each state with read/write masks before combining them?

    public virtual PersistenceFlags SupportedFlags => PersistenceFlags.OutgoingFlags | PersistenceFlags.IncomingFlags;

    public bool HasValue => (readHandle?.HasValue == true) || (writeHandle?.HasValue == true);

    public TValue Value { get => ReadHandle.Value; set => WriteHandle.Value = value; }

    public void DiscardValue()
    {
        if (readHandle is ILazilyGets<TValue> lr)
        {
            lr.DiscardValue();
        }
        writeHandle?.DiscardValue();
    }


    #region IReadHandle<T>

    TValue IReadWrapper<TValue>.Value => ReadHandle.Value;

    #endregion

    #region IWriteHandle<T>

    //public bool GetLocalValueFromRemote => throw new NotImplementedException();

    TValue IWriteWrapper<TValue>.Value { set => WriteHandle.Value = value; }
    public TValue StagedValue { get => WriteHandle.StagedValue; set => WriteHandle.StagedValue = value; }
    public bool HasStagedValue { get => WriteHandle.HasStagedValue; set => WriteHandle.HasStagedValue = value; }

    Task<ITransferResult> ISets.Set(CancellationToken cancellationToken) => WriteHandle.Set(cancellationToken);

    public Task<ITransferResult> Set(TValue value, CancellationToken cancellationToken = default)
        => WriteHandle.Set(value, cancellationToken);

    public void DiscardStagedValue()=> WriteHandle.DiscardStagedValue();


    #endregion

    #endregion

    //#region ReadHandle

    //public abstract IReadHandleBase<TValue> ReadHandle { get; protected set; }

    ////public virtual IReadHandleBase<TValue> ReadHandle
    ////{
    ////    get
    ////    {
    ////        if (readHandle == null)
    ////        {
    ////            if (Reference != null)
    ////            {
    ////                readHandle = Reference.GetReadHandle<TValue>();
    ////            }
    ////        }
    ////        return readHandle;
    ////    }
    ////    protected set
    ////    {
    ////        readHandle = value;
    ////    }
    ////}
    ////protected IReadHandleBase<TValue> readHandle;
    ////public bool HasReadHandle => readHandle != null;
    //public abstract bool HasReadHandle { get; }
    //IGets<TValue> IResolveCommitPair<TValue>.Resolves => ReadHandle;

    //#endregion

    //#region WriteHandle
    //public abstract IWriteHandleBase<TValue> WriteHandle { get; protected set; }
    ////public virtual IWriteHandleBase<TValue> WriteHandle
    ////{
    ////    get
    ////    {
    ////        if (writeHandle == null)
    ////        {
    ////            if (Reference != null)
    ////            {
    ////                writeHandle = Reference.GetWriteHandle<TValue>();
    ////            }
    ////        }
    ////        return writeHandle;
    ////    }
    ////    protected set => writeHandle = value;
    ////}
    ////protected IWriteHandleBase<TValue> writeHandle;
    ////public bool HasWriteHandle => writeHandle != null;
    //public abstract bool HasWriteHandle { get; }
    //IPuts IResolveCommitPair<TValue>.Commits => WriteHandle;

    //#endregion
}
