using System;
using System.Reflection;
using LionFire.Collections.Concurrent;

namespace LionFire.Structures
{

    public static class ManualSingletonRegistrar
    {
        public static ConcurrentList<Type> List = new ConcurrentList<Type>();

        public static void ResetAll()
        {
            foreach(var type in List)
            {
                typeof(ManualSingleton<>).MakeGenericType(type).GetProperty("Instance").SetValue(null, null);
            }   
        }
    }

    public sealed class ManualSingleton<T>
        where T : class
    {
        static ManualSingleton()
        {
            ManualSingletonRegistrar.List.Add(typeof(T));
        }

        public static T Instance { get; set; }
        public static T GuaranteedInstance
        {
            get
            {
                if (Instance == null)
                {
                    var createType = typeof(T);

                    if (typeof(T).GetTypeInfo().IsAbstract || typeof(T).GetTypeInfo().IsInterface)
                    {
                        var attr = typeof(T).GetTypeInfo().GetCustomAttribute<DefaultImplementationTypeAttribute>();
                        if (attr != null)
                        {
                            createType = attr.Type;
                        }
                        else
                        {
                            createType = null;
                        }

                        var sType = typeof(ManualSingleton<>).MakeGenericType(createType);

                        var sTypeInstance = (T)sType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(null);
                        if (sTypeInstance != null)
                        {
                            Instance = sTypeInstance;
                            return Instance;
                        }
                    }
                    if (createType != null)
                    {
                        try
                        {
                            Instance = (T)Activator.CreateInstance(createType);
                        }
                        catch (MissingMethodException mme)
                        {
                            throw new Exception("Missing method when creating instance of " + createType.Name + (createType != typeof(T) ? $" for {typeof(T).Name}" : ""), mme);
                        }
                    }
                }
                return Instance;
            }
        }

        public static void SetIfMissing(T obj)
        {
            if (Instance == null)
            {
                Instance = obj;
            }
        }

        public static T GetGuaranteedInstance<CreateType>()
            where CreateType : class, T, new()
        {
            if (Instance == null)
            {
                Instance = new CreateType();
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
