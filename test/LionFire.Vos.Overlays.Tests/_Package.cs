using LionFire.Persistence.Filesystem.Tests;
using LionFire.FlexObjects;
using LionFire.Persistence.Filesystem;
using LionFire.Services;
using System;
using System.IO;
using Xunit;
using LionFire.Vos.Mounts;
using LionFire.Hosting;
using LionFire.Referencing;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Persistence;
using System.Linq;
using LionFire.MultiTyping;
using LionFire.Vos;
using LionFire.Vos.Packages;
using LionFire.Vos.Packages;
using LionFire.Data;

namespace Packages_
{
    public class _Package
    {
        [Fact]
        public async void Pass()
        {
            var pluginsDir = Path.Combine(FsTestSetup.DataDir, "_UnitTest " + Guid.NewGuid().ToString(), "TestPluginsDiskDir");

            //await VosAppHost.Create() // FUTURE: Also test this with VosApp?  Pass in IHostBuilder to a method?
            await VosHost.Create()
                    .ConfigureServices((context, services) =>
                    {
                        services
                        .AddFilesystem()

                        .VosPackageProvider("/`/TestPlugins".ToVobReference())
                        .VosMount("/`/TestPlugins/available", pluginsDir.ToFileReference(), new VobMountOptions
                        {
                            Name = "UnitTestPluginsDir",
                        })

                        //.VosMount("/`/TestPlugins/available", pluginsDir.ToFileReference(),
                        //new VobMountOptions(
                        //    decorators: new MultiTyped(new MultiTypableVisitor<IVobNodeProvider>(() => new VobMountOptions { Name = "UnitTestPlugin", ReadPriority = 100, WritePriority = -100 }))
                        //    {
                        //        Name = "UnitTestPluginsDir"
                        //    })

                        .VosMount("/`/PluginData", "/`/TestPlugins/data".ToVobReference()) // Custom data dir for app
                        //.InitializeVob("/`/TestPlugins/available/" + Path.GetFileName(pluginsDir), v =>
                        //{
                        //    v.TryAddOwn(M)
                        //})

                        // MOVE - FUTURE: Vos metadata
                        //.VosMount("/_/vos", new VobReference("/") { Persister = "vos" }, new VobMountOptions
                        //{
                        //    IsReadOnly = true,
                        //    IsExclusive = true,
                        //})
                        ;
                    })
                    .RunAsync(async () =>
                    {
                        var path1 = Path.Combine(pluginsDir, "plugin1", "file1" + ".txt");
                        var testContents1 = "B9E72769-E1DA-4648-B766-FAE37D2317E5";
                        var path2 = Path.Combine(pluginsDir, "plugin2", "file2" + ".txt");
                        var testContents2 = "5A18BB7B-85A3-4A30-93D4-89A90510EBCC";

                        #region Create Test File

                        CreateTestFile(path1, testContents1);
                        CreateTestFile(path2, testContents2);
                        void CreateTestFile(string path, string contents)
                        {
                            Assert.False(File.Exists(path));
                            Directory.CreateDirectory(Directory.GetParent(path).FullName);
                            File.WriteAllText(path, contents);
                            Assert.True(File.Exists(path));
                        }
                        #endregion

                        var pluginDataReference1 = new VobReference("/`/PluginData/file1.txt");
                        var pluginDataReference2 = new VobReference("/`/PluginData/file2.txt");
                        var dataReference1 = new VobReference("/`/TestPlugins/data/file1.txt");
                        var dataReference2 = new VobReference("/`/TestPlugins/data/file2.txt");

                        Assert.False(await pluginDataReference1.GetReadHandle<string>().Exists());
                        Assert.False(await pluginDataReference2.GetReadHandle<string>().Exists());
                        Assert.False(await dataReference1.GetReadHandle<string>().Exists());
                        Assert.False(await dataReference2.GetReadHandle<string>().Exists());

                        // How to get a Vob?  VobReference.ToVob() might be nice.  How about VobReference.ToVob().AsType<PackageManager>()
                        var packageProvider = "/`/TestPlugins".GetVob().Get<PackageProvider>();

                        //Assert.True(pluginManager.AvailablePackages.Contains("plugin1")); // TODO
                        //Assert.True(pluginManager.AvailablePackages.Contains("plugin2"));
                        Assert.False(packageProvider.EnabledPackages.Any());

                        #region Plugin1
                        {
                            packageProvider.Enable("plugin1");
                            Assert.Contains(packageProvider.EnabledPackages, s => s == "plugin1");
                            Assert.Single(packageProvider.EnabledPackages);

                            var readHandle = dataReference1.GetReadHandle<string>();
                            var persistenceResult = await readHandle.Get();

                            //Assert.True(persistenceResult.Flags.HasFlag(TransferResultFlags.Success)); // TODO - switch to Retrieve?
                            Assert.True(persistenceResult.IsSuccess);
                            Assert.Equal(testContents1, readHandle.Value);
                        }
                        {
                            var readHandle = pluginDataReference1.GetReadHandle<string>();
                            var persistenceResult = await readHandle.Get();
                            Assert.True(persistenceResult.IsSuccess);
                            Assert.Equal(testContents1, readHandle.Value);
                        }
                        #endregion

                        #region Plugin2
                        {
                            packageProvider.Enable("plugin2");
                            Assert.Contains(packageProvider.EnabledPackages, s => s == "plugin1");
                            Assert.Contains(packageProvider.EnabledPackages, s => s == "plugin2");
                            Assert.Equal(2, packageProvider.EnabledPackages.Count());

                            var readHandle = dataReference2.GetReadHandle<string>();
                            var persistenceResult = await readHandle.Get();

                            Assert.True(persistenceResult.IsSuccess);
                            Assert.Equal(testContents2, readHandle.Value);

                        }
                        {
                            var readHandle = pluginDataReference2.GetReadHandle<string>();
                            var persistenceResult = await readHandle.Get();
                            Assert.True(persistenceResult.IsSuccess);
                            Assert.Equal(testContents2, readHandle.Value);
                        }
                        #endregion

                        Assert.True(packageProvider.Disable("plugin1"));
                        Assert.True(packageProvider.Disable("plugin2"));
                        Assert.Empty(packageProvider.EnabledPackages);

                        var vobMounts = new VobReference("/`/TestPlugins/data").GetVob().AcquireOwn<VobMounts>();
                        Assert.False(vobMounts.HasLocalReadMounts);
                        Assert.False(vobMounts.HasLocalWriteMounts);

                        #region Files no longer available

                        {
                            var readHandle = dataReference1.GetReadHandle<string>();
                            var persistenceResult = await readHandle.Get();
                            Assert.True(((ITransferResult)persistenceResult).Flags.HasFlag(TransferResultFlags.MountNotAvailable));
                            Assert.Null(persistenceResult.IsSuccess);
                        }
                        {
                            var readHandle = dataReference2.GetReadHandle<string>();
                            var persistenceResult = await readHandle.Get();
                            Assert.True(((ITransferResult)persistenceResult).Flags.HasFlag(TransferResultFlags.MountNotAvailable));
                            Assert.Null(persistenceResult.IsSuccess);
                        }

                        #endregion

                        void DeleteFile(string path, bool deleteParentDirectory = false)
                        {
                            File.Delete(path);
                            Assert.False(File.Exists(path));
                            if (deleteParentDirectory)
                            {
                                try
                                {
                                    Directory.Delete(Directory.GetParent(path).FullName);
                                }
                                catch { } // EMPTYCATCH
                                Assert.False(Directory.Exists(Directory.GetParent(path).FullName));
                            }
                        }
                        DeleteFile(path1, true);
                        DeleteFile(path2, true);
                    });
        }
    }
}
