using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LionFire.Persistence.Handles
{

    public interface IObjectHandleProvider
    {
        IReadHandle<TValue> GetReadHandle<TValue>(TValue value);
        IReadWriteHandle<TValue> GetReadWriteHandle<TValue>(TValue value);
        IWriteHandle<TValue> GetWriteHandle<TValue>(TValue value);


        // TODO: Think more about API: 
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="TValue"></typeparam>
        ///// <param name="handle"></param>
        ///// <param name="replace"></param>
        ///// <returns>False if !handle.HasValue or if replace is false or is null and resolves to false and an existing handle is registered for the handle.Value</returns>
        //bool RegisterReadHandle<TValue>(IReadHandleBase<TValue> handle, bool? replace = null);
        void RegisterReadHandle<TValue>(IReadHandleBase<TValue> handle);
        void RegisterReadWriteHandle<TValue>(IReadWriteHandleBase<TValue> handle);
        void RegisterWriteHandle<TValue>(IWriteHandleBase<TValue> handle);
    }

    public class HandleEvents
    {
        public static HandleEvents Default => Singleton<HandleEvents>.Instance;

        public event Action<IHandleBase, object> HandleGotValue;
        public event Action<IHandleBase, object> HandleLostValue;
        public void OnHandleGotValue(IHandleBase handle, object value) => HandleGotValue?.Invoke(handle, value);
        public void OnHandleLostValue(IHandleBase handle, object value) => HandleGotValue?.Invoke(handle, value);
    }

#if FUTURE
    public class HandleAutoRegisterer
    {
        public HandleAutoRegisterer(IObjectHandleProvider objectHandleProvider)
        {
            HandleEvents.Default.HandleGotValue += Default_HandleGotValue;
            HandleEvents.Default.HandleLostValue += Default_HandleLostValue;
            ObjectHandleProvider = objectHandleProvider;
            r = ObjectHandleProvider.GetType().GetMethod(nameof(IObjectHandleProvider.RegisterReadHandle));
        }


        public IObjectHandleProvider ObjectHandleProvider { get; }
        private readonly MethodInfo r;

        private void Default_HandleLostValue(IHandleBase arg1, object arg2) => throw new NotImplementedException();
        private void Default_HandleGotValue(IHandleBase arg1, object arg2) => r.MakeGenericMethod(arg1.Type).Invoke(ObjectHandleProvider, new object[] { arg1 });
    }
#endif
}
