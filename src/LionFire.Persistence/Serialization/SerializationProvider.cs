using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Applications.Hosting;
using LionFire.DependencyInjection;

namespace LionFire.Serialization
{
  
    /// <summary>
    /// Uses current injection context (REVIEW)
    /// </summary>
    public class SerializationProvider : ResolvingSerializationServiceBase, ISerializationProvider
    {
        public IEnumerable<ISerializationService> SerializationServices => InjectionContext.Current.GetService<IEnumerable<ISerializationService>>();
        public IEnumerable<ISerializationStrategy> SerializationStrategies =>
            SerializationServices.SelectMany(service => service.AllStrategies).Concat(InjectionContext.Current.GetService<IEnumerable<ISerializationStrategy>>());
        public override IEnumerable<SerializationStrategyPreference> SerializationStrategyPreferences =>
            InjectionContext.Current.GetService<IEnumerable<SerializationStrategyPreference>>().SelectMany(pref => pref.ResolveToStrategies())
            .Concat(SerializationStrategies.Select(strategy => new SerializationStrategyPreference(strategy)));
    }
}
