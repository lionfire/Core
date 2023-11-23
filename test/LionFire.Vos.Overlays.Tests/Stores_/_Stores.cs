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
using LionFire.Vos.Packages;
using System.Threading.Tasks;
using LionFire.Vos.Services;
using LionFire.Data;

namespace Stores_
{
    public class _Stores
    {


        // TODO: Test writing files
        // TODO: Verify read priorities
        // TODO: Verify write priorities

        async Task run(string dataDir)
        {
            var storesManagerPath = "/stores";
            var exeDir = Path.GetDirectoryName(this.GetType().Assembly.Location);

            var path1 = Path.Combine(exeDir, "file1" + ".txt");
            var testContents1 = "B9E72769-E1DA-4648-B766-FAE37D2317E5";
            var path2 = Path.Combine(dataDir, "file2" + ".txt");
            var testContents2 = "5A18BB7B-85A3-4A30-93D4-89A90510EBCC";

            #region Create Test File

            if (File.Exists(path1)) File.Delete(path1);
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

            var directReference1 = new VobReference(storesManagerPath + "/available/ExeDir/file1.txt");
            var directReference2 = new VobReference(storesManagerPath + "/available/DataDir/file2.txt");
            var dataReference1 = new VobReference(storesManagerPath + "/data/file1.txt");
            var dataReference2 = new VobReference(storesManagerPath + "/data/file2.txt");

            Assert.False(await directReference1.GetReadHandle<string>().Exists()); // False due to VobMountOptions.IsManuallyMounted = true
            Assert.False(await directReference2.GetReadHandle<string>().Exists());
            Assert.False(await dataReference1.GetReadHandle<string>().Exists());
            Assert.False(await dataReference2.GetReadHandle<string>().Exists());

            // How to get a Vob?  VobReference.ToVob() might be nice.  How about VobReference.ToVob().AsType<PackageManager>()
            var packageProvider = "/".GetVob().Acquire<PackageProvider>();
            //var storesManager = "/".GetVob().GetService<ServiceDirectory>().GetRequiredService<PackageProvider>();
            //var storesManager = storesManagerPath.ToVob().GetMultiTyped().AsType<PackageManager>();
            Assert.NotNull(packageProvider);

            //Assert.True(pluginManager.AvailablePackages.Contains("plugin1")); // TODO - once some sort of GetChildren functionality is available 
            //Assert.True(pluginManager.AvailablePackages.Contains("plugin2"));
            Assert.False(Enumerable.Any<string>(packageProvider.EnabledPackages));

            #region ExeDir
            {
                packageProvider.Enable("ExeDir");
                Assert.Contains((System.Collections.Generic.IEnumerable<string>)packageProvider.EnabledPackages, s => s == "ExeDir");
                Assert.Single((System.Collections.Generic.IEnumerable<string>)packageProvider.EnabledPackages);

                var readHandle = dataReference1.GetReadHandle<string>();
                var persistenceResult = await readHandle.Get();

//#error NEXT: this fails for P_Normal.  Verify in Enable that it is getting mounted.
                Assert.True(persistenceResult.Flags.HasFlag(TransferResultFlags.Success));
                Assert.True(persistenceResult.IsSuccess);
                Assert.Equal(testContents1, readHandle.Value);
            }
            {
                var readHandle = directReference1.GetReadHandle<string>();
                var persistenceResult = await readHandle.Get();
                Assert.True(persistenceResult.IsSuccess);
                Assert.Equal(testContents1, readHandle.Value);
            }
            #endregion

            #region DataDir
            {
                Assert.False(await dataReference2.GetReadHandle<string>().Exists());
                packageProvider.Enable("DataDir");
                Assert.Contains((System.Collections.Generic.IEnumerable<string>)packageProvider.EnabledPackages, s => s == "DataDir");
                Assert.Equal(2, Enumerable.Count<string>(packageProvider.EnabledPackages));

                var readHandle = dataReference2.GetReadHandle<string>();
                var persistenceResult = await readHandle.Get();

                Assert.True(persistenceResult.IsSuccess);
                Assert.Equal(testContents2, readHandle.Value);

            }
            {
                var readHandle = directReference2.GetReadHandle<string>();
                var persistenceResult = await readHandle.Get();
                Assert.True(persistenceResult.IsSuccess);
                Assert.Equal(testContents2, readHandle.Value);
            }
            #endregion

            Assert.True((bool)packageProvider.Disable("ExeDir"));
            Assert.True((bool)packageProvider.Disable("DataDir"));
            Assert.Empty((System.Collections.IEnumerable)packageProvider.EnabledPackages);

            var vobMounts = new VobReference("/stores/data").GetVob().AcquireOwn<VobMounts>();
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
                    var parentDir = Directory.GetParent(path).FullName;
                    if (Directory.GetFiles(parentDir).Length == 0)
                    {
                        try
                        {
                            Directory.Delete(parentDir);
                        }
                        catch { } // EMPTYCATCH
                        Assert.False(Directory.Exists(Directory.GetParent(path).FullName));
                    }
                }
            }
            DeleteFile(path1, true);
            DeleteFile(path2, true);
        }

        [Fact]
        public async void P_ManualSetup()
        {
            var dataDir = Path.Combine(FsTestSetup.DataDir, "UnitTest " + Guid.NewGuid().ToString(), "TestPluginsDiskDir");

            await VosHost.Create()
                    .ConfigureServices((context, services) =>
                    {
                        services
                        .AddFilesystem()

                        .VosPackageProvider("/stores".ToVobReference())

                        .VosMount("/stores/available/ExeDir", Path.GetDirectoryName(this.GetType().Assembly.Location).ToFileReference(), new VobMountOptions { MustBeManuallyEnabled = true })
                        .InitializeVob("/stores/available/ExeDir", v => v.AddOwn(v => new VobMountOptions
                        {
                            Name = "ExecutableDirectory",
                            ReadPriority = 10,
                            WritePriority = null,
                        }))
                        .VosMount("/stores/available/DataDir", dataDir.ToFileReference(), new VobMountOptions { MustBeManuallyEnabled = true })
                        .InitializeVob("/stores/available/DataDir", v => v.AddOwn(v => new VobMountOptions
                        {
                            Name = "DataDirectory",
                            ReadPriority = 100,
                            //WritePriority = 10,
                        }));
                    })
                    .RunAsync((Func<System.Threading.Tasks.Task>)(async () =>
                    {
                        await run(dataDir);
                    }));
        }

