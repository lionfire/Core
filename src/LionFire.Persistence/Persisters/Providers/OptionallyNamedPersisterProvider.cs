#nullable enable

using LionFire.Referencing;
using Microsoft.Extensions.Options;
using System;

namespace LionFire.Persistence.Persisters
{
    public class OptionallyNamedPersisterProvider<TReference, TPersister> : NamedPersisterProviderBase<TReference, TPersister>
        where TReference : IReference
        where TPersister : IPersister<TReference>
    {
        public override bool HasDefaultPersister => true;

        public OptionallyNamedPersisterProvider(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
