using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Resolves;
using LionFire.Results;
using LionFire.Structures;
using Microsoft.Extensions.Logging.EventSource;
using MorseCode.ITask;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{
    /// <summary>
    /// Convenience class that combines Reference and Handle
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TReference"></typeparam>
    public class ReadWriteHandlePassthrough<TValue, TReference> : IReadWriteHandle<TValue>, IReferencable<TReference>, IHasReadWriteHandle<TValue>
        where TReference : IReference<TValue>
    {
        #region Construction

        public ReadWriteHandlePassthrough() { }
        public ReadWriteHandlePassthrough(IReadWriteHandle<TValue> handle)
        {
            this.readWriteHandle = handle;
            Reference = (TReference)handle?.Reference;
        }

        #endregion


        public TReference Reference { get; set; }
        IReference<TValue> IReferencableAsValueType<TValue>.Reference => Reference;

        public IReadWriteHandle<TValue> ReadWriteHandle => readWriteHandle ??= Reference?.GetReadWriteHandle<TValue>();
        protected IReadWriteHandle<TValue> readWriteHandle;

        [Ignore]
        public TValue Value
        {
            get => ReadWriteHandle.Value; set
            {
                if (ReadWriteHandle != null)
                {
                    ReadWriteHandle.Value = value;
                }
                else
                {
                    if (Reference != null)
                    {
                        readWriteHandle = Reference.GetReadWriteHandlePreresolved<TValue>(value, overwriteValue: true).handle;
                    }
                    else
                    {
                        // ENH - optional ability to detect missing Reference at set-time?
                        //if (IsReferenceRequired)
                        //{
                        //    throw new Exception();
                        //}
                        readWriteHandle = value.GetObjectReadWriteHandle();
                    }
                }
            }
        }

        TValue IReadWrapper<TValue>.Value => ReadWriteHandle.Value;

        TValue IWriteWrapper<TValue>.Value { set => ReadWriteHandle.Value = value; }

        public string Key => ReadWriteHandle.Key;

        public bool HasValue => ReadWriteHandle.HasValue;

        public Type Type => ReadWriteHandle.Type;

        IReference IReferencable.Reference => ReadWriteHandle.Reference;

        public PersistenceFlags Flags => ReadWriteHandle.Flags;



        public ITask<ILazyResolveResult<TValue>> TryGetValue() => ReadWriteHandle.TryGetValue();
        public ITask<IResolveResult<TValue>> GetOrInstantiateValue() => ReadWriteHandle.GetOrInstantiateValue();
        public ILazyResolveResult<TValue> QueryValue() => ReadWriteHandle.QueryValue();
        public Task<ISuccessResult> Set(TValue value, CancellationToken cancellationToken = default) => ReadWriteHandle.Set(value, cancellationToken);
        public ITask<IResolveResult<TValue>> Resolve() => ReadWriteHandle.Resolve();
        public Task<ISuccessResult> Set() => ReadWriteHandle.Set();
        public Task<bool?> Delete() => ReadWriteHandle.Delete();
        public void MarkDeleted() => ReadWriteHandle.MarkDeleted();
        public void DiscardValue() => ReadWriteHandle.DiscardValue();
    }
}
