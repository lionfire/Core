using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Applications.Hosting;
using LionFire.Dependencies;

namespace LionFire.Serialization;


public class SerializationProvider : ResolvingSerializationServiceBase, ISerializationProvider
{
    public IEnumerable<ISerializationService> SerializationServices { get; private set; }
    //=> DependencyContext.Current.GetService<IEnumerable<ISerializationService>>();
    //public IEnumerable<ISerializationStrategy> SerializationStrategies { get; private set; }
    //=> SerializationServices.SelectMany(service => service.AllStrategies).Concat(DependencyContext.Current.GetService<IEnumerable<ISerializationStrategy>>());

    /// <summary>
    /// Used by the Strategies() method to determine the most preferred strategy for an operation
    /// </summary>
    public override IEnumerable<SerializationStrategyPreference> SerializationStrategyPreferences => serializationStrategyPreferences;
    private IEnumerable<SerializationStrategyPreference> serializationStrategyPreferences;

    public SerializationProvider(IEnumerable<ISerializationService> serializationServices, IEnumerable<ISerializationStrategy> serializationStrategies, IEnumerable<SerializationStrategyPreference> registeredSerializationStrategyPreferences)
    {
        SerializationServices = serializationServices;
        strategies = serializationStrategies;
        serializationStrategyPreferences = registeredSerializationStrategyPreferences.SelectMany(pref => pref.ResolveAllStrategyPreferences())
            .Concat(Strategies.Select(strategy => new SerializationStrategyPreference(strategy)));

    }
}
