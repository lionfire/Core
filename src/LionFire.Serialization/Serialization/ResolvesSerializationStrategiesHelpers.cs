using System.Collections.Generic;
using System.Linq;

namespace LionFire.Serialization
{
    public static class ResolvesSerializationStrategiesHelpers
    {
        public static float ScoreForStrategy(SerializationStrategyPreference preference, SerializationOperation operation = null, SerializationContext context = null)
        {
            float score = preference.Preference;

            // TODO - more scoring

            return score;
        }

        /// <summary>
        /// Returns serializers that may be able to handle the context, sorted by priority.
        /// </summary>
        /// <param name="selectionContext"></param>
        /// <returns></returns>
        public static IEnumerable<SerializationSelectionResult> ResolveStrategies(IEnumerable<SerializationStrategyPreference> preferences, SerializationOperation operation = null, SerializationContext context = null)
        {
            var results = new SortedList<float, SerializationSelectionResult>();

            foreach (var preference in preferences)
            {
                var result = new SerializationSelectionResult(preference);

                var score = ScoreForStrategy(preference, operation, context);
                if (float.IsNaN(score))
                {
                    continue;
                }

                while (results.ContainsKey(score))
                {
                    score += 0.0001f;
                }

                result.Score = score;
                results.Add(-score, result);
            }
            return results.Values;
        }

        // OLD - see IResolvesSerializationStrategiesExtensions
        //public static ISerializationStrategy ResolveStrategy(IEnumerable<SerializationStrategyPreference> preferences, SerializerSelectionContext context) => ResolveStrategies(preferences, context).FirstOrDefault().Strategy;
    }

    //public static class ISerializationProviderExtensions
    //{
    //    // FUTURE?
    //    //public static ISerializationStrategy ResolveStrategy(this IHasSerializationStrategies serializationProvider, SerializerSelectionContext context) => serializationProvider.GetStrategies(context).FirstOrDefault().Strategy;
    //}

}
