using LionFire.Events;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Resolves;
using LionFire.Structures;
using LionFire.Threading;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    public class ReadWriteHandlePairEx<T> : ReadWriteHandlePair<T>
        // , IReadWriteHandlePairEx<T> TODO
    {

    }

    /// <summary>
    /// Automatically clones the ReadHandle.Value to WriteHandle.Value when:
    ///  - WriteHandle is lazily created and HasReadHandle and ReadHandle.HasValue are true
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReadWriteHandlePair<T> : ReadWriteHandlePairBase<T>, IReadWriteHandlePair<T>, WO<T>
    {
        public virtual bool HasChanges => Changes() == null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Null if no differences, otherwise an object describing the differences</returns>
        public virtual object Changes()
        {
            var result = AnyDiff.AnyDiff.Diff(ReadHandle?.Value, WriteHandle?.Value);
            if (result.Count == 0)
            {
                return null;
            }
            return result;
        }

        #region Cloning

        public async Task<(T clonedValue, bool clonedSomething)> CloneReadHandleValueToWriteHandleValue(bool allowRetrieve = true, bool propagateNoValue = false)
        {
            (bool, T) getResult;
            if (!allowRetrieve)
            {
                if (!HasReadHandle)
                {
                    getResult = (false, default);
                }
                else
                {
                    getResult = (ReadHandle.GetValue(allowRetrieve: false));
                }
            }
            else { 
                var getResult = await ReadHandle.GetValue().ConfigureAwait(false);
            }

            if (getResult.HasValue)
            {
                return (CloneValueToWriteHandleValue(getResult.Value), true);
            }
            else
            {
                if (propagateNoValue)
                {
                    WriteHandle.DiscardValue();
                    return (default, true);
                }
            }
            return (default, false);
        }

        public virtual T CloneValueToWriteHandleValue(T value)
        {
            var clonedValue = AnyClone.Cloner.Clone(value);
            WriteHandle.Value = clonedValue;
            return clonedValue;
        }

        #endregion

        #region ReadHandle

        public override RH<T> ReadHandle
        {
            get
            {
                if (readHandle == null)
                {
                    if (Reference != null)
                    {
                        readHandle = Reference.GetReadHandle<T>();
                        if (readHandle is INotifyChanged<T> nvc)
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
                    if (readHandle != null && readHandle is INotifyChanged<T> nvc)
                    {
                        nvc.Changed -= OnReadValueChanged;
                    }
                }
                readHandle = value;
            }
        }

        public async Task<(bool, T)> GetReadValue() => await ReadHandle.GetValue().ConfigureAwait(false);


        #endregion

        #region WriteHandle

        public override WO<T> WriteHandle
        {
            get
            {
                if (writeHandle == null)
                {
                    if (Reference != null)
                    {
                        writeHandle = Reference.GetWriteHandle<T>();
                        if (readHandle != null) {
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

        public void SetWriteValue(T value)
        {
            if (writeHandle == null)
            {
                if (Reference != null)
                {
                    writeHandle = Reference.GetWriteHandle<T>();
                }
            }
            writeHandle.Value = value;
        }

        /// <summary>
        /// Get the Value for WriteHandle.  If WriteHandle.HasValue is false, attempt to get a good starting value, either by:
        ///  - using a clone of the existing value of ReadHandle.Value
        ///  - (if allowRetrieve is true) doing a ReadHandle.GetValue and using a clone of the value
        ///  - (if createIsMissing is true) creating a new value from the factory, or default constructor.
        /// </summary>
        /// <param name="allowRetrieve"></param>
        /// <param name="createIfMissing"></param>
        /// <returns></returns>
        [ThreadSafe(false)] // TODO
        public async Task<(T value, bool gotValue)> GetWriteValue(bool allowRetrieve = true, bool createIfMissing = true, Func<IReference, T> factory = null)
        {
            var writeHandleCopy = WriteHandle;
            if (writeHandleCopy.HasValue) return (writeHandleCopy.Value, true);

            var readHandleCopy = ReadHandle;

            if (readHandleCopy.HasValue)
            {
                return (CloneValueToWriteHandleValue(readHandleCopy.Value), true);
            }
            else if (allowRetrieve)
            {
                var result = await ReadHandle.GetValue();

                if (result.HasValue)
                {
                    return (CloneValueToWriteHandleValue(result.Value), true);
                }
            }

            if (createIfMissing)
            {
                var createdValue = factory != null ? factory(Reference) : Activator.CreateInstance<T>();
                writeHandleCopy.Value = createdValue;
                return (createdValue, true);
            }

            return (default, false);
        }

        private void OnReadValueChanged(ValueChanged<T> obj)
        {
            if (writeHandle != null && !writeHandle.HasValue)
            {
                CloneValueToWriteHandleValue(obj.NewValue);
            }
        }

        #endregion

        public async Task Revert(bool getIfNeeded = true, bool forceRetrieve = false)
        {
            writeHandle.DiscardValue();

            if (forceRetrieve)
            {
                await ReadHandle?.Retrieve();
            }
            else if (getIfNeeded)
            {
                await ReadHandle.GetValue();
            }
        }

        #region Handle pass-thru

        public PersistenceState State => ReadHandle.State | WriteHandle.State; // TODO: Mask each state with read/write masks before combining them?

        public bool HasValue => readHandle?.Value || writeHandle?.HasValue;

        public T Value { get => ReadHandle.Value; set => WriteHandle.Value = value; }

        public void DiscardValue()
        {
            if (readHandle is ILazilyResolves<T> lr)
            {
                lr.DiscardValue();
            }
            writeHandle?.DiscardValue();
        }

        #endregion

        #region R<T>

        T IReadWrapper<T>.Value => ReadHandle.Value;

        #endregion

        #region WO<T>

        public bool GetLocalValueFromRemote => throw new NotImplementedException();

        T IWriteWrapper<T>.Value { set => WriteHandle.Value = value; }
        
        Task<IPutResult> IPuts.Put() => WriteHandle.Put();

        #endregion

    }

    /// <summary>
    /// Also see ReadWriteHandle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ReadWriteHandle"/>
    /// <seealso cref="ReadWriteHandlePairEx"/>
    public class ReadWriteHandlePairBase<T> : ResolvesInputBase<IReference>, IReadWriteHandlePairBase<T>, IResolveCommitPair<T>
    {

        #region Reference

        [SetOnce]
        public IReference Reference { get => Key; set => Key = value; }

        #endregion

        #region Construction

        public ReadWriteHandlePairBase() { }
        public ReadWriteHandlePairBase(IReference reference) : base(reference) { }

        #endregion

        #region ReadHandle

        public virtual RH<T> ReadHandle
        {
            get
            {
                if (readHandle == null)
                {
                    if (Reference != null)
                    {
                        readHandle = Reference.GetReadHandle<T>();
                    }
                }
                return readHandle;
            }
            protected set
            {
                readHandle = value;
            }
        }
        protected RH<T> readHandle;
        public bool HasReadHandle => readHandle != null;
        IResolves<T> IResolveCommitPair<T>.Resolves => ReadHandle;

        #endregion

        #region WriteHandle

        public virtual WO<T> WriteHandle
        {
            get
            {
                if (writeHandle == null)
                {
                    if (Reference != null)
                    {
                        writeHandle = Reference.GetWriteHandle<T>();
                    }
                }
                return writeHandle;
            }
            protected set => writeHandle = value;
        }
        protected WO<T> writeHandle;
        public bool HasWriteHandle => writeHandle != null;
        IPuts IResolveCommitPair<T>.Commits => WriteHandle;

        #endregion

        #region Dispose

        public override void Dispose()
        {
            base.Dispose();
            ReadHandle = null;
            WriteHandle = null;
        }

        #endregion
    }

}
