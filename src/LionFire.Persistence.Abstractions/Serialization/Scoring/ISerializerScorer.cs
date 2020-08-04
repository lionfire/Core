using System;
using LionFire.Persistence;

namespace LionFire.Serialization
{
    public class ScoringAttempt
    {
        public string Extension { get; set; }
    }

    //public interface ISerializerScorerBase<TOp, TContext> OLD
    //    where TContext : class
    //{
    //    float ScoreForStrategy(SerializationStrategyPreference preference, Lazy<TOp> operation = null, TContext context = null);
    //}
    public interface ISerializerScorerBase //: ISerializerScorerBase<SerializePersistenceOperation, SerializePersistenceContext>
    {
        float ScoreForStrategy(SerializationStrategyPreference preference, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null, ScoringAttempt scoringAttempt = null);
    }
    public interface ISerializeScorer : ISerializerScorerBase { }
    public interface IDeserializeScorer : ISerializerScorerBase { }


    //public static class ISerializationProviderExtensions
    //{
    //    // FUTURE?
    //    //public static ISerializationStrategy ResolveStrategy(this IHasSerializationStrategies serializationProvider, SerializerSelectionContext context) => serializationProvider.GetStrategies(context).FirstOrDefault().Strategy;
    //}

}
