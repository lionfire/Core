using LionFire.Persistence.Persisters;
using LionFire.Vos;
using Microsoft.Extensions.Options;
using System;

namespace LionFire.Persistence.Persisters.Vos
{
    public class VosPersisterProvider : OptionallyNamedPersisterProvider<VosReference, VosPersister, VosPersisterOptions>
    //, IPersisterProvider<ProviderFileReference>
    {
        public VosPersisterProvider(IServiceProvider serviceProvider, IOptionsMonitor<VosPersisterOptions> options) : base(serviceProvider, options)
        { }

        //IPersister<ProviderFileReference> IPersisterProvider<ProviderFileReference>.GetPersister(string name) => GetPersister(name);
    }
}
