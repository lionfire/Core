using System.Collections.Generic;

namespace LionFire.Serialization
{

    public interface ISerializationProvider : IResolvesSerializationStrategies
    {
        IEnumerable<ISerializationService> SerializationServices { get; }
    }

    //public static class ISerializationProviderExtensions
    //{
    //    // FUTURE?
    //    //public static ISerializationStrategy ResolveStrategy(this IHasSerializationStrategies serializationProvider, SerializerSelectionContext context) => serializationProvider.GetStrategies(context).FirstOrDefault().Strategy;
    //}

}
