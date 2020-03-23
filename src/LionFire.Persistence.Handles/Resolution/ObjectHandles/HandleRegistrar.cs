using System;
using System.Runtime.CompilerServices;

namespace LionFire.Persistence.Handles
{

    public abstract class HandleRegistrar<TOptions> // : IHandleRegistrar
        where TOptions : HandleRegistrarOptions
    {
        #region Fields

        ConditionalWeakTable<object, IHandleBase> readHandles = new ConditionalWeakTable<object, IHandleBase>();
        ConditionalWeakTable<object, IHandleBase> readWriteHandles = new ConditionalWeakTable<object, IHandleBase>();
        ConditionalWeakTable<object, IHandleBase> writeHandles = new ConditionalWeakTable<object, IHandleBase>();

        #endregion

        public TOptions Options { get; protected set; }

        public HandleRegistrar(TOptions options)
        {
            Options = options;
            if (Options.AutoRegister) throw new NotImplementedException("Cannot set AutoRegister ");
        }

        public void RegisterReadHandle<TValue>(IReadHandleBase<TValue> handle) => throw new NotImplementedException();
        public void RegisterReadWriteHandle<TValue>(IReadWriteHandleBase<TValue> handle) => throw new System.NotImplementedException();
        public void RegisterWriteHandle<TValue>(IWriteHandleBase<TValue> handle) => throw new System.NotImplementedException();

        #region TryRegister

        public bool TryRegisterReadHandle<T>(T obj, IReadHandle<T> handle) => TryRegister<T>(readHandles, obj, handle);
        public void TryRegisterReadWriteHandle<T>(T obj, IReadWriteHandle<T> handle) => TryRegister<T>(readWriteHandles, obj, handle);
        public void TryRegisterWriteHandle<T>(T obj, IWriteHandle<T> handle) => TryRegister<T>(writeHandles, obj, handle);

        public IReadHandle<T> TryRegisterOrGetReadHandle<T>(T obj, Func<T, IHandleBase> handleFactory) => (IReadHandle<T>)TryRegisterOrGet<T>(readHandles, obj, handleFactory);
        public IReadWriteHandle<T> TryRegisterOrGetReadWriteHandle<T>(T obj, Func<T, IHandleBase> handleFactory) => (IReadWriteHandle<T>)TryRegisterOrGet<T>(readWriteHandles, obj, handleFactory);
        public IWriteHandle<T> TryRegisterOrGetWriteHandle<T>(T obj, Func<T, IHandleBase> handleFactory) => (IWriteHandle<T>)TryRegisterOrGet<T>(writeHandles, obj, handleFactory);

        private bool TryRegister<T>(ConditionalWeakTable<object, IHandleBase> t, T obj, IHandleBase handle)
        {
            var result = TryRegisterOrGet<T>(t, obj, _ => handle);
            return ReferenceEquals(result, handle);
        }
        private IHandleBase TryRegisterOrGet<T>(ConditionalWeakTable<object, IHandleBase> t, T obj, Func<T, IHandleBase> handleFactory)
            => Options.AutoRegister
            ? t.GetValue(obj, new ConditionalWeakTable<object, IHandleBase>.CreateValueCallback(o => handleFactory((T)o)))
            : TryGetOrCreate(t, obj, handleFactory)
            ;
        private IHandleBase TryGetOrCreate<T>(ConditionalWeakTable<object, IHandleBase> t, T obj, Func<T, IHandleBase> handleFactory)
        {
            return t.TryGetValue(obj, out IHandleBase v) ? v : handleFactory(obj);
        }

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
            => Options.ReuseHandles
            ? (readHandles.TryGetValue(value, out IHandleBase handle)
                    ? (IReadHandle<TValue>)handle
                    : TryRegisterOrGetReadHandle<TValue>(value, o => CreateReadHandle<TValue>((TValue)o)))
            : CreateReadHandle<TValue>(value);

        public IReadWriteHandle<TValue> GetReadWriteHandle<TValue>(TValue value)
            => Options.ReuseHandles
            ? (readWriteHandles.TryGetValue(value, out IHandleBase handle)
                    ? (IReadWriteHandle<TValue>)handle
                    : TryRegisterOrGetReadWriteHandle<TValue>(value, o => CreateReadWriteHandle<TValue>((TValue)o)))
            : CreateReadWriteHandle<TValue>(value);

        public IWriteHandle<TValue> GetWriteHandle<TValue>(TValue value)
                => Options.ReuseHandles
            ? (readWriteHandles.TryGetValue(value, out IHandleBase handle)
                        ? (IWriteHandle<TValue>)handle
                        : TryRegisterOrGetWriteHandle<TValue>(value, o => CreateWriteHandle<TValue>((TValue)o)))
            : CreateWriteHandle<TValue>(value);

    }

}