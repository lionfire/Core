using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using LionFire.Persistence.Filesystem;
using LionFire.Vos;
using Microsoft.Extensions.Hosting;

namespace LionFire.Services
{
    public static class VosTestSetup
    {

        public static Func<string> VosTestDir { get; set; } = () => Path.Combine(VosTestDirParent(), "_UnitTest " + Guid.NewGuid());
        public static Func<string> VosTestDirParent { get; set; } = () => Path.GetTempPath();

        public static IServiceCollection AddTestFileMount(this IServiceCollection services)
        {
            var dir = VosTestDir();
            Assert.False(Directory.Exists(dir), "Unique test dir already exists: " + dir);
            Directory.CreateDirectory(dir);

            services
                .VosMountReadWrite("/test".ToVosReference(), dir.ToFileReference())
                .AddHostedService(serviceProvider => new TestDirectoryCleaner(dir, serviceProvider.GetRequiredService<IHostApplicationLifetime>()))
                ;

            return services;
        }

        public static IServiceCollection AddReadOnlyTestFileMount(this IServiceCollection services)
        {
            var dir = VosTestDir();
            Assert.False(Directory.Exists(dir), "Unique test dir already exists: " + dir);
            Directory.CreateDirectory(dir);

            services
                .VosMountRead("/test".ToVosReference(), dir.ToFileReference())
                .AddSingleton(serviceProvider => new TestDirectoryCleaner(dir, serviceProvider.GetRequiredService<IHostApplicationLifetime>()))
                ;

            return services;
        }
    }
}
