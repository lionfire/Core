using System;
using System.Collections.Generic;
using LionFire.Persistence;

namespace LionFire.Serialization
{
    public struct SerializationSelectionResult
    {
        public SerializationSelectionResult(SerializationStrategyPreference preference) {
            this.Preference = preference;
            Score = 0;
            Weights = null;
        }

        public SerializationStrategyPreference Preference { get; set; }
        public float Score { get; set; }
        public Dictionary<string, int> Weights { get; set; } 

        public ISerializationStrategy Strategy => Preference.Strategy;

    }

}
