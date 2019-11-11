using LionFire.Persistence.Handles;
using LionFire.Referencing;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence
{

    /// <summary>
    ///  - Events for detecting deep change tracking (TODO)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadWriteHandlePairEx<T> : IReadWriteHandlePair<T>
    {

        event Action<object> HasChangesChanged;
        bool CanDetectDeepChanges { get; }
    }

    public interface IReadWriteHandlePairBase<T> : IReadWriteHandlePairBase<T, IReadHandleBase<T>, IWriteHandleBase<T>>
    {
        #region Changes

        bool HasChanges { get; }

        /// <returns>Null if no differences, otherwise an object describing the differences</returns>
        object Changes();

        #endregion

        /// <summary>
        /// Revert (forget) changes to the local handle, replacing the LocalHandle's Value with a cloned copy of the last-known RemoteHandle's Value (if any).
        /// </summary>
        /// <param name="getIfNeeded"></param>
        /// <param name="forceRetrieve"></param>
        Task Revert(bool getIfNeeded = true, bool forceRetrieve = false);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The cloned value that was set on WriteHandle.Value, (or a non-cloned default(T) if NoValue was propagated.)</returns>
        Task<(T clonedValue, bool clonedSomething)> CloneGetReadHandleValueToWriteHandleValue(bool propagateNoValue = false);
        (T clonedValue, bool clonedSomething) CloneQueryReadHandleValueToWriteHandleValue(bool propagateNoValue = false);

        Task<(T value, bool gotValue)> GetWriteValue(bool allowRetrieve = true, bool createIfMissing = true, Func<IReference, T> factory = null);
    }


    public interface IReadWriteHandlePair<T> : IReadWriteHandlePairBase<T, IReadHandle<T>, IWriteHandle<T>>
    {

        //new IReadHandle<T> ReadHandle { get; }
        //new IWriteHandle<T> WriteHandle { get; }

        ///// <summary> // REVIEW - should this be a get property?
        ///// If true, when the WriteHandle.Value is requested, it will attempt to initialize it with a clone of the ReadHandle.Value as a starting point.
        ///// </summary>
        //bool GetWriteValueFromReadHandle { get; }
        

        
    }
}

