using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using LionFire.ExtensionMethods;
using Microsoft.Extensions.Options;

namespace LionFire.DependencyInjection;

public class TransientGenericFactories : IOpenGenericFactory
{

    private ConcurrentDictionary<Type, Type>? ConcreteImplementationTypes { get; set; }
    private ConcurrentDictionary<Type, Type>? UnboundGenericImplementationTypes { get; set; }

    private ConcurrentDictionary<Type, Func<Type, object[], object>>? UnboundGenericFactories { get; set; }
    private ConcurrentDictionary<Type, Func<Type, object[], object>>? BoundFactories { get; set; }

    public IServiceProvider ServiceProvider { get; init; }

    public TransientGenericFactories(IServiceProvider serviceProvider, IOptionsMonitor<ServiceCollectionEx> servicesOptions)
    {
        ServiceProvider = serviceProvider;

        var serivces = servicesOptions.CurrentValue;
        foreach (var d in serivces)
        {
            if (d.ImplementationType != null)
            {
                Register(d.ServiceType, d.ImplementationType);
            }
            if (d.Factory != null)
            {
                Register(d.ServiceType, d.Factory);
            }
        }
    }

    //public bool TryCreate<TResult>(out TResult result, params object[] parameters)
    //    => TryCreate<TResult>(typeof(TResult), out result, parameters);

    public bool TryCreate<TResult>(out TResult result, params object[] parameters)
    {
        // TEMP
        // return type: IPolymorphicGrainListItem<IMatchmaker>
        // type: IPolymorphicGrainListItem<ICustomMatchmaker>

        Type? unboundInterfaceType = null;
        Type? unboundImplementationType = null;

        if (ConcreteImplementationTypes != null && ConcreteImplementationTypes.TryGetValue(typeof(TResult), out unboundImplementationType))
        {
            // ok
        }
        else
        {
            //|| !typeof(TResult).IsGenericType || typeof(TResult).IsGenericTypeDefinition
            if (typeof(TResult).IsGenericType && UnboundGenericImplementationTypes != null)
            {
                unboundInterfaceType = typeof(TResult).GetGenericTypeDefinition();
                UnboundGenericImplementationTypes.TryGetValue(unboundInterfaceType, out unboundImplementationType);
            }
        }

        if (unboundImplementationType == null) { result = default; return false; }
        // TEMP: unboundImplementationType = PolymorphicGrainListItem<>

        Type implementationType = unboundImplementationType.IsGenericTypeDefinition
            ? unboundImplementationType.MakeGenericType(typeof(TResult).GetGenericArguments())
            : unboundImplementationType;
        // TEMP: implementationType = PolymorphicGrainListItem<IMatchmaker>

        if (UnboundGenericFactories != null && UnboundGenericFactories.TryGetValue(unboundImplementationType, out var func))
        {
            // NOTE: type is ignored!
            result = (TResult)func(typeof(TResult), parameters);
        }
        else
        {
            // NOTE: type is ignored!
            result = (TResult)ActivatorUtilities.CreateInstance(ServiceProvider, implementationType, parameters);
        }
        return true;

    }

    public TResult Create<TResult>(params object[] parameters)
    {
        if (TryCreate(out TResult result, parameters)) { return result; }
        throw new ArgumentException($"No implementation type has been registered for {typeof(TResult).FullName}");
        //throw Exception($"Failed to create {typeof(TResult).FullName}");
    }

    private TransientGenericFactories Register(Type interfaceType, Type concreteType)
    {
        if (interfaceType.IsGenericTypeDefinition)
        {
            UnboundGenericImplementationTypes ??= new();
            UnboundGenericImplementationTypes.AddOrThrow(interfaceType, concreteType);
        }
        else
        {
            if (!concreteType.IsGenericTypeDefinition || interfaceType.GetGenericArguments().Length != concreteType.GetGenericArguments().Length)
            {
                throw new ArgumentException($"If {nameof(interfaceType)} is an unbound generic, concreteType must also be, with the same number of generic arguments");
            }
            ConcreteImplementationTypes ??= new();
            ConcreteImplementationTypes.AddOrThrow(interfaceType, concreteType);
        }

        return this;
    }

    private TransientGenericFactories Register(Type interfaceType, Func<Type, object[], object> factory)
    {
        if (interfaceType.IsGenericTypeDefinition)
        {
            UnboundGenericFactories ??= new();
            UnboundGenericFactories.AddOrThrow(interfaceType, factory);
        }
        else
        {
            BoundFactories ??= new();
            BoundFactories.AddOrThrow(interfaceType, factory);
        }

        return this;
    }
}
