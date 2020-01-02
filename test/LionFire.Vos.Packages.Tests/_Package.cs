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

namespace LionFire.Vos.Packages.Tests
{
    public class _Package
    {
        [Fact]
        public async void Pass()
        {
            var pluginsDir = Path.Combine(FsTestUtils.DataDir, "TestPlugins");
            //var packsDir = Path.Combine(FsTestUtils.DataDir, "TestPacks");

            await VosAppHost.Create()
                    .ConfigureServices((context, services) =>
                    {
                        services
                        .AddFilesystem()
                        //.InitializeVob("/`/TestPacks", v => v.AddPackageManager())
                        //.VosMount("/`/TestPacks/available", pluginsDir.ToFileReference())
                        .InitializeVob("/`/TestPlugins", v => v.AddPackageManager())
                        .VosMount("/`/TestPlugins/available", pluginsDir.ToFileReference())
                        .VosMount("/`/TestPlugins/data", "/`/PluginData".ToVosReference())
                        .VosPackageManager("/`/TestPlugins")


                        //.VosMount("/_/vos", new VosReference("/") { Persister = "vos" }, new MountOptions
                        //{
                        //    IsReadOnly = true,
                        //    IsExclusive = true,
                        //})
                        //.Configure<VosPackageOptions>() {
                        //        DefaultPackageSource = "/app/packages/available",
                        // }
                        //.TryAddEnumerableSingleton(new TMount("testDir", new FileReference(FsTestUtils.DataDir))
                        ;
                    })
                    .Run(async () =>
                    {

                        var path1 = Path.Combine(pluginsDir, "plugin1", "file1" + ".txt");
                        var testContents1 = "B9E72769-E1DA-4648-B766-FAE37D2317E5";
                        var path2 = Path.Combine(pluginsDir, "plugin2", "file2" + ".txt");
                        //var path2 = Path.Combine(FsTestUtils.DataDir, "file2" + ".txt");
                        var testContents2 = "5A18BB7B - 85A3 - 4A30 - 93D4 - 89A90510EBCC";

                        #region Create Test File

                        CreateTestFile(path1, testContents1);
                        CreateTestFile(path2, testContents2);
                        void CreateTestFile(string path, string contents)
                        {
                            Assert.False(File.Exists(path));
                            File.WriteAllText(path, testContents);
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
                        var pluginManager = "/`/TestPlugins".ToVob().GetMultiTyped().AsType<VosPackageManager>();

                        Assert(pluginManager.AvailablePackages
                        // TODO:
                        // Enable package 1
                        // Read file from /app/PackageData/test1.txt
                        // Enable package 2
                        // Read file from /app/PackageData/test2.txt
                        // Unmount package 1
                        // Ensure test1.txt doesn't exist in PackageData
                        // Read file from /app/PackageData/test2.txt
                        // Unmount package 2
                        // Ensure test2.txt doesn't exist in PackageData
                        // delete test files and folders
                        {

                            // TODO ----------------------------------- below is copied from another test.

                            var reference = new VosReference("testDir", Path.GetFileName(path));
                            Assert.Equal("testDir/" + Path.GetFileName(path), reference.Path);
                            //Assert.Equal("UnitTestRoot", reference.Persister);

                            var readHandle = reference.ToReadHandle<string>();
                            var persistenceResult = await readHandle.Resolve();

                            //Assert.True(persistenceResult.Flags.HasFlag(PersistenceResultFlags.Success)); // TODO - switch to Retrieve?
                            Assert.True(persistenceResult.IsSuccess);
                            Assert.Equal(testContents, readHandle.Value);
                        }

                        //{
                        //    FileReference reference = path;
                        //    Assert.Equal( path, reference.Path);

                        //    var readHandle = reference.GetReadHandle<string>();
                        //    var persistenceResult = await readHandle.Retrieve();

                        //    Assert.True(persistenceResult.Flags.HasFlag(PersistenceResultFlags.Success));
                        //    Assert.Equal(testContents, readHandle.Value);
                        //}
                        File.Delete(path);
                        Assert.False(File.Exists(path));
                    });
        }
    }
}
