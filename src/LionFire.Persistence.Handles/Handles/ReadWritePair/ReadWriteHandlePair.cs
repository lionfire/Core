using LionFire.Events;
using LionFire.ExtensionMethods.Resolves;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Resolves;
using LionFire.Results;
using LionFire.Structures;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    public class ReadWriteHandlePairEx<TReference, TValue> : ReadWriteHandlePairBase<TReference, TValue>
        // , IReadWriteHandlePairEx<T> TODO
        where TReference : IReference
        where TValue : class
    {
    }

    //// This gets back to OBus / OBase.
    //// Question: Previously, handles had a reference to the OBase that created the handle.  I think that can be a good idea, perhaps optionally
    //public abstract class HandlePersistenceProviderBase : IHandlePersistenceProvider
    //{

    //}

    public class NoopReadWriteHandlePair<TReference, TValue> : ReadWriteHandlePair<TReference, TValue>
        where TReference : IReference
        where TValue : class // Required by UltraMapper
    {
        public override TValue Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override bool HasValue => throw new NotImplementedException();

        public override PersistenceFlags Flags => throw new NotImplementedException();

        public override Task<bool?> Delete() => throw new NotImplementedException();
        public override void DiscardValue() => throw new NotImplementedException();
        public override void MarkDeleted() => throw new NotImplementedException();
        public override Task<ISuccessResult> Put() => throw new NotImplementedException();
        public override Task<ISuccessResult> Put(TValue value) => throw new NotImplementedException();
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

        TValue IReadWrapper<TValue>.Value => throw new NotImplementedException();

        TValue IWriteWrapper<TValue>.Value { set => throw new NotImplementedException(); }

        public abstract Task<bool?> Delete();
        public abstract void DiscardValue();
        public abstract void MarkDeleted();
        public abstract Task<ISuccessResult> Put();
        public abstract Task<ISuccessResult> Put(TValue value);

        //public Task<bool?> Delete() => WriteHandle.Delete();
        //public void MarkDeleted() => WriteHandle.MarkDeleted();
        //public Task<ISuccessResult> Put(TValue value) => WriteHandle.Put(value);
    }

    /// <summary>
    /// Automatically clones the ReadHandle.Value to WriteHandle.Value when:
    ///  - WriteHandle is lazily created and HasReadHandle and ReadHandle.HasValue are true
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class ReadWriteHandlePairBase<TReference, TValue> : ReadWriteHandlePairBase<TReference, TValue, IReadHandleBase<TValue>, IWriteHandleBase<TValue>>, IReadWriteHandlePairBase<TValue>, IWriteHandleBase<TValue>
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

        public async Task<IResolveResult<TValue>> GetReadValue() => await ReadHandle.GetValue().ConfigureAwait(false); // REVIEW - consider a ILazilyResolves<T> so we can use ILazilyResolves<T>.GetValue instead of the IDefaultableReadWrapper extension method 


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
            if (readHandle is ILazilyResolves<TValue> lr)
            {
                lr.DiscardValue();
            }
            writeHandle?.DiscardValue();
        }


        #region IReadHandle<T>

        TValue IReadWrapper<TValue>.Value => ReadHandle.Value;

        #endregion

        #region IWriteHandle<T>

        public bool GetLocalValueFromRemote => throw new NotImplementedException();

        TValue IWriteWrapper<TValue>.Value { set => WriteHandle.Value = value; }

        Task<ISuccessResult> IPuts.Put() => WriteHandle.Put();
        

        #endregion

        #endregion

        //#region ReadHandle

        //public abstract IReadHandleBase<T> ReadHandle { get; protected set; }

        ////public virtual IReadHandleBase<T> ReadHandle
        ////{
        ////    get
        ////    {
        ////        if (readHandle == null)
        ////        {
        ////            if (Reference != null)
        ////            {
        ////                readHandle = Reference.GetReadHandle<T>();
        ////            }
        ////        }
        ////        return readHandle;
        ////    }
        ////    protected set
        ////    {
        ////        readHandle = value;
        ////    }
        ////}
        ////protected IReadHandleBase<T> readHandle;
        ////public bool HasReadHandle => readHandle != null;
        //public abstract bool HasReadHandle { get; }
        //IResolves<T> IResolveCommitPair<T>.Resolves => ReadHandle;

        //#endregion

        //#region WriteHandle
        //public abstract IWriteHandleBase<T> WriteHandle { get; protected set; }
        ////public virtual IWriteHandleBase<T> WriteHandle
        ////{
        ////    get
        ////    {
        ////        if (writeHandle == null)
        ////        {
        ////            if (Reference != null)
        ////            {
        ////                writeHandle = Reference.GetWriteHandle<T>();
        ////            }
        ////        }
        ////        return writeHandle;
        ////    }
        ////    protected set => writeHandle = value;
        ////}
        ////protected IWriteHandleBase<T> writeHandle;
        ////public bool HasWriteHandle => writeHandle != null;
        //public abstract bool HasWriteHandle { get; }
        //IPuts IResolveCommitPair<T>.Commits => WriteHandle;

        //#endregion
    }

}
