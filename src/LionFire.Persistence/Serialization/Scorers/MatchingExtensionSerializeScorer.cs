using LionFire.Persistence;
using LionFire.Structures;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace LionFire.Serialization
{
    public class MatchingExtensionSerializeScorer : ISerializeScorer
    {
        IOptions<SerializationOptions> Options { get; set; }
        SerializationOptions SerializationOptions => Options?.Value ?? Singleton<SerializationOptions>.Instance;

        public MatchingExtensionSerializeScorer(IOptions<SerializationOptions> options)
        {
            Options = options;
        }

        #region PassScore

        public float PassScore
        {
            get => passScore;
            set => passScore = value;
        }
        private float passScore = 10f;

        #endregion

        #region FailScore

        public float FailScore
        {
            get => failScore;
            set => failScore = value;
        }
        private float failScore = -10f;

        #endregion

        public float ScoreForStrategy(SerializationStrategyPreference preference, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            switch (SerializationOptions.SerializeExtensionScoring)
            {
                case FileExtensionScoring.MustMatch:
                    return preference.Strategy.SupportsExtension(operation.Value.FileExtension) ? PassScore : float.NaN;
                case FileExtensionScoring.RewardMatch:
                    return preference.Strategy.SupportsExtension(operation.Value.FileExtension) ? PassScore : FailScore;
                case FileExtensionScoring.IgnoreExtension:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(SerializationOptions.SerializeExtensionScoring));
            }
        }
    }
}