using System.Collections.Generic;
using System.Linq;
using LionFire.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Serialization
{
    //[RegisterInterface(typeof(ISerializationService))]
    public abstract class SerializationService : ISerializationService
    {

        public abstract IEnumerable<ISerializationStrategy> Strategies { get; }

        //#region Serializers

        ////public IEnumerable<ISerializationStrategy> AllStrategies { get; private set; }
        ////{
        ////    get
        ////    {
        ////        if (serializers == null)
        ////        {
        ////            // Get strategies from DependencyContext, but only the first time.  This may be useful when initializing a Program
        ////            // with some strategies and then attempting to use the SerializationService.
        ////            serializers = DependencyContext.Current.ServiceProvider.GetServices<ISerializationStrategy>().Distinct().ToList();
        ////        }
        ////        return serializers;
        ////    }
        ////}
        ////private List<ISerializationStrategy> serializers;

        //#endregion

        //public SerializationService(IEnumerable<ISerializationStrategy> AllStrategies)
        //{
        //    serializers = AllStrategies;
        //}
    }
}
