#nullable enable
using System;
using System.Collections.Generic;
using LionFire.Persistence;

namespace LionFire.Serialization;

public struct SerializationSelectionResult
{
    public SerializationSelectionResult(SerializationStrategyPreference preference, ScoringAttempt scoringAttempt)
    {
        this.Preference = preference;
        Score = 0;
        Weights = null;
        ScoringAttempt = scoringAttempt;
    }

    public SerializationStrategyPreference? Preference { get; set; }
    public float Score { get; set; }
    public Dictionary<string, int>? Weights { get; set; }

    public ISerializationStrategy? Strategy => Preference?.Strategy;

    public ScoringAttempt ScoringAttempt { get; set; }
}
