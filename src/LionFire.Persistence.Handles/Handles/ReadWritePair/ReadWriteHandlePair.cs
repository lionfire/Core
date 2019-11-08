using LionFire.Events;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Resolves;
using LionFire.Structures;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    public class ReadWriteHandlePairEx<TValue> : ReadWriteHandlePairBase<TValue>
    // , IReadWriteHandlePairEx<T> TODO
        where  TValue : class
    {
    }

    public class ReadWriteHandlePair<T> : ReadWriteHandlePairBase<T, IReadHandle<T>, IWriteHandle<T>>, IReadWriteHandlePair<T>, IWriteHandle<T>
    {
        public bool HasValue => throw new NotImplementedException();

        public T Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public PersistenceFlags Flags => throw new NotImplementedException();

        T IReadWrapper<T>.Value => throw new NotImplementedException();

        T IWriteWrapper<T>.Value { set => throw new NotImplementedException(); }

        public Task<bool> Delete() => throw new NotImplementedException();
        public void DiscardValue() => throw new NotImplementedException();
        public void MarkDeleted() => throw new NotImplementedException();
        public Task<IPutResult> Put() => throw new NotImplementedException();
        public Task<IPutResult> Put(T value) => throw new NotImplementedException();
    }

    /// <summary>
    /// Automatically clones the ReadHandle.Value to WriteHandle.Value when:
    ///  - WriteHandle is lazily created and HasReadHandle and ReadHandle.HasValue are true
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class ReadWriteHandlePairBase<TValue> : ReadWriteHandlePairBase<TValue, IReadHandleBase<TValue>, IWriteHandleBase<TValue>>, IReadWriteHandlePairBase<TValue>, IWriteHandleBase<TValue>
        , IWrapper<TValue>
        , IReadWrapper<TValue>
        , IWriteWrapper<TValue>
        where TValue : class
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

        public async Task<IResolveResult<TValue>> GetReadValue() => await ReadHandle.GetValue().ConfigureAwait(false);


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
                        writeHandle = UpCast<IWriteHandleBase<TValue>, IWriteHandle<TValue>>(Reference.GetWriteHandle<TValue>());
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
                    writeHandle = Reference.GetWriteHandle<TValue>();
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

        public bool HasValue => readHandle?.Value || writeHandle?.HasValue;

        public TValue Value { get => ReadHandle.Value; set => WriteHandle.Value = value; }

        public void DiscardValue()
        {
            if (readHandle is ILazilyResolves<TValue> lr)
            {
                lr.DiscardValue();
            }
            writeHandle?.DiscardValue();
        }

        #endregion

        #region R<T>

        TValue IReadWrapper<TValue>.Value => ReadHandle.Value;

        #endregion

        #region WO<T>

        public bool GetLocalValueFromRemote => throw new NotImplementedException();

        TValue IWriteWrapper<TValue>.Value { set => WriteHandle.Value = value; }

        Task<IPutResult> IPuts.Put() => WriteHandle.Put();

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
