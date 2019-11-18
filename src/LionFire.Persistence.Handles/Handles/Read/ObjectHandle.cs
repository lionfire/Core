using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LionFire.Persistence.Handles;
using LionFire.Structures;
using LionFire.Referencing;
using LionFire.Resolves;
using MorseCode.ITask;

namespace LionFire.Persistence.Handles
{
    /// <summary>
    /// REVIEW: Incomplete? / needs design analysis
    /// Reference type: NamedReference
    /// Read-only Handles to .NET object references (can be null) -- can be named and then retrieved by the handle registry system.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class ObjectHandle<TValue> : ReadHandleBase<NamedReference, TValue>
        where TValue : class
    {
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

        #region Construction

        public ObjectHandle() { }

        public ObjectHandle(TValue initialValue) {
            ProtectedValue = initialValue;
        }

        public ObjectHandle(NamedReference reference, TValue initialValue = null) : base(reference)
        {
            if (initialValue != default)
            {
                ProtectedValue = initialValue;
            }
        }

        #endregion
    }
}
