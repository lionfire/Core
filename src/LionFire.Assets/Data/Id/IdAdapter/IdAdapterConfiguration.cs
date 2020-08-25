using System.Collections.Generic;

namespace LionFire.Data.Id
{
    public class IdAdapterConfiguration
    {
        public List<IIdMappingStrategy> Strategies { get; set; } = new List<IIdMappingStrategy>();
    }
}
