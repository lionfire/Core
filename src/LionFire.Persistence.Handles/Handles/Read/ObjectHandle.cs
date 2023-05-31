using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LionFire.Persistence.Handles;
using LionFire.Structures;
using LionFire.Referencing;
using LionFire.Resolves;
using MorseCode.ITask;
using LionFire.Results;
using System.Threading;

namespace LionFire.Persistence.Handles
{
    /// <summary>
    /// REVIEW: Incomplete? / needs design analysis
    /// Reference type: NamedReference
    /// Read-only Handles to .NET object references (can be null) -- can be named and then retrieved by the handle registry system.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class ObjectHandle<TValue> : ReadHandleBase<NamedReference<TValue>, TValue>, IReadWriteHandle<TValue>
    {

        IReference<TValue> IReferencableAsValueType<TValue>.Reference => Reference;

        string IKeyed<string>.Key => Reference?.Key;

        void ThrowCannotSet() => throw new InvalidOperationException("Cannot set the value on an ObjectHandle after creation");

        TValue IWrapper<TValue>.Value
        {
            get
            {
                if (isDisposed) throw new ObjectDisposedException("");
                return ProtectedValue;
            }
            set => ThrowCannotSet();
        }
        TValue IWriteWrapper<TValue>.Value { set => ThrowCannotSet(); }

        protected override ITask<IResolveResult<TValue>> ResolveImpl()
        {
            if (HasValue)
            {
                return Task.FromResult<IResolveResult<TValue>>(new RetrieveResult<TValue>()
                {
                    Flags = PersistenceResultFlags.Success,
                    Value = Value,
                }).AsITask();
            }
            else
            {
                return Task.FromResult<IResolveResult<TValue>>(RetrieveResult<TValue>.NotFound).AsITask();
            }
        }

        public Task<ISuccessResult> Set(TValue value, CancellationToken cancellationToken = default)
        {
            if (!ReferenceEquals(value, ProtectedValue))
            {
                ThrowCannotSet();
            }
            return Task.FromResult(NoopSuccessResult.Instance);
        }

        public Task<ISuccessResult> Set()
        {
            if (isDeletePending)
            {
                Delete();
                return Task.FromResult(SuccessResult.Success);
            }
            else
            {
                return Task.FromResult(NoopSuccessResult.Instance);
            }
        }
        public Task<bool?> Delete()
        {
            Dispose();
            return Task.FromResult<bool?>(true);
        }
        public void MarkDeleted() => isDeletePending = true;

        public ITask<IResolveResult<TValue>> GetOrInstantiateValue()
        {
            if (HasValue) return Task.FromResult((IResolveResult<TValue>)RetrieveResult<TValue>.Noop(Value)).AsITask();
            ProtectedValue = Activator.CreateInstance<TValue>();
            return Task.FromResult((IResolveResult<TValue>)new RetrieveResult<TValue>(Value, PersistenceResultFlags.Success | PersistenceResultFlags.Instantiated)).AsITask();
        }

        bool isDeletePending = false;

        #region Construction

        public ObjectHandle() { }

        public ObjectHandle(TValue initialValue)
        {
            ProtectedValue = initialValue;
        }

        public ObjectHandle(NamedReference<TValue> reference, TValue initialValue = default) : base(reference)
        {
            if (!EqualityComparer<TValue>.Default.Equals(initialValue, default))
            {
                ProtectedValue = initialValue;
            }
        }

        #endregion
    }
}
