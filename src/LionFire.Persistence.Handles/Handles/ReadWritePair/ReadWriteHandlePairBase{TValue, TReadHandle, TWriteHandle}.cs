using LionFire.ExtensionMethods.Resolves;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Resolves;
using LionFire.Threading;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    //public abstract class ReadWriteHandlePairBase<T> : DisposableKeyed<IReference>
    //    , IReadWriteHandlePairBase<T>
    //    , IResolveCommitPair<T>
    //{
    //}

    // REVIEW: May not make sense to inherit from DisposableKeyed

    /// <summary>
    /// Also see ReadWriteHandle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ReadWriteHandle"/>
    /// <seealso cref="ReadWriteHandlePairEx"/>
    public class ReadWriteHandlePairBase<TReference, TValue, TReadHandle, TWriteHandle> : DisposableKeyed<TReference>
        , IResolveCommitPair<TValue>
        , IReadWriteHandlePairBase<TValue, TReadHandle, TWriteHandle>
        where TReference : IReference
        where TReadHandle : class, IReadHandleBase<TValue>
        where TWriteHandle : class, IWriteHandleBase<TValue>
        //where TValue : class
    {
        public Type Type => typeof(TValue);

        #region Reference

        [SetOnce]
        public TReference Reference { get => Key; set => Key = value; }
        IReference IReferencable.Reference => Key;

        #endregion

        #region Construction

        public ReadWriteHandlePairBase() { }
        public ReadWriteHandlePairBase(TReference reference) : base(reference) { }

        #endregion

        protected static TDerived UpCast<TSource, TDerived>(TSource source)
            where TDerived : TSource
        {
            if (source != null && !(source is IReadHandle<TValue>))
            {
                throw new Exception($"This object requires a value of type {typeof(TDerived).FullName} but got {source.GetType().FullName}.");
            }
            return (TDerived)source;
        }

        #region ReadHandle

        public virtual TReadHandle ReadHandle
        {
            get
            {
                if (readHandle == null)
                {
                    if (Reference != null)
                    {
                        readHandle = UpCast<IReadHandleBase<TValue>, TReadHandle>(Reference.GetReadHandle<TValue>());
                    }
                }
                return readHandle;
            }
            protected set
            {
                readHandle = value;
            }
        }
        protected TReadHandle readHandle;
        public bool HasReadHandle => readHandle != null;
        IResolves<TValue> IResolveCommitPair<TValue>.Resolves => ReadHandle;

        #endregion

        #region Write

        #region WriteHandle

        public virtual TWriteHandle WriteHandle
        {
            get
            {
                if (writeHandle == null)
                {
                    if (Reference != null)
                    {
                        writeHandle = (TWriteHandle) Reference.TryGetWriteHandle<TValue>(); // CAST
                    }
                }
                return writeHandle;
            }
            protected set => writeHandle = value;
        }
        protected TWriteHandle writeHandle;
        public bool HasWriteHandle => writeHandle != null;
        IPuts IResolveCommitPair<TValue>.Commits => WriteHandle;

        #endregion

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
        public async Task<(TValue value, bool gotValue)> GetWriteValue(bool allowRetrieve = true, bool createIfMissing = true, Func<IReference, TValue> factory = null)
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
                var createdValue = factory != null ? factory(Reference) : Activator.CreateInstance<TValue>();
                writeHandleCopy.Value = createdValue;
                return (createdValue, true);
            }

            return (default, false);
        }

        #endregion

        #region Change Detection

        public virtual bool HasChanges => Changes() == null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Null if no differences, otherwise an object describing the differences</returns>
        public virtual object Changes()
        {
            if (ReadHandle == null && WriteHandle == null) return null;
            if (ReadHandle == null && WriteHandle != null) return "ReadHandle == null && WriteHandle != null";
            if (ReadHandle != null && WriteHandle == null) return "ReadHandle != null && WriteHandle == null";

            var result = AnyDiff.AnyDiff.Diff(ReadHandle.Value, WriteHandle.Value);
            if (result.Count == 0)
            {
                return null;
            }
            return result;
        }

        #endregion

        #region Cloning

        public async Task<(TValue clonedValue, bool clonedSomething)> CloneGetReadHandleValueToWriteHandleValue(bool propagateNoValue = false)
            => _CloneGetOrQueryReadHandleValueToWriteHandleValue2(await ReadHandle.GetValue().ConfigureAwait(false), propagateNoValue);

        public (TValue clonedValue, bool clonedSomething) CloneQueryReadHandleValueToWriteHandleValue(bool propagateNoValue = false)
            => _CloneGetOrQueryReadHandleValueToWriteHandleValue2(!HasReadHandle ? null : ReadHandle.QueryValue(), propagateNoValue);

        private (TValue clonedValue, bool clonedSomething) _CloneGetOrQueryReadHandleValueToWriteHandleValue2(IResolveResult<TValue> getOrQueryResult, bool propagateNoValue)
        {
            if (getOrQueryResult != null && getOrQueryResult.HasValue)
            {
                return (CloneValueToWriteHandleValue(getOrQueryResult.Value), true);
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

        public virtual TValue CloneValueToWriteHandleValue(TValue value)
        {
            var clonedValue = AnyClone.Cloner.Clone(value);
            WriteHandle.Value = clonedValue;
            return clonedValue;
        }

        #endregion

        #region Revert

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
