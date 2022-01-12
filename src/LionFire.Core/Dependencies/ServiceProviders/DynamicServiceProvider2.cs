using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Dependencies
{
    public class DynamicServiceProvider2 : IServiceProvider
    {
        public IServiceProvider ParentServiceProvider { get; }

        public DynamicServiceProvider2(IServiceProvider parentServiceProvider)
        {
            ParentServiceProvider = parentServiceProvider;
        }

        public object GetService(Type serviceType)
        {
            if (Singletons.ContainsKey(serviceType)) { return Singletons[serviceType]; }

            if (SingletonRegistrations.ContainsKey(serviceType)) {
                var newSingleton = ActivatorUtilities.CreateInstance(this, serviceType);
                Singletons.Add(serviceType, newSingleton);
                return newSingleton;
            }

            if (Transients.ContainsKey(serviceType)) { return ActivatorUtilities.CreateInstance(this, serviceType); }

            foreach(var sp in ServiceProviders)
            {
                var result = sp.GetService(serviceType);
                if(result != null)
                {
                    return result;
                }
            }
            return ParentServiceProvider.GetService(serviceType);
        }

        public Dictionary<Type, object> Singletons { get; } = new Dictionary<Type, object>();
        public Dictionary<Type, Type> SingletonRegistrations { get; } = new Dictionary<Type, Type>();
        public Dictionary<Type, Type> Transients { get; } = new Dictionary<Type, Type>();

        public List<IServiceProvider> ServiceProviders { get; } = new List<IServiceProvider>();

        public void AddSingleton<T>(T instance) => Singletons.Add(typeof(T), instance);
        public void AddSingleton<TConcrete>() => SingletonRegistrations.Add(typeof(TConcrete), typeof(TConcrete));
        //public bool RemoveSingleton<TConcrete>() => SingletonRegistrations.Remove(typeof(TConcrete));
        
        public void AddSingleton<TInterface, TConcrete>() => SingletonRegistrations.Add(typeof(TInterface), typeof(TConcrete));
        public void AddSingleton(Type serviceType, object instance) => Singletons.Add(serviceType, instance);
        public void AddTransient<TService, TImplementation>() => Singletons.Add(typeof(TService), typeof(TImplementation));
    }
}
