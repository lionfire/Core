using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Referencing
{
    //public class LightHandle<T> : H<T>
    //{

    //}

    //public class LightReadHandle<T> : R<T>
    //{
    //}



    public interface R<out T> : IReadWrapper<T>, IReferencable
    {
        /// <summary>
        /// Raised if Object changes for any reason, such as: was retrieved from source, or was changed by user of handle.
        /// </summary>
        event Action<R<T> /* handle */ , T /*oldValue*/ , T /*newValue*/> ObjectChanged;

        /// <summary>
        /// True if successfully retrieved the Object or lack thereof.  HasObject may be false if the retrieval mechanism succeeded and found no object at the source location.
        /// </summary>
        bool IsResolved { get; }
        event Action<bool> IsResolvedChanged;

        /// <summary>
        /// Returns true if retrieval was attempted via get_Object or TryResolveObject, and a non-null object was retrieved.
        /// </summary>
        bool HasObject { get; }

        // REVIEW: Is persistenceContext helpful?
        Task<bool> TryResolveObject();
        //Task<bool> TryResolveObject(bool forgetOnFail = false);
        //Task<bool> TryResolveObject(object persistenceContext = null, bool forgetOnFail = false);
        void ForgetObject();
    }

}
