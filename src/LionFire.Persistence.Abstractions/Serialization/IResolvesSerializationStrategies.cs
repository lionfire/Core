using System;
using System.Collections.Generic;
using LionFire.Persistence;
using LionFire.Structures;

namespace LionFire.Serialization
{
    public interface IResolvesSerializationStrategies : IHasSerializationStrategies
    {
        /// <summary>
        /// Get available strategies, sorted to have best scores first
        /// </summary>
        IEnumerable<SerializationSelectionResult> Strategies(Lazy<PersistenceOperation> operation = null, PersistenceContext context = null);
    }
}
