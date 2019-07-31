using System.Diagnostics;
using System.Threading.Tasks;

namespace LionFire.Referencing
{
    /// <summary>
    /// Backing fields: none
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class WBase<T> : RBase<T>, H<T>
    //where T : class
    {
        // TODO: Delete on set to empty?  Should it be an Option, or per OBase or per handle?
        //protected override void OnObjectChanged()
        //{
        //    if (_object == null)
        //    {
        //        DeletePending = true;
        //    }
        //    base.OnO
        //}

        #region Construction

        public WBase() { }
        public WBase(IReference reference) : base(reference) { }

        #endregion

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

        protected abstract Task WriteObject(object persistenceContext = null);
        protected abstract Task<bool?> DeleteObject(object persistenceContext = null);

        public async Task<bool?> Delete() => await DeleteObject();

        public async Task<bool?> Delete(object persistenceContext = null)
        {
            this.Object = default(T);
            var result = await DeleteObject(persistenceContext);
            DeletePending = false;
            return result;
        }

        public void MarkDeleted()
        {
            this.Object = default(T);
            DeletePending = true;
        }
    }
}
