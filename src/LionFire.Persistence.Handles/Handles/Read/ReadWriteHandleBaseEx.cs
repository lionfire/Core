using LionFire.Persistence.Implementation;
using LionFire.Referencing;
using LionFire.Resolves;
using LionFire.Threading;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{
    /// <summary>
    /// Backing fields: none
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public abstract class ReadWriteHandleBaseEx<TValue> : ReadHandle<TValue>, Wx<TValue>
    //where T : class
    {
        internal static readonly bool DeleteIfObjectNull = true; // TODO: Decide how to configure/hardcode this

        #region Construction

        public ReadWriteHandleBaseEx() { }
        public ReadWriteHandleBaseEx(IReference reference, TValue handleObject = default) : base(reference, handleObject) { }

        #endregion

        #region Events (TODO)


        //event Action<RH<T>, HandleEvents> RH<T>.HandleEvents
        //{
        //    add
        //    {
        //    }

        //    remove
        //    {
        //    }
        //}

        //event Action<RH<T>, T, T> RH<T>.ObjectReferenceChanged
        //{
        //    add
        //    {
        //    }

        //    remove
        //    {
        //    }
        //}

        //event Action<RH<T>> RH<T>.ObjectChanged
        //{
        //    add
        //    {
        //    }

        //    remove
        //    {
        //    }
        //}
        #endregion

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


        #endregion

        public async Task Commit() => (await ((ICommitableImpl)this).Commit()).ThrowIfUnsuccessful();

        //async Task<IPersistenceResult> ICommitableImpl.Commit()
        //{
        //    if (DeletePending || (DeleteIfObjectNull && _value == null))
        //    {
        //        var result = await DeleteObject();
        //        DeletePending = false;
        //        return result;
        //    }
        //    else
        //    {
        //        return await WriteObject();
        //    }
        //}

        public async Task<bool> Delete() => (await ((IDeletableImpl)this).Delete()).IsSuccess();

        //[ThreadSafe(false)]
        //async Task<IPersistenceResult> IDeletableImpl.Delete()
        //{
        //    this.Value = default;
        //    DeletePending = true;
        //    var result = await DeleteObject();
        //    DeletePending = false;
        //    return result;
        //}

        public void MarkDeleted()
        {
            this.Value = default;
            DeletePending = true;
        }

        #region Abstract

        /// <summary>
        /// If !HasObject, do nothing (REVIEW/confirm this)
        /// </summary>
        /// <returns></returns>
        protected abstract Task<IPersistenceResult> WriteObject();

        public virtual Task<IPersistenceResult> WriteObject(TValue @object)
        {
            this.Value = @object;
            return WriteObject();
        }


        #endregion

        protected abstract Task<IPersistenceResult> DeleteObject();
        Task<IPutResult> IPuts.Put() => throw new System.NotImplementedException();
    }
}