#if TODO
        [Fact]
        public async void P_Normal()
        {
            var dataDir = Path.Combine(FsTestUtils.DataDir, "UnitTest " + Guid.NewGuid().ToString(), "TestPluginsDiskDir");

            await VosHost.Create()
                    .ConfigureServices((context, services) =>
                    {
                        services
                        .AddFilesystem()

                        // NEXT: - this is null            var storesManager = storesManagerPath.ToVob().GetMultiTyped().AsType<PackageManager>();

                        //.AddVosStores("/.stores", c =>
                        //{
                        //    c.AddStore("ExeDir", Path.GetDirectoryName(this.GetType().Assembly.Location).ToFileReference(), new VobMountOptions(10, name: "ExecutableDirectory"));
                        //})

#if truex // TODO

#if ENV
                        .VosEnvironment("stores", "/.stores-fromEnv")
                        .AddVosStores("$stores") 
#endif

#if VosApp
                                    .AddVosStores() // TODO: VosApp dll
#endif

                                    .VosEntrypointStore() // TODO: Once VosStores service is available? // VosStoreNames.EntrypointDir, this.GetType().Assembly.Location.ToFileReference())
                                    .VosMountAvailablePackage("/stores", "ExeDir", this.GetType().Assembly.Location.ToFileReference()) // TODO: resolves to correct subpath for "available"
#endif


                        //.VosPackageManager("/stores")
                        //.VosMount("/stores/available/ExeDir", Path.GetDirectoryName(this.GetType().Assembly.Location).ToFileReference(), new VobMountOptions { /*IsDisabled = true */})
                        //.InitializeVob("/stores/available/ExeDir", v => v.AddOwn(v => new VobMountOptions
                        //{
                        //    Name = "ExecutableDirectory",
                        //    ReadPriority = 10,
                        //    WritePriority = null,
                        //}))
                        //.VosMount("/stores/available/DataDir", dataDir.ToFileReference(), new VobMountOptions { /*IsDisabled = true */})
                        //.InitializeVob("/stores/available/DataDir", v => v.AddOwn(v => new VobMountOptions
                        //{
                        //    Name = "DataDirectory",
                        //    ReadPriority = 100,
                        //    //WritePriority = 10,
                        //}));
                        //#error NEXT: More elegant mount options for ExeDir

                        //new VobMountOptions(
                        //    decorators: new MultiType(new MultiTypableVisitor<IVobNodeProvider>(()=> new VobMountOptions { Name = "UnitTestPlugin", ReadPriority = 100, WritePriority = -100 }))
                        //{
                        //    Name = "UnitTestPluginsDir"
                        //})

                        //.VosMount("/`/PluginData", "/`/TestPlugins/data".ToVobReference()) // Custom data dir for app

                        // MOVE - FUTURE: Vos metadata
                        //.VosMount("/_/vos", new VobReference("/") { Persister = "vos" }, new VobMountOptions
                        //{
                        //    IsReadOnly = true,
                        //    IsExclusive = true,
                        //})
                        ;
                    })
                    .Run((Func<System.Threading.Tasks.Task>)(async () =>
                    {
                        await run(dataDir);
                    }));
        }
#endif
    }
}
