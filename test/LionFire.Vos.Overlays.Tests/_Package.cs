using LionFire.Persistence.Filesystem.Tests;
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
using LionFire.Vos.Overlays;

namespace Packages_
{

    public class _Package
    {
        [Fact]
        public async void Pass()
        {
            var pluginsDir = Path.Combine(FsTestUtils.DataDir, "UnitTest " + Guid.NewGuid().ToString(), "TestPluginsDiskDir");

            //await VosAppHost.Create() // FUTURE: Also test this with VosApp?  Pass in IHostBuilder to a method?
            await VosHost.Create()
                    .ConfigureServices((context, services) =>
                    {
                        services
                        .AddFilesystem()

                        .VosOverlayStack("/`/TestPlugins")
                        .VosMount("/`/TestPlugins/available", pluginsDir.ToFileReference(), new MountOptions
                        {
                            Name = "UnitTestPluginsDir",
                        })

                        //.VosMount("/`/TestPlugins/available", pluginsDir.ToFileReference(),
                        //new MountOptions(
                        //    decorators: new MultiType(new MultiTypableVisitor<IVobNodeProvider>(() => new MountOptions { Name = "UnitTestPlugin", ReadPriority = 100, WritePriority = -100 }))
                        //    {
                        //        Name = "UnitTestPluginsDir"
                        //    })

                        .VosMount("/`/PluginData", "/`/TestPlugins/data".ToVosReference()) // Custom data dir for app
                        //.InitializeVob("/`/TestPlugins/available/" + Path.GetFileName(pluginsDir), v =>
                        //{
                        //    v.TryAddOwn(M)
                        //})

                        // MOVE - FUTURE: Vos metadata
                        //.VosMount("/_/vos", new VosReference("/") { Persister = "vos" }, new MountOptions
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

                        var pluginDataReference1 = new VosReference("/`/PluginData/file1.txt");
                        var pluginDataReference2 = new VosReference("/`/PluginData/file2.txt");
                        var dataReference1 = new VosReference("/`/TestPlugins/data/file1.txt");
                        var dataReference2 = new VosReference("/`/TestPlugins/data/file2.txt");

                        Assert.False(await pluginDataReference1.GetReadHandle<string>().Exists());
                        Assert.False(await pluginDataReference2.GetReadHandle<string>().Exists());
                        Assert.False(await dataReference1.GetReadHandle<string>().Exists());
                        Assert.False(await dataReference2.GetReadHandle<string>().Exists());

                        // How to get a Vob?  VosReference.ToVob() might be nice.  How about VosReference.ToVob().AsType<PackageManager>()
                        var pluginManager = "/`/TestPlugins".ToVob().GetMultiTyped().AsType<OverlayStack>();

                        //Assert.True(pluginManager.AvailablePackages.Contains("plugin1")); // TODO
                        //Assert.True(pluginManager.AvailablePackages.Contains("plugin2"));
                        Assert.False(pluginManager.EnabledPackages.Any());

                        #region Plugin1
                        {
                            pluginManager.Enable("plugin1");
                            Assert.Contains(pluginManager.EnabledPackages, s => s == "plugin1");
                            Assert.Single(pluginManager.EnabledPackages);

                            var readHandle = dataReference1.GetReadHandle<string>();
                            var persistenceResult = await readHandle.Resolve();

                            //Assert.True(persistenceResult.Flags.HasFlag(PersistenceResultFlags.Success)); // TODO - switch to Retrieve?
                            Assert.True(persistenceResult.IsSuccess);
                            Assert.Equal(testContents1, readHandle.Value);
                        }
                        {
                            var readHandle = pluginDataReference1.GetReadHandle<string>();
                            var persistenceResult = await readHandle.Resolve();
                            Assert.True(persistenceResult.IsSuccess);
                            Assert.Equal(testContents1, readHandle.Value);
                        }
                        #endregion

                        #region Plugin2
                        {
                            pluginManager.Enable("plugin2");
                            Assert.Contains(pluginManager.EnabledPackages, s => s == "plugin1");
                            Assert.Contains(pluginManager.EnabledPackages, s => s == "plugin2");
                            Assert.Equal(2, pluginManager.EnabledPackages.Count());

                            var readHandle = dataReference2.GetReadHandle<string>();
                            var persistenceResult = await readHandle.Resolve();

                            Assert.True(persistenceResult.IsSuccess);
                            Assert.Equal(testContents2, readHandle.Value);

                        }
                        {
                            var readHandle = pluginDataReference2.GetReadHandle<string>();
                            var persistenceResult = await readHandle.Resolve();
                            Assert.True(persistenceResult.IsSuccess);
                            Assert.Equal(testContents2, readHandle.Value);
                        }
                        #endregion

                        Assert.True(pluginManager.Disable("plugin1"));
                        Assert.True(pluginManager.Disable("plugin2"));
                        Assert.Empty(pluginManager.EnabledPackages);

                        var vobMounts = new VosReference("/`/TestPlugins/data").ToVob().GetOwn<VobMounts>();
                        Assert.False(vobMounts.HasLocalReadMounts);
                        Assert.False(vobMounts.HasLocalWriteMounts);

                        #region Files no longer available

                        {
                            var readHandle = dataReference1.GetReadHandle<string>();
                            var persistenceResult = await readHandle.Resolve();
                            Assert.True(((IPersistenceResult)persistenceResult).Flags.HasFlag(PersistenceResultFlags.MountNotAvailable));
                            Assert.Null(persistenceResult.IsSuccess);
                        }
                        {
                            var readHandle = dataReference2.GetReadHandle<string>();
                            var persistenceResult = await readHandle.Resolve();
                            Assert.True(((IPersistenceResult)persistenceResult).Flags.HasFlag(PersistenceResultFlags.MountNotAvailable));
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
