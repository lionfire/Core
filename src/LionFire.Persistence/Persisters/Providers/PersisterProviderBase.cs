#nullable enable

using LionFire.Referencing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace LionFire.Persistence.Persisters
{
    public abstract class PersisterProviderBase<TReference, TPersister, TOptions> : IPersisterProvider<TReference>
          where TReference : IReference
        where TPersister : IPersister<TReference>
    {
        protected IServiceProvider serviceProvider;
        protected IOptionsMonitor<TOptions> options;
        public PersisterProviderBase(IServiceProvider serviceProvider, IOptionsMonitor<TOptions> options)
        {
            this.options = options;
            this.serviceProvider = serviceProvider;
        }

        public Func<string, TPersister> FactoryMethod { get => factoryMethod ?? DefaultFactoryMethod; protected set => factoryMethod = value; }
        protected Func<string, TPersister>? factoryMethod;
        public abstract bool HasDefaultPersister { get; }


        public TPersister DefaultFactoryMethod(string? name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return ActivatorUtilities.CreateInstance<TPersister>(serviceProvider, name, options.CurrentValue);
                //return Activator.CreateInstance<TPersister>(options.CurrentValue);
                //return (TPersister)Activator.CreateInstance(typeof(TPersister), name, options.CurrentValue);
            }
            else
            {
                return ActivatorUtilities.CreateInstance<TPersister>(serviceProvider, name, options.Get(name));
                //return (TPersister)Activator.CreateInstance(typeof(TPersister), name, options.Get(name));
            }
        }
        IPersister<TReference> IPersisterProvider<TReference>.GetPersister(string? name) => GetPersister(name);
        public abstract TPersister GetPersister(string? name = null);
    }
}
