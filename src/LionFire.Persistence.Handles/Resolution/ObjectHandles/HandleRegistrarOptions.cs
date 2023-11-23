namespace LionFire.Persistence.Handles
{
#if TODO // OPTIMIZATION - prevent creating unnecessary ObjectHandles, and also provide a gateway to get references and in turn proper handles

    public interface IHandleRegistrar : IObjectHandleProvider
    {

        bool TryRegisterReadHandle<T>(T obj, IReadHandle<T> handle);
        bool TryRegisterWriteHandle<T>(T obj, IReadHandle<T> handle);
        bool TryRegisterReadWriteHandle<T>(T obj, IReadHandle<T> handle);
    }

#endif

    public class HandleRegistrarOptions
    {
        public bool ReuseHandles { get; set; }
        public bool AutoRegister { get; set; }
        public bool CheckIReferencable { get; set; }

        //public bool WeakHandleReferences { get; set; } // Is this doable? A good idea?
        //public bool WeakValueReferences { get; set; } // Is this doable? A good idea?
    }

}