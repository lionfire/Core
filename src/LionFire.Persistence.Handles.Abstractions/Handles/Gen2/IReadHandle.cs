//using LionFire.Persistence;
//using LionFire.Structures;
//using System;
//using System.Collections.Generic;
//using System.Linq;

namespace LionFire
{

    //    // REVIEW: Consider leaning on covariance and IReadHandle<object> everywhere instead of this?
    public interface IReadHandle : IResolvableHandle
    //    //, ITreeHandlePersistence  OLD
    {
        //        object Object { get; }
        //        event Action<IReadHandle<T> /* Wrapper */ , T /*oldValue*/ , T /*newValue*/> ObjectChanged;
    }
}
