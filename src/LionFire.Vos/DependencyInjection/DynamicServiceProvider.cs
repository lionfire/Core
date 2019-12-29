using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.DependencyInjection
{
    public class DynamicServiceProvider : IServiceCollection, IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            if (singletonInstances.ContainsKey(serviceType))
            {
                return singletonInstances[serviceType];
            }
            else if (singletonInstanceFactories.ContainsKey(serviceType))
            {
                var instance = singletonInstanceFactories[serviceType](this);
                singletonInstanceFactories.Remove(serviceType);
                if (singletonInstances.TryAdd(serviceType, instance))
                {
                    return serviceType;
                }
                else
                {
                    return singletonInstances[serviceType];
                }
            }
            return null;
        }

        ConcurrentDictionary<Type, object> singletonInstances = new ConcurrentDictionary<Type, object>();
        Dictionary<Type, Func<IServiceProvider, object>> singletonInstanceFactories = new Dictionary<Type, Func<IServiceProvider, object>>();

        #region IServiceCollection

        public void Add(ServiceDescriptor item)
        {

            switch (item.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    if (item.ImplementationInstance != null)
                    {
                        singletonInstances.AddOrUpdate(item.ServiceType, item.ImplementationInstance, (t, o) => throw new AlreadyException());
                    }
                    else if (item.ImplementationType != null)
                    {
                        singletonInstanceFactories.Add(item.ServiceType, sp => ActivatorUtilities.CreateInstance(sp, item.ImplementationType));
                    }
                    else if (item.ImplementationFactory != null)
                    {
                        singletonInstanceFactories.Add(item.ServiceType, item.ImplementationFactory);
                    }
                    break;
                //case ServiceLifetime.Scoped:
                //    break;
                //case ServiceLifetime.Transient:
                //    break;
                default:
                    throw new NotImplementedException("TODO");
            }
        }
        
        #endregion

        #region IServiceCollection (not implemented)

        int ICollection<ServiceDescriptor>.Count => throw new NotImplementedException();

        bool ICollection<ServiceDescriptor>.IsReadOnly => throw new NotImplementedException();

        ServiceDescriptor IList<ServiceDescriptor>.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        int IList<ServiceDescriptor>.IndexOf(ServiceDescriptor item) => throw new NotImplementedException();
        void IList<ServiceDescriptor>.Insert(int index, ServiceDescriptor item) => throw new NotImplementedException();
        void IList<ServiceDescriptor>.RemoveAt(int index) => throw new NotImplementedException();
        void ICollection<ServiceDescriptor>.Add(ServiceDescriptor item) => throw new NotImplementedException();
        void ICollection<ServiceDescriptor>.Clear() => throw new NotImplementedException();
        bool ICollection<ServiceDescriptor>.Contains(ServiceDescriptor item) => throw new NotImplementedException();
        void ICollection<ServiceDescriptor>.CopyTo(ServiceDescriptor[] array, int arrayIndex) => throw new NotImplementedException();
        bool ICollection<ServiceDescriptor>.Remove(ServiceDescriptor item) => throw new NotImplementedException();
        IEnumerator<ServiceDescriptor> IEnumerable<ServiceDescriptor>.GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

        #endregion
    }
}
