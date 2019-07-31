#define TRACE_REPLACE // TEMP
//#define TRACE_RESOLVE_SUCCESS
//#define WARN_RESOLVE_FAIL
//#define TRACE_RESOLVE_CREATED

using LionFire.DependencyInjection;
using LionFire.Structures;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace LionFire
{
    /// <remarks>
    /// REVIEW - Should this functionality be folded into the DependencyContext's resolution mechanism, with the ability to set up fallbacks within an DependencyContext? 
    /// </remarks>
    public class InstanceStackRegistrar
    {
        #region Static Accessor

        //public static InstanceStackRegistrar Instance => ManualSingleton<InstanceStackRegistrar>.Instance;
        public static InstanceStackRegistrar Default => DependencyContext.Default.GetServiceOrSingleton<InstanceStackRegistrar>();

        #endregion

        #region (Private) Fields

        private readonly object listLock = new object();
        private readonly ConcurrentDictionary<Type, List<object>> interfaceToObjectStack = new ConcurrentDictionary<Type, List<object>>();

        #endregion

        public InstanceStackRegistrar()
        {
        }

        private interface IKeyType<T> : IList<T> { }

        public void Deregister<T>( T obj)
        {
            List<object> result = TryResolveList<T>();
            if (result == null) return;

            lock (listLock)
            {
                result.Remove(obj);

                if (result.Count == 0) interfaceToObjectStack.TryRemove(typeof(IKeyType<T>), out result);
            }

        }

        #region Register

        public void Register<interfaceKeyType>(object obj)
        {
            Register(typeof(interfaceKeyType), obj);
        }

        public void Register(Type interfaceKeyType, object obj)
        {
            if (!interfaceKeyType.IsAssignableFrom(obj.GetType()))
            {
                throw new ArgumentException("interfaceKeyType must be assignable from obj.GetType()");
            }

            var list = interfaceToObjectStack.GetOrAdd(interfaceKeyType, (key) => new List<object>());

            lock (listLock)
            {
                list.Add(obj);
            }
        }

        #endregion


        public T TryResolve<T>()
        {
            return (T)TryResolve(typeof(T));
            //object result = TryResolve(typeof(T));
            //T casted = (T)result;
            //return casted;
        }

        public object TryResolve(Type interfaceType)
        {
            var list = TryResolveList(interfaceType);
            object result;

            lock (listLock)
            {
                 result = list.LastOrDefault();
            }

            if (result != null)
            {
#if TRACE_RESOLVE_SUCCESS
                l.Trace("Resolved " + interfaceType.FullName + " to instance of type " + result.GetType().FullName);
#endif
            }
            else
            {
#if WARN_RESOLVE_FAIL
                l.Warn("Failed to resolve instance for key type: " + interfaceType.FullName);
#endif
            }

            return result;
        }

        public List<object> TryResolveList<T>()
        {
            return TryResolveList(typeof(T));
        }
        public List<object> TryResolveList(Type interfaceType)
        {
            var result = interfaceToObjectStack.TryGetValue(interfaceType);

            lock (listLock)
            {
                if (result != null && result.Count == 0)
                {
                    // Unexpected
                    interfaceToObjectStack.TryRemove(interfaceType, out result);
                    result = null;
                }
            }

            return result;
        }
    }
}
