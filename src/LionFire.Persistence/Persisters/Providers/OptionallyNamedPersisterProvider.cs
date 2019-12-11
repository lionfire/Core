#nullable enable

using LionFire.Referencing;
using Microsoft.Extensions.Options;
using System;

namespace LionFire.Persistence.Persisters
{
    public class OptionallyNamedPersisterProvider<TReference, TPersister,TOptions> : NamedPersisterProviderBase<TReference, TPersister, TOptions> 
        where TReference : IReference
        where TPersister : class, IPersister<TReference>
    {
        public override bool HasDefaultPersister => true;

        public OptionallyNamedPersisterProvider(IServiceProvider serviceProvider, IOptionsMonitor<TOptions> options) : base(serviceProvider, options)
        {
        }
    }
}
