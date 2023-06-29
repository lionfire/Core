#if OLD
using LionFire.Persistence.Implementation;
using LionFire.Referencing;
using LionFire.Data.Gets;
using LionFire.Threading;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{
    /// <summary>
    /// Backing fields: none
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public abstract class ReadWriteHandleBaseEx<TValue> : ReadHandle<TValue>, IReadWriteHandle<TValue>
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
            get => Flags.HasFlag(PersistenceFlags.OutgoingDeletePending);
            set
            {
                if (value)
                {
                    Flags |= PersistenceFlags.OutgoingDeletePending;
                }
                else
                {
                    Flags &= ~PersistenceFlags.OutgoingDeletePending;
                }
            }
        }


        #endregion

        public async Task Commit() => (await ((ICommitableImpl)this).Commit()).ThrowIfUnsuccessful();

        //async Task<ITransferResult> ICommitableImpl.Commit()
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
        //async Task<ITransferResult> IDeletableImpl.Delete()
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
        protected abstract Task<ITransferResult> WriteObject();

        public virtual Task<ITransferResult> WriteObject(TValue @object)
        {
            this.Value = @object;
            return WriteObject();
        }


        #endregion

        protected abstract Task<ITransferResult> DeleteObject();
        Task<IPutResult> IPuts.Put() => throw new System.NotImplementedException();
    }
}
#endif