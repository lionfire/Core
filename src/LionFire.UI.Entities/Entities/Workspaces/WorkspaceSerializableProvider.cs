#if FUTURE
#nullable enable
using System.Collections.Generic;

namespace LionFire.UI.Workspaces
{
//#error NEXT: Way to save/load things that have (ISerializableStrategy<T> or ISerializableProvider<T> or object) and IReferencable
    
//    /// <summary>
//    /// 
//    /// </summary>
//    public static class ObjectPersister
//    {
//        PersistenceResult<T> Upsert<T>(T r)
//            where T : IReferencable
//            => r.GetReadWriteHandle
//    }

    public interface ISerializableProvider<T> { 
        
    }

    public class WorkspaceSerializableProvider : ISerializableStrategy<IWorkspace>, ISerializableStrategy
    {
        public SerializableProvider WorkspaceStrategies { get; } = new SerializableProvider()
        {
            Strategies = new List<ISerializableStrategy>
            {
                new HasInstantiationSerializableStrategy(),
                new ObjectSerializableStrategy(),
            }
        };
        public SerializableProvider WorkspaceItemStrategies { get; } = new SerializableProvider()
        {
            Strategies = new List<ISerializableStrategy>
            {
                new HasInstantiationSerializableStrategy(),
                new ObjectSerializableStrategy(),
            }
        };


        public List<ISerializableStrategy> Strategies { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public bool CanGetSerializable(object obj, object? persistenceContext = null) => throw new System.NotImplementedException();
        public object GetSerializable(object obj, object? persistenceContext = null) => throw new System.NotImplementedException();
    }

    // TODO: Children Executable Visitor: attach to object, then on onstarting/onstopping, crawl thru the hierarchy
    // TODO: Init call a global executablemanager to say that an executable is intializing, to give it a chance to add state listeners

}
#endif