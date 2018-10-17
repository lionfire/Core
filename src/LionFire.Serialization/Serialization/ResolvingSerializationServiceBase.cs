using System;
using System.Collections.Generic;
using LionFire.Persistence;

namespace LionFire.Serialization
{
    public abstract class ResolvingSerializationServiceBase : IResolvesSerializationStrategies
    {
        public abstract IEnumerable<SerializationStrategyPreference> SerializationStrategyPreferences { get; }

        public IEnumerable<ISerializationStrategy> AllStrategies => strategies;
        protected List<ISerializationStrategy> strategies;

        //public IEnumerable<SerializationSelectionResult> Strategies(SerializationOperation operation = null, SerializationContext context = null) => ResolvesSerializationStrategiesHelpers.ResolveStrategies(SerializationStrategyPreferences, operation, context);

        public IEnumerable<SerializationSelectionResult> Strategies(Lazy<PersistenceContext> context = null) => ResolvesSerializationStrategiesHelpers.ResolveStrategies(SerializationStrategyPreferences, context);
    }
}
