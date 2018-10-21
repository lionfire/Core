using System;
using System.IO;
using LionFire.Persistence;

namespace LionFire.Serialization
{
    public class MatchingExtensionDeserializerScorer : IDeserializeScorer
    {
        public float ScoreForStrategy(SerializationStrategyPreference preference, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            var op = operation.Value;
            foreach (var file in op.Deserialization.CandidateFileNames)
            {
                var extension = Path.GetExtension(file).TrimFirstDot();

                if(preference.Strategy.SupportsExtension(extension))
                {
                    return context.Deserialization.DeserializerSelectionContext.Scores.SupportedFileExtension;
                }
            }
            return 0;
        }
    }

    //public static class ISerializationProviderExtensions
    //{
    //    // FUTURE?
    //    //public static ISerializationStrategy ResolveStrategy(this IHasSerializationStrategies serializationProvider, SerializerSelectionContext context) => serializationProvider.GetStrategies(context).FirstOrDefault().Strategy;
    //}

}
