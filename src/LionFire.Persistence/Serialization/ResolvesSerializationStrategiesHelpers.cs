using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LionFire.Dependencies;
using LionFire.IO;
using LionFire.Persistence;

namespace LionFire.Serialization
{
    //public class ResolvesSerializationStrategiesHelpersBase<TScorerInterface, TOp, TContext>
    //    where TScorerInterface : ISerializerScorerBase<TOp,TContext>
    //    where TOp : PersistenceOperation
    //    where TContext : PersistenceContext 
    //{

    //}

    //public class DeserializeHelpers 
    //    : ResolvesSerializationStrategiesHelpersBase<ISerializerScorer, PersistenceOperation, PersistenceContext>
    //{
    //}
    public class SerializeHelpers
    //: ResolvesSerializationStrategiesHelpersBase<ISerializerScorer, PersistenceOperation, PersistenceContext>
    {
        public static float ScoreForStrategy(SerializationStrategyPreference preference, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null, IODirection? direction = null)
        {
            if (!direction.HasValue) direction = operation?.Value.Direction;

            float sum = preference.Preference;

            var op = operation?.Value;

            Type serviceType;
            IEnumerable<ISerializerScorerBase> strategyScorers;
            switch (op == null ? (direction ?? IODirection.Unspecified) : op.Direction)
            {
                default:
                case IODirection.Unspecified:
                    throw new NotSupportedException();
                //serviceType = typeof(IEnumerable<ISerializerScorerBase>);
                //break;
                case IODirection.Write:
                    serviceType = typeof(IEnumerable<ISerializeScorer>);
                    strategyScorers = preference.Strategy.SerializeScorers;
                    break;
                case IODirection.Read:
                    serviceType = typeof(IEnumerable<IDeserializeScorer>);
                    strategyScorers = preference.Strategy.DeserializeScorers;
                    break;
            }

            foreach (var scorer in ((IEnumerable)DependencyContext.Current.GetService(serviceType)).OfType<ISerializerScorerBase>().Concat(strategyScorers))
            {
                if (float.IsNaN(sum))
                {
                    break;
                }
                sum += scorer.ScoreForStrategy(preference, operation, context);
            }

            return sum;
        }

        /// <summary>
        /// Returns serializers that may be able to handle the context, sorted by priority.
        /// </summary>
        /// <param name="selectionContext"></param>
        /// <returns></returns>
        public static IEnumerable<SerializationSelectionResult> ResolveStrategies(IEnumerable<SerializationStrategyPreference> preferences, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null, IODirection? direction = null)
        {
            if (!direction.HasValue) direction = operation?.Value.Direction;

            var results = new SortedList<float, SerializationSelectionResult>();

            foreach (var preference in preferences)
            {
                var result = new SerializationSelectionResult(preference);

                var score = ScoreForStrategy(preference, operation, context, direction: direction);
                if (float.IsNaN(score))
                {
                    continue;
                }

                while (results.ContainsKey(-score))
                {
                    score += 0.0001f;
                }

                result.Score = score;
                results.Add(-score, result);
            }
            return results.Values;
        }
    }

    public static class PathUtils // MOVE
    {
        public static string TrimFirstDot(this string str)
        {
            if (str?.StartsWith(".") == true) return str.Substring(1);
            return str;
        }
    }

    public static class ResolvesSerializationStrategiesHelpers
    {
        public static bool SupportsExtension(this ISerializationStrategy strategy, string extension) => strategy.Formats.Where(f => f.FileExtensions.Contains(extension.TrimFirstDot(), StringComparer.InvariantCultureIgnoreCase)).Any();

        //public static float ScoreForStrategy(SerializationStrategyPreference preference, Lazy<DeserializePersistenceOperation> operation = null, DeserializationPersistenceContext context = null)
        //{
        //    float sum = preference.Preference;

        //    foreach (var scorer in DependencyContext.Current.GetService<IEnumerable<IDeserializerScorer>>())
        //    {
        //        if (float.IsNaN(sum))
        //        {
        //            break;
        //        }
        //        sum += scorer.ScoreForStrategy(preference, operation, context);
        //    }
        //    return sum;
        //}


        //public static IEnumerable<SerializationSelectionResult> ResolveSerializeStrategies(IEnumerable<SerializationStrategyPreference> preferences, Lazy<SerializePersistenceOperation> operation = null, SerializationPersistenceContext context = null)
        //{
        //    var results = new SortedList<float, SerializationSelectionResult>();

        //    foreach (var preference in preferences)
        //    {
        //        var result = new SerializationSelectionResult(preference);

        //        var score = ScoreForStrategy(preference, operation, context);
        //        if (float.IsNaN(score))
        //        {
        //            continue;
        //        }

        //        while (results.ContainsKey(score))
        //        {
        //            score += 0.0001f;
        //        }

        //        result.Score = score;
        //        results.Add(-score, result);
        //    }
        //    return results.Values;
        //}

        // OLD - see IResolvesSerializationStrategiesExtensions
        //public static ISerializationStrategy ResolveStrategy(IEnumerable<SerializationStrategyPreference> preferences, SerializerSelectionContext context) => ResolveStrategies(preferences, context).FirstOrDefault().Strategy;
    }

    //public static class ISerializationProviderExtensions
    //{
    //    // FUTURE?
    //    //public static ISerializationStrategy ResolveStrategy(this IHasSerializationStrategies serializationProvider, SerializerSelectionContext context) => serializationProvider.GetStrategies(context).FirstOrDefault().Strategy;
    //}

}
