#if false

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Handles
{
    public abstract class WritableHandleBase<T> : 
        //ReadHandleBase<T>, 
        IWriteHandle<T>
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

        public void SetObject(T value)
        {
            base.Object = value;
            if (value == null)
            {
                DeletePending = true;
            }
        }


        public async Task Save(object persistenceContext = null)
        {
            if (DeletePending)
            {

                await DeleteObject(persistenceContext);
                DeletePending = false;
                return;
            }

            await WriteObject(persistenceContext);

        }

        public abstract Task WriteObject(object persistenceContext = null);
        public abstract Task DeleteObject(object persistenceContext = null);
    }
}
#endif