using LionFire.DependencyInjection;

namespace LionFire.ObjectBus
{
    public interface IOBusReferenceProviderService : IResolverService<IOBus, string>
    {
    }
}
