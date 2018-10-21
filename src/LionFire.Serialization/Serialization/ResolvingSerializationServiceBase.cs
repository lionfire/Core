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

        public IEnumerable<SerializationSelectionResult> Strategies(Lazy<PersistenceOperation> operation = null, PersistenceContext context = null) => SerializeHelpers.ResolveStrategies(SerializationStrategyPreferences, operation, context);
    }
}
