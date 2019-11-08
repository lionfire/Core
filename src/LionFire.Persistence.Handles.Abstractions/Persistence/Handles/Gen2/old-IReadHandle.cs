#if OLD
//using LionFire.Persistence;
//using LionFire.Structures;
//using System;
//using System.Collections.Generic;
//using System.Linq;

using System;

namespace LionFire
{

    //    // REVIEW: Consider leaning on covariance and IReadHandle<object> everywhere instead of this?
    [Obsolete("Use RH?")]
    public interface IReadHandle : IResolvableHandle
    //    //, ITreeHandlePersistence  OLD
    {
        //        object Object { get; }
        //        event Action<IReadHandle<T> /* Wrapper */ , T /*oldValue*/ , T /*newValue*/> ObjectChanged;
    }
}
#endif