#if OLD
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Structures;
using System;

namespace LionFire.Persistence
{
    // TODO REFACTOR - try (again?) to replace this with IReadHandle<object>?

    [Obsolete("Use H<T>")] // TODO
    public interface IHandle : IChangeableReferencable,
        IHandlePersistence,  // Move this to extension methods?
        IHasHandle,
#if AOT
 IROStringKeyed  // String key for parent ITreeHandle tree collection
#else
	IKeyed<string>  // String key for parent ITreeHandle tree collection
#endif
        , RH
    {
#if AOT && true // AOTTEMP
#else
        object Object { get; set; }
#endif
        event ObjectChanged ObjectChanged;
        
    }
}
#endif