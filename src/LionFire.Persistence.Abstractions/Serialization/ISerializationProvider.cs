using System.Collections.Generic;

namespace LionFire.Serialization
{

    /// <summary>
    /// Provides access to all ISerializationServices.  
    /// TODO: Swap the names of ISerializationProvider and ISerializationService
    /// </summary>
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
