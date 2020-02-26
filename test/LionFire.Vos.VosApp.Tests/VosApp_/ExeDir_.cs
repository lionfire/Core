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
    public class ExeDir_
    {

        [Fact]
        public async void Pass()
        {
            await VosAppHost.Create(options: new VosAppOptions
            {
            })
                .RunAsync(serviceProvider =>
                {
                    // Assert list of stores from the Org and App names
                    var root = serviceProvider.GetRequiredService<IRootManager>().Get();
                    Assert.NotNull(root);

                    var stores = root.QueryChild("/_/stores");

                    Debug.WriteLine(((Vob)root).DumpTree());
                    Assert.NotNull(stores);

                    #region ExeDir
                    {
                        var ExeDir = stores.QueryChild("ExeDir");
                        Assert.NotNull(stores.QueryChild("ExeDir"));
                        var exeMounts = ExeDir.TryGetOwnVobNode<VobMounts>()?.Value;
                        Assert.NotNull(exeMounts);
                        Assert.Single(exeMounts.ReadMounts);

                        var assemblyPath = Assembly.GetEntryAssembly().Location;
                        var assemblyDir = Path.GetDirectoryName(assemblyPath);
                        var assemblyDirLionPath = LionPath.Normalize(Path.GetDirectoryName(assemblyPath));

                        Assert.Equal(assemblyDirLionPath, exeMounts.ReadMounts.First().Value.Target.Path);
                    }
                    #endregion

                    // TODO: Clean up
                });
        }
    }
}
