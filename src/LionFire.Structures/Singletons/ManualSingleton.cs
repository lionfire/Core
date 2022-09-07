using System;
using System.Threading;

namespace LionFire.Structures
{
    public static class ManualSingletonConfig
    {
        public static Func<Type, LazyThreadSafetyMode> GetLazyThreadSafetyMode = t => LazyThreadSafetyMode.ExecutionAndPublication;
    }

    public sealed class ManualSingleton<T>
    where T : class
    {
        static ManualSingleton()
        {
            ManualSingletonRegistrar.List.Add(typeof(T));
        }

        #region Instance

        public static T Instance
        {
            get => instance;
            set
            {
                if (instance == value) return;
                if (value != null && instance != default(T)) throw new NotSupportedException("Instance can only be set once or back to null.");
                instance = value;
            }
        }
        private static T instance;

        #endregion


        // See  http://csharpindepth.com/Articles/General/Singleton.aspx implementation #6

        public static T GuaranteedInstance
        {
            get
            {
                if (Instance == null)
                {
                    Instance = lazyGuaranteedInstance.Value;
                    //Instance = (T)ManualSingletonProvider.GuaranteedInstanceProvider(typeof(T));
                }
                return Instance;
            }
        }
        private static readonly Lazy<T> lazyGuaranteedInstance = new Lazy<T>(() => (T)ManualSingletonProvider.GuaranteedInstanceProvider(typeof(T)), ManualSingletonConfig.GetLazyThreadSafetyMode(typeof(T)));

        /// <param name="obj"></param>
        /// <returns>True if object was missing, and was set to obj.  False otherwise.</returns>
        public static bool SetIfMissing(T obj)
        {
            if (Instance == null)
            {
                Instance = obj;
                return true;
            }
            return false;
        }

        // REVIEW: Use GuaranteedInstance instead? (this is not threadsafe?)
        public static T GetGuaranteedInstance<CreateType>()
            where CreateType : class, T, new()
        {
            if (Instance == null)
            {
                Instance = new CreateType();
            }
            return Instance;
        }

        // REVIEW: Use GuaranteedInstance instead? (this is not threadsafe?)
        public static T GetGuaranteedInstance<CreateType>(Func<CreateType> createFunc)
            where CreateType : T
        {
            if (Instance == null)
            {
                Instance = createFunc();
            }
            return Instance;
        }

        // REVIEW: Use GuaranteedInstance instead? (this is not threadsafe?)
        public static T GetGuaranteedInstance(Func<object> createFunc)
        {
            if (Instance == null)
            {
                Instance = (T)createFunc(); // CAST
            }
            return Instance;
        }

    }
    public sealed class ManualSingleton
    {
        public static object GetInstance(object obj, Type type) => typeof(ManualSingleton<>).MakeGenericType(type).GetProperty("Instance").GetValue(null);
        public static void SetInstance(object obj, Type type) => typeof(ManualSingleton<>).MakeGenericType(type).GetProperty("Instance").SetValue(null, obj);
    }
}
