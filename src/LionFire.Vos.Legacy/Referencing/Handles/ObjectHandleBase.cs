using System;
using System.Threading.Tasks;

namespace LionFire.Handles
{
    // UNUSED?
    // TODO: Reconcile with Legacy  HandleBase
    public abstract class ObjectHandleBase<T> : ReadHandleBase<T>, IWriteHandle<T>
        where T : class
    {

        #region DeletePending

        /// <summary>
        /// Next save will delete the underlying object
        /// </summary>
        public bool DeletePending
        {
            get { return deletePending; }
            set
            {
                deletePending = value;
                if (value)
                {
                    _object = null;
                }
            }
        }
        private bool deletePending;

        #endregion


        void IWriteHandle<T>.SetObject(T value)
        {
            base.Object = value;
            if (value == null)
            {
                DeletePending = true;
            }
        }

        public abstract Task Save(object persistenceContext = null);
    }
}