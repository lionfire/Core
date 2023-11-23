#nullable enable

using LionFire.Referencing;
using LionFire.Structures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace LionFire.Persistence.Persisters;

public abstract class PersisterProviderBase<TReference, TPersister> : IPersisterProvider<TReference>
      where TReference : IReference
    where TPersister : IPersister<TReference>
{
    protected IServiceProvider serviceProvider;
    //protected IOptionsMonitor<TOptions> options;
    public PersisterProviderBase(IServiceProvider serviceProvider)
    {
        //this.options = options;
        this.serviceProvider = serviceProvider;
    }

    public virtual TPersister CreatePersister(params object[] parameters)
    {
        #region Type

        //persisterType ??= DefaultType;

        //if (options is IMultiTypePersisterProviderOptions mt && mt.PersisterType != null)
        //{
        //    type = mt.PersisterType;
        //}

        //ArgumentNullException.ThrowIfNull(nameof(type));

        #endregion

        //if(options is INamedPersisterOptions n)
        //{
        //    n.PersisterName ??= persisterName;
        //}

        //if (string.IsNullOrEmpty(name))
        //{
        //    return (TPersister)Activator.CreateInstance(typeof(TPersister));
        //}
        //else
        //{
        //try
        //{
            return parameters == null || parameters.Length == 0
                ? ActivatorUtilities.CreateInstance<TPersister>(serviceProvider)
                : ActivatorUtilities.CreateInstance<TPersister>(serviceProvider, parameters);
        //}
        //catch (InvalidOperationException ioe)
        //    when (ioe.Message.Contains("A suitable constructor") && ioe.Message.Contains("could not be located."))
        //{
        //    // REVIEW: Require all Persisters to always have a name parameter?
        //    //throw new Exception("Persister constructor not found.  Since name of persister is not null or empty, persister must provide a constructor accepting a string parameter.", ioe);
        //    throw new Exception("Persister constructor not found.  Persister must provide a constructor accepting a string parameter representing the persisterName of the persister.", ioe);
        //}
        //return (TPersister)Activator.CreateInstance(typeof(TPersister), name, options.Get(name));
        //}
    }

    //public Func<object[], TPersister> FactoryMethod { get => factoryMethod ?? DefaultFactoryMethod; protected set => factoryMethod = value; }
    //protected Func<object[], TPersister>? factoryMethod;
    //public TPersister DefaultFactoryMethod(object[]? parameters = null)
    //{       
    //}

    public abstract bool HasDefaultPersister { get; }

    public virtual Type? DefaultType => null;

    IPersister<TReference> IPersisterProvider<TReference>.GetPersister(string? name) => GetPersister(name);
    public abstract TPersister GetPersister(string? name = null);
}
