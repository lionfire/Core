using System.Collections;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.DependencyInjection;

public class DynamicServiceProvider : IServiceCollection, IServiceProvider
{
    #region Parameters

    #region Fallback Parent IServiceProvider

    public IServiceProvider? Parent
    {
        get
        {
            return parent ??= ParentFunc?.Invoke();
        }
        set => parent = value;
    }
    private IServiceProvider? parent;
    public Func<IServiceProvider?>? ParentFunc { get; set; } = null;

    #endregion

    #endregion

    #region Construction

    public DynamicServiceProvider(IServiceProvider? parentServiceProvider = null)
    {
        Parent = parentServiceProvider;
    }
    public DynamicServiceProvider(Func<IServiceProvider?> parentServiceProviderFunc)
    {
        ParentFunc = parentServiceProviderFunc;
    }

    #endregion

    #region State

    private object _lock = new object();
    ConcurrentDictionary<Type, object> singletonInstances = new ConcurrentDictionary<Type, object>();

    #endregion

    Dictionary<Type, Func<IServiceProvider, object>> singletonFactories = new Dictionary<Type, Func<IServiceProvider, object>>();
    //ConcurrentDictionary<Type, Type> implementationTypes = new ConcurrentDictionary<Type, Type>();

    Dictionary<Type, Type> transientImplementationTypes = new Dictionary<Type, Type>();
    Dictionary<Type, Func<IServiceProvider, object>> transientFactories = new Dictionary<Type, Func<IServiceProvider, object>>();

    #region (public) Methods

    public object GetService(Type serviceType)
    {
        if (singletonInstances.ContainsKey(serviceType))
        {
            return singletonInstances[serviceType];
        }
        else if (singletonFactories.ContainsKey(serviceType))
        {
            var instance = singletonFactories[serviceType](this);
            singletonFactories.Remove(serviceType);
            if (singletonInstances.TryAdd(serviceType, instance))
            {
                return instance;
            }
            else
            {
                return singletonInstances[serviceType];
            }
        }
        else if (transientFactories.ContainsKey(serviceType))
        {
            // UNTESTED
            var factory = transientFactories[serviceType];
            return factory(this);
        }
        else if (transientImplementationTypes.ContainsKey(serviceType))
        {
            // UNTESTED
            return ActivatorUtilities.CreateInstance(this, transientImplementationTypes[serviceType]);
        }
        try
        {
            return Parent?.GetService(serviceType);
        }
        catch (ObjectDisposedException) { } // EMPTYCATCH
        return null;
    }

    #endregion

    #region IServiceCollection

    void ICollection<ServiceDescriptor>.Add(ServiceDescriptor item) => Add(item);
    public void Add(ServiceDescriptor item)
    {
        lock (_lock)
        {
            if (singletonFactories.ContainsKey(item.ServiceType)) throw new AlreadySetException();
            //if (implementationTypes.ContainsKey(item.ServiceType)) throw new AlreadySetException();
            if (transientImplementationTypes.ContainsKey(item.ServiceType)) throw new AlreadySetException();

            switch (item.Lifetime)
            {
                // ENH: more comprehensive detection of duplicate/overlapping registrations (throw)

                case ServiceLifetime.Singleton:
                    if (item.ImplementationInstance != null)
                    {
                        singletonInstances.AddOrUpdate(item.ServiceType, item.ImplementationInstance, (t, o) => throw new AlreadyException());
                    }
                    else if (item.ImplementationFactory != null)
                    {
                        singletonFactories.Add(item.ServiceType, item.ImplementationFactory);
                    }
                    else if (item.ImplementationType != null)
                    {
                        singletonFactories.Add(item.ServiceType, sp => ActivatorUtilities.CreateInstance(sp, item.ImplementationType));
                    }
                    break;
                //case ServiceLifetime.Scoped:
                //    break;
                case ServiceLifetime.Transient:
                    if (item.ImplementationType != null)
                    {
                        transientImplementationTypes.Add(item.ServiceType, item.ImplementationType);
                    }
                    else if (item.ImplementationFactory != null)
                    {
                        transientFactories.Add(item.ServiceType, item.ImplementationFactory);
                    }
                    else throw new ArgumentException("Transient descriptor must have factory or implementation type.");
                    break;
                default:
                    throw new NotImplementedException("TODO");
            }
        }
    }

    #endregion

    #region IServiceCollection (not implemented)

    int ICollection<ServiceDescriptor>.Count => singletonInstances.Count +
        singletonFactories.Count +
        //implementationTypes.Count +
        transientImplementationTypes.Count +
        transientFactories.Count;

    bool ICollection<ServiceDescriptor>.IsReadOnly => false;
    void ICollection<ServiceDescriptor>.Clear()
    {
        singletonInstances.Clear();
        singletonFactories.Clear();
        //implementationTypes.Clear();
        transientImplementationTypes.Clear();
        transientFactories.Clear();
    }

    ServiceDescriptor IList<ServiceDescriptor>.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    int IList<ServiceDescriptor>.IndexOf(ServiceDescriptor item) => throw new NotImplementedException();
    void IList<ServiceDescriptor>.Insert(int index, ServiceDescriptor item) => throw new NotImplementedException();
    void IList<ServiceDescriptor>.RemoveAt(int index) => throw new NotImplementedException();
    bool ICollection<ServiceDescriptor>.Contains(ServiceDescriptor item) => throw new NotImplementedException();
    void ICollection<ServiceDescriptor>.CopyTo(ServiceDescriptor[] array, int arrayIndex) => throw new NotImplementedException();
    bool ICollection<ServiceDescriptor>.Remove(ServiceDescriptor item) => throw new NotImplementedException();
    IEnumerator<ServiceDescriptor> IEnumerable<ServiceDescriptor>.GetEnumerator() => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    #endregion
}
