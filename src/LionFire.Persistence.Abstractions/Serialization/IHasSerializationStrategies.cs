using System.Collections.Generic;

namespace LionFire.Serialization
{
    public interface IHasSerializationStrategies
    {
        IEnumerable<ISerializationStrategy> AllStrategies { get; }
    }

}
