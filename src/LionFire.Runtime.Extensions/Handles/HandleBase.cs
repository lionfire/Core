using System;
using System.Threading.Tasks;

namespace LionFire.Handles
{
    public abstract class HandleBase<T> : ReadHandleBase<T>, IWriteHandle<T>
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


        T IWriteHandle<T>.Object
        {
            set
            {
                base.Object = value;
                if (value == null)
                {
                    DeletePending = true;
                }
            }
        }

        public abstract Task Save(object persistenceContext = null);
    }
}