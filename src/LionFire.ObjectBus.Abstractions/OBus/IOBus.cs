using LionFire.Dependencies;
using LionFire.Referencing;
using LionFire.Persistence.Handles;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.ObjectBus
{

    

    public interface IOBus : IOBaseProvider, IReadWriteHandleProvider, IReadHandleProvider, ICollectionHandleProvider, IReferenceProvider, ICompatibleWithSome<IReference>
    {
        IServiceCollection AddServices(IServiceCollection sc);
    }
    
}
