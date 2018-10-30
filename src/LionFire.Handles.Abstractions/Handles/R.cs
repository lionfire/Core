using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Referencing
{
    public delegate void PersistenceStateChangeHandler (PersistenceState from, PersistenceState to);

    public interface R : R<object>
    {
    }

    public interface R<out T> : IReadWrapper<T>, IReferencable
    {
        #region Persistence State

        PersistenceState State { get; }
        event PersistenceStateChangeHandler StateChanged;

        bool IsPersisted { get; }

        #endregion

        #region Events

        event Action<R<T>, HandleEvents> HandleEvents;

        #endregion

        #region Object

        #region Events

        /// <summary>
        /// Raised if Object changes for any reason, such as: was retrieved from source, or was changed by user of handle.
        /// </summary>
        event Action<R<T> /* handle */ , T /*oldValue*/ , T /*newValue*/> ObjectReferenceChanged;

        event Action<R<T> /* handle */> ObjectChanged;

        #endregion

        /// <summary>
        /// Returns true if retrieval was attempted via get_Object or TryResolveObject, and a non-null object was retrieved.
        /// </summary>
        bool HasObject { get; }

        #region Object Getters

        /// <summary>
        /// Invokes get_Object, forcing a lazy retrieve if it was null.  Override to avoid this.
        /// If the user has set the object, then this will return true even if the object is not committed back to the source yet.
        /// </summary>
        /// <seealso cref="Exists"/>
        /// <returns>True if an object was found after a retrieval or was manually set on the handle, false otherwise.</returns>
        Task<bool> TryGetObject();

        #endregion

        #region Modify Object

        void ForgetObject(); // RENAME DiscardObject

        #endregion

        #endregion

        #region Retrieve

        /// <summary>
        /// Force a retrieve of the reference from the source.  Replace the Object.
        /// </summary>
        /// <returns>true if an object was retrieved.  False if object was not found at location of the Reference.  Throws if could not resolve the Reference to a valid source.</returns>
        Task<bool> TryRetrieveObject(); 

        // REVIEW: Is persistenceContext helpful?
        //Task<bool> TryResolveObject(bool forgetOnFail = false);
        //Task<bool> TryResolveObject(object persistenceContext = null, bool forgetOnFail = false);

        /// <summary>
        /// Query whether the object exists at the target of the handle.
        /// If a retrieve was already attempted, this will return true or false depending on whether an object was found.
        /// Note: If the object has not been retrieved but the object has been set on the handle, this will still initiate
        /// a retrieve which if an object was found may result in a merge conflict (FUTURE).
        /// </summary>
        /// <seealso cref="TryGetObject"/>
        /// <returns>True if an object was found after a retrieval, false otherwise.</returns>
        Task<bool> Exists(bool forceCheck = false);

        #endregion
    }

}
