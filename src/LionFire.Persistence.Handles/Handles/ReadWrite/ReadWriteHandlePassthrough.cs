using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Resolves;
using LionFire.Results;
using LionFire.Structures;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{
    /// <summary>
    /// Convenience class that combines Reference and Handle
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TReference"></typeparam>
    public class ReadWriteHandlePassthrough<TValue, TReference> : IReadWriteHandle<TValue>, IReferencable<TReference>, IHasReadWriteHandle<TValue>
        where TReference : IReference
    {
        #region Construction

        public ReadWriteHandlePassthrough() { }
        public ReadWriteHandlePassthrough(IReadWriteHandle<TValue> handle)
        {
            this.handle = handle; 
            Reference = (TReference)handle?.Reference;
        }

        #endregion


        public TReference Reference { get; set; }

        public IReadWriteHandle<TValue> ReadWriteHandle => handle ??= Reference?.GetReadWriteHandle<TValue>();

        public TValue Value { get => ReadWriteHandle.Value; set => ReadWriteHandle.Value = value; }

        TValue IReadWrapper<TValue>.Value => ReadWriteHandle.Value;

        TValue IWriteWrapper<TValue>.Value { set => ReadWriteHandle.Value = value; }

        public string Key => ReadWriteHandle.Key;

        public bool HasValue => ReadWriteHandle.HasValue;

        public Type Type => ReadWriteHandle.Type;

        IReference IReferencable.Reference => ReadWriteHandle.Reference;

        public PersistenceFlags Flags => ReadWriteHandle.Flags;


        protected IReadWriteHandle<TValue> handle;

        public ITask<ILazyResolveResult<TValue>> GetValue() => ReadWriteHandle.GetValue();
        public ITask<IResolveResult<TValue>> GetOrInstantiateValue() => ReadWriteHandle.GetOrInstantiateValue();
        public ILazyResolveResult<TValue> QueryValue() => ReadWriteHandle.QueryValue();
        public Task<ISuccessResult> Put(TValue value) => ReadWriteHandle.Put(value);
        public ITask<IResolveResult<TValue>> Resolve() => ReadWriteHandle.Resolve();
        public Task<ISuccessResult> Put() => ReadWriteHandle.Put();
        public Task<bool?> Delete() => ReadWriteHandle.Delete();
        public void MarkDeleted() => ReadWriteHandle.MarkDeleted();
        public void DiscardValue() => ReadWriteHandle.DiscardValue();
    }
}
