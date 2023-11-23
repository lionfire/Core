#nullable enable
using System;
using System.Collections.Generic;
using LionFire.Dependencies;

namespace LionFire.DependencyInjection
{
    //public class ServiceProviderWrapper<T> : IServiceProvider
    //{
    //    public T Singleton { get; }
    //    public IServiceProvider UnderlyingServiceProvider { get; }
    //    public ServiceProviderWrapper(IServiceProvider underlyingServiceProvider, T singletonInstance)
    //    {
    //        Singleton = singletonInstance;
    //        UnderlyingServiceProvider = underlyingServiceProvider;
    //    }

    //    public object GetService(Type serviceType) => serviceType == typeof(T) ? Singleton : UnderlyingServiceProvider.GetService(serviceType);
    //    public object GetRequiredService(Type serviceType) => serviceType == typeof(T) ? (Singleton ?? throw new HasUnresolvedDependenciesException(serviceType)) : (UnderlyingServiceProvider?.GetService(serviceType) ?? throw new HasUnresolvedDependenciesException(serviceType));
    //}

    public class ServiceProviderWrapper : IServiceProvider
    {
        public IDictionary<Type, object> Singletons { get; }

        public IServiceProvider UnderlyingServiceProvider { get; }
        public ServiceProviderWrapper(IServiceProvider underlyingServiceProvider, IDictionary<Type, object> singletons)
        {
            ArgumentNullException.ThrowIfNull(underlyingServiceProvider);
            UnderlyingServiceProvider = underlyingServiceProvider;
            Singletons = singletons;
        }

        public object? GetService(Type serviceType) => Singletons.ContainsKey(serviceType) ? Singletons[serviceType] : UnderlyingServiceProvider.GetService(serviceType);
        public object GetRequiredService(Type serviceType) => Singletons.ContainsKey(serviceType) ? (Singletons[serviceType] ?? throw new HasUnresolvedDependenciesException(serviceType))
            : (UnderlyingServiceProvider?.GetService(serviceType) ?? throw new HasUnresolvedDependenciesException(serviceType));
    }
}
