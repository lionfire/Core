using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using LionFire.Collections;
using LionFire.Structures;
using Microsoft.Extensions.Logging;
using LionFire.Dependencies;

namespace LionFire
{
    public class InterfaceTypeResolver
    {
        #region Static Accessor

        public static InterfaceTypeResolver Instance => Singleton<InterfaceTypeResolver>.Instance;

        #endregion

        #region (Private) Fields

        private readonly ConcurrentDictionary<Type, Type> interfaceToConcreteType = new ConcurrentDictionary<Type, Type>();

        #endregion

        public InterfaceTypeResolver()
        {
        }

        public void Register(Type concreteType, Type interfaceType = null)
        {
            if (interfaceType == null)
            {
                var interfaces = concreteType.GetInterfaces();
                if (interfaces.Length == 0)
                {
                    interfaceType = concreteType;
                }
                else if (interfaces.Length == 1)
                {
                    interfaceType = interfaces[0];
                }
                else
                {
                    throw new NotImplementedException("TODO: Auto-detect interface type(s) when there is more than one interface");
                }
            }
            //#if SanityChecks
            if (!interfaceType.IsAssignableFrom(concreteType)) { throw new ArgumentException("interfaceType must be assignable from concreteType."); }
            //#endif

            if (!interfaceToConcreteType.TryAdd(interfaceType, concreteType))
            {
                throw new AlreadyException(interfaceType.FullName);
            }
        }

        public Type TryResolve<InterfaceType>() => TryResolve(typeof(InterfaceType));

        public Type TryResolve(Type interfaceType)
        {
            var result = interfaceToConcreteType.TryGetValue(interfaceType);
            if (result == null) { l.Warn("Failed to resolve " + interfaceType.FullName);}
            return result;
        }

        public interfaceType CreateInterface<interfaceType>()
        {
            return (interfaceType)CreateInterface(typeof(interfaceType));
        }
        public object CreateInterface(Type interfaceType)
        {
            if (interfaceType == null) throw new ArgumentNullException();

            Type concreteType = TryResolve(interfaceType);
            return concreteType == null
                ? throw new ArgumentException("No type registered for Interface: " + interfaceType.FullName)
                : Activator.CreateInstance(concreteType);
        }

        #region Misc

        private static readonly ILogger l = Log.Get();

        #endregion
    }
}
