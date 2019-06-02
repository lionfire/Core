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

        #endregion
    }
}
