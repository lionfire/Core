using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LionFire.DependencyInjection;
using LionFire.Ontology;
using LionFire.Persistence;
using LionFire.Referencing;

namespace LionFire.Vos
{

    [ReadOnlyEditionIs(typeof(VobReadHandle<>))]
    public class VobHandle<T> : VobReadHandle<T>, H<T>
        , IVobHandle<T>
    //, ITreeHandle
    {

        public override bool IsReadOnly => false;

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

        public VobHandle(Vob vob) : base(vob)
        {
        }

        /// <summary>
        /// Finds Vob using default available VBase.  Uses VosReference from that Vob, typed to T.
        /// </summary>
        /// <param name="vosReference"></param>
        public VobHandle(VosReference vosReference) : base(vosReference)
        {
        }

        /// <summary>
        /// Finds Vob using default available VBase.  Uses VosReference from that Vob, typed to T.
        /// </summary>
        /// <param name="reference">Currently must be of type VosReference.  (FUTURE: Allow reference types compatible with / convertible to VosReference)</param>
        public VobHandle(IReference reference) : base(reference)
        {
        }

#endregion

#region Object Construction



        // REVIEW - also include these in WBase? / Consider mixin strategy to allow some sort of reuse and effective multiple inheritance between custom read handles and base write handles

        public virtual async Task<T> TryGetOrCreate()
        {
            if (!HasValue)
            {
                await RetrieveImpl().ConfigureAwait(false);
                if (!HasValue) { Object = ReferenceObjectFactory.ConstructDefault<T>(Reference); }
            }
            return Object;
        }

        public void EnsureConstructed() // REVIEW: What should be done here?
        {
            //RetrieveOrCreateDefault(); ??

            if (Object == null)
            {
                Object = ReferenceObjectFactory.ConstructDefault<T>(Reference);
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
            get => State.HasFlag(PersistenceState.OutgoingDeletePending);
            set
            {
                if (value)
                {
                    State |= PersistenceState.OutgoingDeletePending;
                }
                else
                {
                    State &= ~PersistenceState.OutgoingDeletePending;
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
            this.Object = default(T);
            //OutgoingDeletePending = true;
        }

#endregion

#region Writable Handle Implementation

        public Task DeleteObject() => throw new NotImplementedException();
        public Task WriteObject() => throw new NotImplementedException();
        public Task<bool> Delete() => throw new NotImplementedException();

        public void OnRenamed(IVobHandle<T> newHandle) => throw new NotImplementedException();

        #endregion
    }
}
