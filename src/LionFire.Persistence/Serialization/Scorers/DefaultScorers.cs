using System;
using System.Collections.Generic;

namespace LionFire.Serialization
{

    public static class DefaultScorers
    {
        public static IEnumerable<Type> DefaultDeserializerScorers
        {
            get
            {
                yield return typeof(MatchingExtensionDeserializeScorer);
            }
        }
        public static IEnumerable<Type> DefaultSerializerScorers
        {
            get
            {
                //yield return typeof(MatchingExtensionDeserializerScorer);
                yield return typeof(MatchingExtensionSerializeScorer);
                yield break;
            }
        }
    }

    //public static class ISerializationProviderExtensions
    //{
    //    // FUTURE?
    //    //public static ISerializationStrategy ResolveStrategy(this IHasSerializationStrategies serializationProvider, SerializerSelectionContext context) => serializationProvider.GetStrategies(context).FirstOrDefault().Strategy;
    //}

}
