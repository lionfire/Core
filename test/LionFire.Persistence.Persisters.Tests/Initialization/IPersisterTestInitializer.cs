//using LionFire.ObjectBus.Filesystem.Persisters;
using LionFire.Persistence;
using LionFire.Referencing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace LionFire.Persistence
{
    public interface IPersisterTestInitializer
    {
        string Scheme { get; }
        Type PersisterType { get; }
         IServiceCollection AddServicesForTest(IServiceCollection services);

        IHostBuilder ConfigureHostForTest(IHostBuilder hostBuilder) => hostBuilder.ConfigureServices(s => AddServicesForTest(s));
        IHostBuilder CreateTestHostBuilder() => PersistersHost.Create().ConfigureServices(s => AddServicesForTest(s));

        IReference GetReferenceForTestPath(string testPath);
        string GetPathForTestPath(string testPath);
    }
}