using System;
using System.IO;
using LionFire.IO;
using LionFire.Persistence;
using LionFire.Structures;
using Microsoft.Extensions.Options;

namespace LionFire.Serialization
{
    
    public class MatchingExtensionDeserializeScorer : IDeserializeScorer
    {
        IOptions<SerializationOptions> Options { get; set; }
        SerializationOptions SerializationOptions => Options?.Value ?? Singleton<SerializationOptions>.Instance;

        public MatchingExtensionDeserializeScorer(IOptions<SerializationOptions> options)
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


        //public float ScoreForStrategy(SerializationStrategyPreference preference, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        //{
        //    var op = operation.Value;
        //    foreach (var file in op.Deserialization.CandidateFileNames)
        //    {
        //        var extension = Path.GetExtension(file).TrimFirstDot();

        //        if (preference.Strategy.SupportsExtension(extension))
        //        {
        //            return context.Deserialization.DeserializerSelectionContext.Scores.SupportedFileExtension;
        //        }
        //    }
        //    return 0;
        //}
        public float ScoreForStrategy(SerializationStrategyPreference preference, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            switch (SerializationOptions.DeserializeExtensionScoring)
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

    //public static class ISerializationProviderExtensions
    //{
    //    // FUTURE?
    //    //public static ISerializationStrategy ResolveStrategy(this IHasSerializationStrategies serializationProvider, SerializerSelectionContext context) => serializationProvider.GetStrategies(context).FirstOrDefault().Strategy;
    //}

}
