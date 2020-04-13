using LionFire;

using LionFire.Persistence.Filesystem;
using LionFire.Applications;
using LionFire.Environment;
using LionFire.Hosting;
using LionFire.Referencing;
using LionFire.Services;
using LionFire.Vos;
using LionFire.Vos.Mounts;
using LionFire.Vos.VosApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Xunit;

namespace VosApp_.Windows_
{
    // TODO: Same tests for LocalLow, Roaming ?

    public class LocalAppDataDirs_
    {
        public const string TestOrgName = "LionFire_UnitTest";
        public const string TestAppName = "UnitTest_AppName";
        public const string TestDataDir = "UnitTest_DataName";

        [Fact]
        public void P_AppDir()
        {
            VosAppHostBuilder.Create(TestAppName, TestOrgName)
                .RunAsync(serviceProvider =>
                {
                    var programDataDir = Environment.GetEnvironmentVariable("LocalAppData");
                    Assert.Equal(TestOrgName, serviceProvider.GetRequiredService<AppInfo>().OrgName);
                    Assert.Equal(TestAppName, serviceProvider.GetRequiredService<AppInfo>().AppName);

                    Assert.Equal(Path.Combine(programDataDir, TestOrgName, TestAppName).ToFileReference(),
                        serviceProvider.GetRequiredService<RootManager>().Get()
                        .QueryChild("/_/stores").QueryChild("LocalAppDataAppDir")
                        .AcquireOwn<VobMounts>().ReadMounts.Single().Value.Target);
                });
        }

        [Fact]
        public void P_OrgDir()
        {
            VosAppHostBuilder.Create(TestAppName, TestOrgName)
                .RunAsync(serviceProvider =>
                {
                    var programDataDir = Environment.GetEnvironmentVariable("LocalAppData");
                    Assert.Equal(TestOrgName, serviceProvider.GetRequiredService<AppInfo>().OrgName);

                    Assert.Equal(Path.Combine(programDataDir, TestOrgName).ToFileReference(),
                        serviceProvider.GetRequiredService<RootManager>().Get()
                        .QueryChild("/_/stores").QueryChild("LocalAppDataOrgDir")
                        .AcquireOwn<VobMounts>().ReadMounts.Single().Value.Target);
                });
        }

        [Fact]
        public void P_CustomDir()
        {
            VosAppHostBuilder.Create(options: new VosAppOptions
            {
                AppInfo = new AppInfo
                {
                    DataDirName = TestDataDir,
                    OrgName = TestOrgName,
                    AppName = TestAppName,
                }
            })
                .RunAsync(serviceProvider =>
                {
                    var programDataDir = Environment.GetEnvironmentVariable("LocalAppData");
                    Assert.Equal(TestOrgName, serviceProvider.GetRequiredService<AppInfo>().OrgName);
                    Assert.Equal(TestAppName, serviceProvider.GetRequiredService<AppInfo>().AppName);
                    Assert.Equal(TestDataDir, serviceProvider.GetRequiredService<AppInfo>().DataDirName);

                    Assert.Equal(Path.Combine(programDataDir, TestOrgName, TestDataDir).ToFileReference(),
                        serviceProvider.GetRequiredService<RootManager>().Get()
                        .QueryChild("/_/stores").QueryChild("LocalAppDataDataDir")
                        .AcquireOwn<VobMounts>().ReadMounts.Single().Value.Target);
                });
        }
    }
}
