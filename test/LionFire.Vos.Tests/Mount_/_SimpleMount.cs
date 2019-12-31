using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using Xunit;
using LionFire.Hosting;
using LionFire.Services;
using LionFire.Dependencies;
using LionFire.Persistence.Filesystem;
using System.IO;
using LionFire.Persistence.Filesystem.Tests;
using LionFire.Vos;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Referencing;
using LionFire.Vos.Mounts;

namespace Mount_
{
    public class _SimpleMount
    {
        [Fact]
        public async void Pass()
        {
            //await FrameworkHostBuilder.Create()
            //    .Run(async () =>
            //    {
            //        await Task.Delay(1);
            //    });
            await VosHost.Create()
            .ConfigureServices((context, services) =>
            {
                services
                    .AddFilesystem()
                    .VosMount("/testDir", FsTestUtils.DataDir.ToFileReference())
                    .VosMount("/testDir2", FsTestUtils.DataDir.ToFileReference())
                    .VosMount("/_/vos", new VosReference("/") { Persister = "vos" }, new MountOptions
                    {
                        IsReadOnly = true,
                        IsExclusive = true,
                    })
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
