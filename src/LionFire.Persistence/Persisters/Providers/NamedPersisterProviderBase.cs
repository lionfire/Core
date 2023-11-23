#nullable enable

using LionFire.Referencing;
using LionFire.Structures;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;

namespace LionFire.Persistence.Persisters
{
    public abstract class NamedPersisterProviderBase<TReference, TPersister> : PersisterProviderBase<TReference, TPersister>
          where TReference : IReference
        where TPersister : IPersister<TReference>
        //where TOptions : INamedPersisterOptions
    {
        protected ConcurrentDictionary<string, TPersister> persisters = new ConcurrentDictionary<string, TPersister>();

        public NamedPersisterProviderBase(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override TPersister GetPersister(string? name = null)
        {
            if (name != null)
            {
                return persisters.GetOrAdd(name, n => CreatePersister(new OptionsName(name)));
            }
            else
            {
                name = Microsoft.Extensions.Options.Options.DefaultName;
                return persisters.GetOrAdd(name, n => CreatePersister());
            }
        }
    }
}
