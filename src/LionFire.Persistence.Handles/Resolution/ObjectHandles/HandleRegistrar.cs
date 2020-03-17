using System.Runtime.CompilerServices;

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

    public abstract class HandleRegistrar // : IHandleRegistrar
    {
        #region Fields

        ConditionalWeakTable<object, IHandleBase> readHandles = new ConditionalWeakTable<object, IHandleBase>();
        ConditionalWeakTable<object, IHandleBase> readWriteHandles = new ConditionalWeakTable<object, IHandleBase>();
        ConditionalWeakTable<object, IHandleBase> writeHandles = new ConditionalWeakTable<object, IHandleBase>();

        #endregion

        #region TryRegister

        public bool TryRegisterReadHandle<T>(T obj, IReadHandle<T> handle) => TryRegister(readHandles, obj, handle);
        public void TryRegisterReadWriteHandle<T>(T obj, IReadWriteHandle<T> handle) => TryRegister(readWriteHandles, obj, handle);
        public void TryRegisterWriteHandle<T>(T obj, IWriteHandle<T> handle) => TryRegister(writeHandles, obj, handle);

        public IReadHandle<T> TryRegisterOrGetReadHandle<T>(T obj, IReadHandle<T> handle) => (IReadHandle<T>)TryRegisterOrGet(readHandles, obj, handle);
        public IReadWriteHandle<T> TryRegisterOrGetReadWriteHandle<T>(T obj, IReadWriteHandle<T> handle) => (IReadWriteHandle<T>)TryRegisterOrGet(readWriteHandles, obj, handle);
        public IWriteHandle<T> TryRegisterOrGetWriteHandle<T>(T obj, IWriteHandle<T> handle) => (IWriteHandle<T>)TryRegisterOrGet(writeHandles, obj, handle);

        private bool TryRegister(ConditionalWeakTable<object, IHandleBase> t, object obj, IHandleBase handle)
        {
            var result = TryRegisterOrGet(t, obj, handle);
            return ReferenceEquals(result, handle);
        }
        private IHandleBase TryRegisterOrGet(ConditionalWeakTable<object, IHandleBase> t, object obj, IHandleBase handle)
            => t.GetValue(obj, new ConditionalWeakTable<object, IHandleBase>.CreateValueCallback(o => handle));

        #endregion

        #region (abstract) Create handle

        protected abstract IReadHandle<TValue> CreateReadHandle<TValue>(TValue obj);
        protected abstract IReadWriteHandle<TValue> CreateReadWriteHandle<TValue>(TValue obj);
        protected abstract IWriteHandle<TValue> CreateWriteHandle<TValue>(TValue obj);

        #endregion

        // Possible options:
        // - fall back to ObjectHandle
        //   - optional register ObjectHandle 
        public IReadHandle<TValue> GetReadHandle<TValue>(TValue value)
            => readHandles.TryGetValue(value, out IHandleBase handle)
                    ? (IReadHandle<TValue>)handle
                    : TryRegisterOrGetReadHandle(value, CreateReadHandle<TValue>(value));

        public IReadWriteHandle<TValue> GetReadWriteHandle<TValue>(TValue value)
            => readWriteHandles.TryGetValue(value, out IHandleBase handle)
                    ? (IReadWriteHandle<TValue>)handle
                    : TryRegisterOrGetReadWriteHandle(value, CreateReadWriteHandle<TValue>(value));

        public IWriteHandle<TValue> GetWriteHandle<TValue>(TValue value)
                => readWriteHandles.TryGetValue(value, out IHandleBase handle)
                        ? (IWriteHandle<TValue>)handle
                        : TryRegisterOrGetWriteHandle(value, CreateWriteHandle<TValue>(value));

    }

}