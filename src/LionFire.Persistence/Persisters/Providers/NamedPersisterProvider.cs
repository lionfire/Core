#nullable enable

using LionFire.Referencing;
using Microsoft.Extensions.Options;
using System;

namespace LionFire.Persistence.Persisters
{
    /// <summary>
    /// Requires a named persister provider.  Attempts to get an unnamed (or empty string) one will throw a NotSupportedException
    /// </summary>
    public class NamedPersisterProvider<TReference, TPersister> : NamedPersisterProviderBase<TReference, TPersister>, IPersisterProvider<TReference>
          where TReference : IReference
        where TPersister : IPersister<TReference>
    {
        public override bool HasDefaultPersister => false;

        public NamedPersisterProvider(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override TPersister GetPersister(string? name)
        {
            if (string.IsNullOrEmpty(name)) throw new NotSupportedException($"Named persisters not available (Reference tyoe: {typeof(TReference).FullName}, persister type: {typeof(TPersister).FullName})");
            return base.GetPersister(name);
        }
    }
}
