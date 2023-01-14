using LionFire.Hosting;
using LionFire.MultiTyping;
using LionFire.Persistence.Filesystem;
using LionFire.Persisters.Expanders;
using LionFire.Services;
using LionFire.Testing;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

[TestClass]
public static class TestHostBuilder
{
    [AssemblyInitialize]
    public static void Init(TestContext testContext)
    {
        TestRunner.HostApplicationFactory = () =>
        {
            var hab = new HostApplicationBuilder();
            hab.LionFire(lf => lf
                    .Vos()
                )
                .Services
                    .Expansion()
                    .AddFilesystem()
                    ;
            return hab;
        };
    }
}
