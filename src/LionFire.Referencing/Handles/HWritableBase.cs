using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Referencing
{
    public abstract class HWritableBase<T> : HBase<T>, IWriteHandle<T>
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
                if (deletePending == value)
                {
                    if(value)
                    {
                        if(this.Object!=null)
                        {
                            Trace.TraceWarning("DeletePending set to true again.  Object was not null.  Setting Object to null again.");
                        }
                        this.Object = null;
                    }
                    return;
                }

                deletePending = value;

                if (value)
                {
                    this.Object = null;
                }
                else
                {
                    Trace.TraceWarning("DeletePending set to false from true.  Object was discarded.  There is currently no way to undiscard the Object.");
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
