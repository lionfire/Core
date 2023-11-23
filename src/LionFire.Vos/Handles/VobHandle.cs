#if DISABLED
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LionFire.Dependencies;
using LionFire.Ontology;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Data.Async.Gets;

namespace LionFire.Vos
{

    [ReadOnlyEditionIs(typeof(VobReadHandle<>))]
    public class VobHandle<T> : ReadWriteHandle<VobReference, T>
        , IVobHandle<T>
    //, ITreeHandle
    {

        public string Path => Reference?.Path;

        //public override bool IsReadOnly => false;

        #region Construction Operators

        public static implicit operator VobHandle<T>(Vob vob) => vob.GetHandle<T>();

        public static explicit operator VobHandle<T>(string path) => DependencyContext.Current.GetService<IVBase>()[path].GetHandle<T>();


#if ALLOW_UNPATHED_VOBHANDLES
        public static explicit operator VobHandle<ObjectType>(ObjectType obj)
        {
            var result = new VobHandle<ObjectType>(obj);
            return result;
        }
#endif

        #endregion

        #region Construction

        // Pass-through to base class

        public VobHandle(Vob vob) : base(vob.VobReference)
        {
        }

        /// <summary>
        /// Finds Vob using default available VBase.  Uses VobReference from that Vob, typed to T.
        /// </summary>
        /// <param name="vobReference"></param>
        public VobHandle(VobReference vobReference) : base(vobReference)
        {
        }

        ///// <summary>
        ///// Finds Vob using default available VBase.  Uses VobReference from that Vob, typed to T.
        ///// </summary>
        ///// <param name="reference">Currently must be of type VobReference.  (FUTURE: Allow reference types compatible with / convertible to VobReference)</param>
        //public VobHandle(IReference reference) : base((VobReference)reference)
        //{
        //}

        #endregion

        #region Object Construction



        // REVIEW - also include these in WBase? / Consider mixin strategy to allow some sort of reuse and effective multiple inheritance between custom read handles and base write handles

        public virtual async Task<T> TryGetOrCreate()
        {
            if (!HasValue)
            {
                await GetImpl().ConfigureAwait(false);
                if (!HasValue) { Value = ReferenceObjectFactory.ConstructDefault<T>(Reference); }
            }
            return Value;
        }

        public void EnsureConstructed() // REVIEW: What should be done here?
        {
            //RetrieveOrCreateDefault(); ??

            if (Value == null)
            {
                Value = ReferenceObjectFactory.ConstructDefault<T>(Reference);
            }
        }

#if UNUSED
        private void EnsureConstructedNoEvents() // REVIEW: What should be done here?
        {
            if (_object == null)
            {
                _object = CreateDefaultUtils.ConstructDefault<T>(Reference);
            }
        }
#endif

        #endregion

        #region Duplicate from WBase

        #region DeletePending

        /// <summary>
        /// Next save will delete the underlying object
        /// </summary>
        public bool DeletePending
        {
            get => Flags.HasFlag(PersistenceFlags.OutgoingDeletePending);
            set
            {
                if (value)
                {
                    Flags |= PersistenceFlags.OutgoingDeletePending;
                }
                else
                {
                    Flags &= ~PersistenceFlags.OutgoingDeletePending;
                }
            }
        }

        IVob IVobHandle<T>.Vob => throw new NotImplementedException();

        #endregion

        public async Task Commit()
        {
            if (DeletePending)
            {
                await DeleteObject();
                DeletePending = false;
            }
            else
            {
                await WriteObject();
            }
        }

        public void MarkDeleted()
        {
            this.Value = default(T);
            //OutgoingDeletePending = true;
        }

        #endregion

        #region Writable Handle Implementation

        public Task DeleteObject() => throw new NotImplementedException();
        public Task WriteObject() => throw new NotImplementedException();
        public Task<bool> Delete() => throw new NotImplementedException();

        public void OnRenamed(IVobHandle<T> newHandle) => throw new NotImplementedException();
        protected override Task<IGetResult<T>> GetImpl(CancellationToken cancellationToken = default) => throw new NotImplementedException();
        protected override Task<ITransferResult> UpsertImpl() => throw new NotImplementedException();
        public override IGetResult<T> QueryValue() => throw new NotImplementedException();
        public override void RaisePersistenceEvent(PersistenceEvent<T> ev) => throw new NotImplementedException();

        #endregion
    }
}
#endif