using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Structures;

namespace LionFire // MOVE to LionFire.Referencing
{
    // TODO REFACTOR - try (again?) to replace this with IReadHandle<object>?
    public interface IHandle : IChangeableReferencable,
        IHandlePersistence,  // Move this to extension methods?
        IHasHandle,
#if AOT
 IROStringKeyed  // String key for parent ITreeHandle tree collection
#else
	IKeyed<string>  // String key for parent ITreeHandle tree collection
#endif
        , IReadHandle
    {
#if AOT && true // AOTTEMP
#else
        object Object { get; set; }
#endif
        event ObjectChanged ObjectChanged;
        
    }

}
