#nullable enable

using LionFire.Referencing;
using Microsoft.Extensions.Options;
using System;

namespace LionFire.Persistence.Persisters
{
    /// <summary>
    /// Requires a named persister provider.  Attempts to get an unnamed (or empty string) one will throw a NotSupportedException
    /// </summary>
    public class NamedPersisterProvider<TReference, TPersister, TOptions> : NamedPersisterProviderBase<TReference, TPersister, TOptions>, IPersisterProvider<TReference>
          where TReference : IReference
        where TPersister : IPersister<TReference>
        //where TOptions : INamedPersisterOptions
    {
        public override bool HasDefaultPersister => false;

        public NamedPersisterProvider(IServiceProvider serviceProvider, IOptionsMonitor<TOptions> options) : base(serviceProvider, options)
        {
        }

        public override TPersister GetPersister(string? name)
        {
            if (string.IsNullOrEmpty(name)) throw new NotSupportedException($"Named persisters not available (Reference tyoe: {typeof(TReference).FullName}, persister type: {typeof(TPersister).FullName})");
            return base.GetPersister(name);
        }
    }
}
