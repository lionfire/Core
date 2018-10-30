using System.Diagnostics;
using System.Threading.Tasks;

namespace LionFire.Referencing
{
    /// <summary>
    /// Backing fields: none
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class WBase<T> : RBase<T>, W<T>
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
            this.Object = default(T);
            //DeletePending = true;
        }
    }
}
