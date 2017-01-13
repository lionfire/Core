using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace LionFire.Structures
{
    public sealed class ManualSingleton<T>
        where T : class
    {

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
                        var attr = typeof(T).GetTypeInfo().GetCustomAttribute<DefaultConcreteTypeAttribute>();
                        if (attr != null)
                        {
                            createType = attr.Type;
                        }
                        else
                        {
                            createType = null;
                        }
                    }
                    if (createType != null)
                    {
                        Instance = (T)Activator.CreateInstance(createType);
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

}
