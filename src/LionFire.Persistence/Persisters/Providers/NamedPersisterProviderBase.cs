#nullable enable

using LionFire.Referencing;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;

namespace LionFire.Persistence.Persisters
{
    public abstract class NamedPersisterProviderBase<TReference, TPersister, TOptions> : PersisterProviderBase<TReference, TPersister, TOptions>
          where TReference : IReference
        where TPersister : IPersister<TReference>
        //where TOptions : INamedPersisterOptions
    {
        protected ConcurrentDictionary<string, TPersister> persisters = new ConcurrentDictionary<string, TPersister>();

        public NamedPersisterProviderBase(IServiceProvider serviceProvider, IOptionsMonitor<TOptions> options) : base(serviceProvider, options)
        {
        }

        public override TPersister GetPersister(string? name = null)
        {
            if (name == null) name = "";
            return persisters.GetOrAdd(name, n => FactoryMethod(n));
        }
    }
}
