#nullable enable
using LionFire.Persistence.Persisters;
using LionFire.Serialization;
using LionFire.Structures;
using LionFire.Vos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace LionFire.Persistence.Persisters.Vos;

public class MultiplexingPersisterProviderOptions
{
    public Dictionary<string, Type> PersisterNamesToPersisterProviderTypes { get; set; } = new();
    public Dictionary<string, Type> PersisterNamesToPersisterTypes { get; set; } = new();
}

public class VosPersisterProviderOptions : MultiplexingPersisterProviderOptions
{
}


public class VosPersisterProvider : OptionallyNamedPersisterProvider<IVobReference, VosPersister, VosPersisterOptions>
{
    public IOptionsMonitor<VosPersisterProviderOptions> VosPersisterProviderOptionsMonitor { get; }


    public VosPersisterProvider(IServiceProvider serviceProvider, IOptionsMonitor<VosPersisterProviderOptions> vosPersisterProviderOptionsMonitor) : base(serviceProvider)
    {
        VosPersisterProviderOptionsMonitor = vosPersisterProviderOptionsMonitor;
    }

    //IPersister<ProviderFileReference> IPersisterProvider<ProviderFileReference>.GetPersister(string name) => GetPersister(name);

    public override VosPersister CreatePersister(params object[] parameters)
    {
        OptionsName? optionsName = (OptionsName?)parameters?.FirstOrDefault(o => o is OptionsName);

        if (optionsName?.Name != null)
        {
            #region Try Named PersisterProvider

            var persisterProviderType = VosPersisterProviderOptionsMonitor.CurrentValue.PersisterNamesToPersisterProviderTypes.TryGetValue(optionsName.Name);
            if (persisterProviderType != null)
            {
                var provider = (IPersisterProvider<IVobReference>?)serviceProvider.GetService(persisterProviderType) ?? throw new NotSupportedException($"PersisterProvider type {persisterProviderType} is registered for PersisterName {optionsName.Name} but is not available from the ServiceProvider");
                return (VosPersister)provider.GetPersister(optionsName.Name);
            }

            #endregion

            #region Try Named Persister

            var persisterType = VosPersisterProviderOptionsMonitor.CurrentValue.PersisterNamesToPersisterProviderTypes.TryGetValue(optionsName.Name);
            if (persisterType != null)
            {
                var provider = (IPersisterProvider<IVobReference>?)serviceProvider.GetService(persisterType) ?? throw new NotSupportedException($"Persister type {persisterType} is registered for PersisterName {optionsName.Name} but is not available from the ServiceProvider");
                return (VosPersister)provider.GetPersister(optionsName.Name);
            }

            #endregion
        }
        
        return base.CreatePersister();
    }
}
