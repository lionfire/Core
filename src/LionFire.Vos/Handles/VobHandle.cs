using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LionFire.DependencyInjection;
using LionFire.Ontology;
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
            if (!HasObject)
            {
                await TryRetrieveObject().ConfigureAwait(false);
                if (!HasObject) { Object = ReferenceObjectFactory.ConstructDefault<T>(Reference); }
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
            get => State.HasFlag(PersistenceState.Persisted);
            set
            {
                if (value)
                {
                    State |= PersistenceState.DeletePending;
                }
                else
                {
                    State &= ~PersistenceState.DeletePending;
                }
            }
        }

        IVob IVobHandle<T>.Vob => throw new NotImplementedException();

#endregion

        public async Task Commit(object persistenceContext = null)
        {
            if (DeletePending)
            {
                await DeleteObject(persistenceContext);
                DeletePending = false;
            }
            else
            {
                await WriteObject(persistenceContext);
            }
        }

        public void MarkDeleted()
        {
            this.Object = default(T);
            //DeletePending = true;
        }

#endregion

#region Writable Handle Implementation

        public Task DeleteObject(object persistenceContext = null) => throw new NotImplementedException();
        public Task WriteObject(object persistenceContext = null) => throw new NotImplementedException();
        public Task<bool?> Delete() => throw new NotImplementedException();
        public void OnRenamed(IVobHandle<T> newHandle) => throw new NotImplementedException();

#endregion
    }
}
