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
using Microsoft.Extensions.Hosting;

namespace VosApp_.Windows_

{
    public class ProgramDataDirs_
    {
        public const string TestOrgName = "LionFire_UnitTest";
        public const string TestAppName = "UnitTest_AppName";
        public const string TestDataDir = "UnitTest_DataName";

        [Fact]
        public void P_AppDir()
        {
            Host.CreateDefaultBuilder()
                .LionFire(b => b.VosApp())
                .AppInfo(new AppInfo(TestAppName, TestOrgName))
                .RunAsync(serviceProvider =>
                {
                    var programDataDir = Environment.GetEnvironmentVariable("ProgramData");
                    Assert.Equal(TestOrgName, serviceProvider.GetRequiredService<AppInfo>().OrgName);
                    Assert.Equal(TestAppName, serviceProvider.GetRequiredService<AppInfo>().AppName);

                    Assert.Equal(Path.Combine(programDataDir, TestOrgName, TestAppName).ToFileReference(),
                        serviceProvider.GetRequiredService<RootManager>().Get()
                        .QueryChild("/_/stores").QueryChild("ProgramDataAppDir")
                        .AcquireOwn<VobMounts>().ReadMounts.Single().Value.Target);
                });
        }

        [Fact]
        public void P_OrgDir()
        {
            Host.CreateDefaultBuilder().LionFire(b => b.VosApp())
                .AppInfo(new AppInfo(TestAppName, TestOrgName))
                .RunAsync(serviceProvider =>
                {
                    var programDataDir = Environment.GetEnvironmentVariable("ProgramData");
                    Assert.Equal(TestOrgName, serviceProvider.GetRequiredService<AppInfo>().OrgName);

                    Assert.Equal(Path.Combine(programDataDir, TestOrgName).ToFileReference(),
                        serviceProvider.GetRequiredService<RootManager>().Get()
                        .QueryChild("/_/stores").QueryChild("ProgramDataOrgDir")
                        .AcquireOwn<VobMounts>().ReadMounts.Single().Value.Target);
                });
        }

        [Fact]
        public void P_CustomDir()
        {
            Host.CreateDefaultBuilder().LionFire(b => b.VosApp())
                .AppInfo(new AppInfo
                {
                    DataDirName = TestDataDir,
                    OrgName = TestOrgName,
                    AppName = TestAppName,
                })
                .RunAsync(serviceProvider =>
                {
                    var programDataDir = Environment.GetEnvironmentVariable("ProgramData");
                    Assert.Equal(TestOrgName, serviceProvider.GetRequiredService<AppInfo>().OrgName);
                    Assert.Equal(TestAppName, serviceProvider.GetRequiredService<AppInfo>().AppName);
                    Assert.Equal(TestDataDir, serviceProvider.GetRequiredService<AppInfo>().DataDirName);

                    Assert.Equal(Path.Combine(programDataDir, TestOrgName, TestDataDir).ToFileReference(),
                        serviceProvider.GetRequiredService<RootManager>().Get()
                        .QueryChild("/_/stores").QueryChild("ProgramDataDataDir")
                        .AcquireOwn<VobMounts>().ReadMounts.Single().Value.Target);
                });
        }
    }
}
