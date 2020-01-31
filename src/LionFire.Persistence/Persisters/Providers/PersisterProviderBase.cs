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
            //if (string.IsNullOrEmpty(name))
            //{
            //    return (TPersister)Activator.CreateInstance(typeof(TPersister));
            //}
            //else
            //{
                try
                {
                    return ActivatorUtilities.CreateInstance<TPersister>(serviceProvider, name);
                }
                catch (InvalidOperationException ioe)
                    when (ioe.Message.Contains("A suitable constructor") && ioe.Message.Contains("could not be located."))
                {
                    // REVIEW: Require all Persisters to always have a name parameter?
                    //throw new Exception("Persister constructor not found.  Since name of persister is not null or empty, persister must provide a constructor accepting a string parameter.", ioe);
                    throw new Exception("Persister constructor not found.  Persister must provide a constructor accepting a string parameter representing the name of the persister.", ioe);
            }
            //return (TPersister)Activator.CreateInstance(typeof(TPersister), name, options.Get(name));
            //}
        }
        IPersister<TReference> IPersisterProvider<TReference>.GetPersister(string? name) => GetPersister(name);
        public abstract TPersister GetPersister(string? name = null);
    }
}
