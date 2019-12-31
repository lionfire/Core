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

namespace LionFire.Vos.Packages.Tests
{
    public class _Package
    {
        [Fact]
        public async void Pass()
        {
            var pluginsDir = Path.Combine(FsTestUtils.DataDir, "TestPlugins");
            var packsDir = Path.Combine(FsTestUtils.DataDir, "TestPacks");

            await VosAppHost.Create()
                    .ConfigureServices((context, services) =>
                    {
                        services
                        .AddFilesystem()
                        .InitializeVob("/`/TestPacks", v => v.AddPackageManager())
                        .VosMount("/`/TestPacks/available", pluginsDir.ToFileReference())
                        .InitializeVob("/`/TestPlugins", v => v.AddPackageManager())
                        .VosMount("/`/TestPlugins/available", packsDir.ToFileReference())


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
                        var path = FsTestUtils.TestFile + ".txt";
                        var testContents = "B9E72769-E1DA-4648-B766-FAE37D2317E5";

                #region Create Test File
                Assert.False(File.Exists(path));
                        File.WriteAllText(path, testContents);
                        Assert.True(File.Exists(path));
                #endregion

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
