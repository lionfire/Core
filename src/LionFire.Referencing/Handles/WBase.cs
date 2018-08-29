using System.Diagnostics;
using System.Threading.Tasks;

namespace LionFire.Referencing
{
    public abstract class WBase<T> : RBase<T>, W<T>
        where T : class
    {
        protected override void OnObjectChanged()
        {
            if (_object == null)
            {
                DeletePending = true;
            }
        }


        #region DeletePending

        /// <summary>
        /// Next save will delete the underlying object
        /// </summary>
        public bool DeletePending
        {
            get => deletePending;
            set
            {
                if (deletePending == value)
                {
                    if (value)
                    {
                        if (Object != null)
                        {
                            Trace.TraceWarning("DeletePending set to true again.  Object was not null.  Setting Object to null again.");
                        }
                        Object = null;
                    }
                    return;
                }

                deletePending = value;

                if (value)
                {
                    Object = null;
                }
                else
                {
                    Trace.TraceWarning("DeletePending set to false from true.  Object was discarded.  There is currently no way to undiscard the Object.");
                }
            }
        }
        private bool deletePending;

        #endregion

        public async Task Save(object persistenceContext = null)
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

        public abstract Task WriteObject(object persistenceContext = null);
        public abstract Task DeleteObject(object persistenceContext = null);

        public void MarkDeleted()
        {
            this.Object = null;
            //DeletePending = true;
        }
    }
}
