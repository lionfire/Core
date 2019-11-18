using System;
using System.Reflection;

namespace LionFire.Structures
{
    public sealed class ManualSingletonProvider
    {
        public static Func<Type, object> GuaranteedInstanceProvider = DefaultGuaranteedInstanceProviderMethod;

        public static object SimpleDefaultGuaranteedInstanceProviderMethod(Type createType)
        {
            return Activator.CreateInstance(createType);
        }

        /// <summary>
        /// For concrete types, returns Activator.CreateInstance(createType).
        /// For interface and abstract types, it will look for a concrete type in the [DefaultImplementationType()] attribute and if found, return the GuaranteedInstance for that type.
        /// </summary>
        /// <param name="createType">The type for which to get a guaranteed singleton instance of.</param>
        /// <returns>For concrete types: a newly constructed instance.  For interface and abstract types: the GuaranteedInstance for the associated concrete type, if any.</returns>
        public static object DefaultGuaranteedInstanceProviderMethod(Type createType)
        {

            if (createType.GetTypeInfo().IsAbstract || createType.GetTypeInfo().IsInterface)
            {
                var attr = createType.GetTypeInfo().GetCustomAttribute<DefaultImplementationTypeAttribute>();
                if (attr != null)
                {
                    createType = attr.Type;
                }
                else
                {
                    createType = null;
                }

                var sType = typeof(ManualSingleton<>).MakeGenericType(createType);

                var sTypeInstance = sType.GetProperty("GuaranteedInstance", BindingFlags.Static | BindingFlags.Public).GetValue(null);
                if (sTypeInstance != null)
                {
                    return sTypeInstance;
                }
            }

            if (createType != null)
            {
                try
                {
                    return Activator.CreateInstance(createType);
                }
                catch (MissingMethodException mme)
                {
                    throw new Exception("Missing method when creating instance of " + createType.Name, mme);
                    //throw new Exception("Missing method when creating instance of " + createType.Name + (createType != typeof(T) ? $" for {typeof(T).Name}" : ""), mme);
                }
            }
            return null;
        }
    }
}
