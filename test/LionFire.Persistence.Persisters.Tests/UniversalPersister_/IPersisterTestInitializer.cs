//using LionFire.ObjectBus.Filesystem.Persisters;
using LionFire.Referencing;
using Microsoft.Extensions.DependencyInjection;

namespace UniversalPersister_
{
    public interface IPersisterTestInitializer
    {
        string Scheme { get; }

         IServiceCollection AddServicesForTest(IServiceCollection services);

        IReference GetReferenceForTestPath(string testPath);
        string GetPathForTestPath(string testPath);
    }
}