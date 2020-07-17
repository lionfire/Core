using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text;
using Xunit;
using LionFire.Persistence.Filesystem;
using LionFire.Vos;
using Microsoft.Extensions.Hosting;
using LionFire.FlexObjects;
using LionFire.Vos.Mounts;
using LionFire.Persistence.Persisters;

namespace LionFire.Services
{

    public class VosTest
    {

        public string VosTestDir { get; set; }
        public string VosTestDirParent { get; set; } = Path.GetTempPath();

        public VosTest()
        {
            VosTestDir = Path.Combine(VosTestDirParent, "_UnitTest " + Guid.NewGuid());
        }
    }

    public enum ExtensionExclusivityMode
    {
        Unspecified,
        Single,
        Multi,
    }
    public class ExtensionOptions
    {
        public ExtensionExclusivityMode ExtensionExclusivityMode { get; set; }
    }

    public static class VosTestSetup
    {


        public static IServiceCollection AddTestFileMount(this IServiceCollection services, bool autoExtension)
        {
            var vosTest = new VosTest();
            services.AddSingleton(vosTest);

            var dir = vosTest.VosTestDir;
            Assert.False(Directory.Exists(dir), "Unique test dir already exists: " + dir);
            Directory.CreateDirectory(dir);

            //var decorators = Flex.Create(
            //new ExtensionOptions
            //{
            //    ExtensionExclusivityMode = ExtensionExclusivityMode.Single,
            //})
            //    );
            //decorators.Add<INameTranslator>()

            services
                .VosMount("/test".ToVobReference(), dir.ToFileReference(), new MountOptions()
                {
                    ReadPriority = MountOptions.DefaultReadPriority,
                    WritePriority = MountOptions.DefaultWritePriority,
                    //Flex = decorators,
                })
                .AddHostedService(serviceProvider => new TestDirectoryCleaner(dir, serviceProvider.GetRequiredService<IHostApplicationLifetime>()))
                ;

            return services;
        }

        public static IServiceCollection AddReadOnlyTestFileMount(this IServiceCollection services)
        {
            var vosTest = new VosTest();
            services.AddSingleton(vosTest);
            var dir = vosTest.VosTestDir;
            Assert.False(Directory.Exists(dir), "Unique test dir already exists: " + dir);
            Directory.CreateDirectory(dir);

            services
                .VosMountRead("/test".ToVobReference(), dir.ToFileReference())
                .AddSingleton(serviceProvider => new TestDirectoryCleaner(dir, serviceProvider.GetRequiredService<IHostApplicationLifetime>()))
                ;

            return services;
        }
    }
}
