using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Collections;
using LionFire.Types;
using LionFire.Persistence;
using LionFire.Structures;
using LionFire.MultiTyping;

namespace LionFire.ObjectBus
{
    
    //public interface IRHTEntity : IReferencable
    //{
    //    TEntity Object { get; }
    //    bool HasObject { get; }
    //}

    public interface IHandle : IChangeableReferencable,
        IHandlePersistence,  // Move this to extension methods?
        IHasHandle,
#if AOT
 IROStringKeyed  // String key for parent ITreeHandle tree collection
#else
	IKeyed<string>  // String key for parent ITreeHandle tree collection
#endif
        , ITreeHandle, ITreeHandlePersistence
        , IMultiTyped
        , IReadHandle
    {
#if AOT && true // AOTTEMP
#else
        new object Object { get; set; }
#endif
        event ObjectChanged ObjectChanged;
    }

}
