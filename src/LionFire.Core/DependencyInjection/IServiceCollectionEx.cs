using System.Collections;

namespace LionFire.DependencyInjection; 

public interface IServiceCollectionEx : ICollection<ServiceDescriptorEx>, IEnumerable<ServiceDescriptorEx>, IEnumerable, IList<ServiceDescriptorEx>
{
}