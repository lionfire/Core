using LionFire;
using LionFire.Applications;
using LionFire.Environment;
using LionFire.Hosting;
using LionFire.Referencing;
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

namespace VosApp_
{
    public class AppDir_
    {

        [Fact]
        public void Pass()
        {
            VosAppHost.Create(options: new VosAppOptions
            {
                AppInfo = new AppInfo(TestGlobals.TestApplicationId)
            })
                .RunAsync(serviceProvider =>
                {
                    var root = serviceProvider.GetRequiredService<RootManager>().Get();
                    Assert.NotNull(root);

                    var stores = root.QueryChild("/_/stores");
                    Assert.NotNull(stores);

                    #region AppDir
                    {
                        var AppDir = stores.QueryChild("AppDir");
                        Assert.NotNull(stores.QueryChild("AppDir"));

                        var mounts = AppDir.TryGetOwnVobNode<VobMounts>()?.Value;
                        Assert.NotNull(mounts);
                        Assert.Single(mounts.ReadMounts);

                        var assemblyPath = Assembly.GetExecutingAssembly().Location;

                        string applicationDir = null;
                        for (var dir = Path.GetDirectoryName(assemblyPath); dir != null; dir = Directory.GetParent(dir).FullName)
                        {
                            var applicationInfoPath = Path.Combine(dir, "application.json");
                            if (File.Exists(applicationInfoPath))
                            {
                                var json = File.ReadAllText(applicationInfoPath);
                                var applicationInfo = System.Text.Json.JsonSerializer.Deserialize<AppMarker>(json);
                                if (applicationInfo != null)
                                {
                                    var expected = TestGlobals.TestApplicationId;
                                    Assert.Equal(expected, applicationInfo.AppId);
                                    if (expected == applicationInfo.AppId)
                                    {
                                        applicationDir = dir;
                                        break;
                                    }
                                }
                            }
                        }
                        Assert.NotNull(applicationDir);

                        Assert.Equal(LionPath.Normalize(applicationDir), mounts.ReadMounts.First().Value.Target.Path); // Primary assertion
                    }
                    #endregion

                });
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void P_NoCustomAppId(bool useExeDirAsAppDirIfMissing)
        {
            Assert.True(LionFireEnvironment.IsUnitTest); // MOVE

             await VosAppHost.Create(options: new VosAppOptions
            {
                //AppInfo = null,
                UseExeDirAsAppDirIfMissing = useExeDirAsAppDirIfMissing
            })
                .RunAsync(serviceProvider =>
                {
                    // Assert list of stores from the Org and App names
                    var root = serviceProvider.GetRequiredService<IRootManager>().Get();
                    Assert.NotNull(root);

                    var stores = root.QueryChild("/_/stores");
                    Assert.NotNull(stores);

                    var AppDir = stores.QueryChild("AppDir");

                    if (useExeDirAsAppDirIfMissing)
                    {
                        Assert.NotNull(AppDir);

                        var mounts = AppDir.TryGetOwnVobNode<VobMounts>()?.Value;
                        Assert.NotNull(mounts);
                        Assert.Single(mounts.ReadMounts);

                        var assemblyPath = Assembly.GetEntryAssembly().Location;

                        string applicationDir = Path.GetDirectoryName(assemblyPath);
                        Assert.NotNull(applicationDir);

                        Assert.Equal(LionPath.Normalize(applicationDir), mounts.ReadMounts.First().Value.Target.Path); // Primary assertion 
                    }
                    else
                    {
                        Assert.Null(AppDir);
                    }
                });
            Assert.True(true);
        }
    }
}
