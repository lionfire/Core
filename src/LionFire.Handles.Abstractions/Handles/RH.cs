using LionFire.Referencing;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    public delegate void PersistenceStateChangeHandler(PersistenceState from, PersistenceState to);

    public interface RH : RH<object>
    {
    }

    public interface RH<out T> : IReadWrapper<T>, IReferencable, IRetrievable, ILazyRetrievable, IHasPersistenceState
    {
        //public static bool ForgetObjectOnRetrieveFail = false; // FUTURE?

        #region Events

        event Action<RH<T>, HandleEvents> HandleEvents;

        #endregion

        #region Object

        /// <summary>
        /// Returns true if retrieval was attempted via get_Object or TryResolveObject, and a non-null object was retrieved.
        /// </summary>
        bool HasObject { get; }

        #region Events

        /// <summary>
        /// Raised if Object changes for any reason, such as: was retrieved from source, or was changed by user of handle.
        /// </summary>
        event Action<RH<T> /* handle */ , T /*oldValue*/ , T /*newValue*/> ObjectReferenceChanged;

        event Action<RH<T> /* handle */> ObjectChanged;

        #endregion

        #region Modify Object

        void ForgetObject(); // RENAME DiscardObject

        #endregion

        #endregion

        #region Retrieve

        /// <summary>
        /// Invokes get_Object, forcing a lazy retrieve if it was null and State does not have the NotFound flag (TOTEST).  Override to avoid this.
        /// If the user has set the object, then this will return true even if the object is not committed back to the source yet.
        /// </summary>
        /// <seealso cref="Exists"/>
        /// <returns>True if an object was found after a retrieval or was manually set on the handle, false otherwise (in which case State is set to |= PersistenceState.NotFound, if it doesn't already have that flag (TOTEST)).</returns>
        new Task<bool> TryGetObject();

        #endregion

    }
}
