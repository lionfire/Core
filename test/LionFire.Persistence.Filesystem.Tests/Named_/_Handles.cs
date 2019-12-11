using LionFire.Dependencies;
using LionFire.Hosting.ExtensionMethods;
using LionFire.Persistence;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.Filesystem.Tests;
using LionFire.Referencing;
using LionFire.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LionFire.Hosting;
using Xunit;
using System.Threading.Tasks;

namespace Named_
{
    public class _Handles
    {

        [Fact]
        public async void P_ReadHandle_NamedProvider_FileInRoot()
        {
            await PersistersHost.Create()
            .ConfigureServices((context, services) =>
            {
                services
                    .AddFilesystem()
                    .Configure<FilesystemPersisterOptions>("UnitTestRoot", c => c.RootDirectory = FsTestUtils.DataDir)
                ;
            })
            .Run(async () =>
            {
                var path = FsTestUtils.TestFile + ".txt";
                Assert.False(File.Exists(path));

                var testContents = "F70625BC-A050-4F36-9710-9DC4AB8C40B6";
                File.WriteAllText(path, testContents);
                Assert.True(File.Exists(path));

                {
                    var reference = new ProviderFileReference("UnitTestRoot", Path.GetFileName(path));

                    Assert.Equal(Path.GetFileName(path), reference.Path);
                    Assert.Equal("UnitTestRoot", reference.Persister);

                    var readHandle = reference.GetReadHandle<string>();
                    var persistenceResult = await readHandle.Retrieve();

                    Assert.True(persistenceResult.Flags.HasFlag(PersistenceResultFlags.Success));
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

        [Fact]
        public async void P_ReadHandle_NamedProvider_FileInSubDir()
        {
            var testDirGuid = Guid.NewGuid();
            await PersistersHost.Create()
            .ConfigureServices((context, services) =>
            {
                services
                    .AddFilesystem()
                    .Configure<FilesystemPersisterOptions>("UnitTestDir", c => c.RootDirectory = FsTestUtils.DataDir)
                ;
            })
            .Run(async () =>
            {
                var subdir = "UnitTestSubdir-" + testDirGuid;
                var dir = Path.Combine(FsTestUtils.DataDir, subdir);
                Assert.False(Directory.Exists(dir));
                Directory.CreateDirectory(dir);
                Assert.True(Directory.Exists(dir));


                var path = FsTestUtils.TestFileForDir(dir) + ".txt";
                Assert.False(File.Exists(path));

                var testContents = "12341234-A050-4F36-9710-9DC4AB8C40B6";
                File.WriteAllText(path, testContents);
                Assert.True(File.Exists(path));

                {
                    var reference = new ProviderFileReference("UnitTestDir", LionPath.Combine(subdir, Path.GetFileName(path)));
                    Assert.Equal(subdir + "/" + Path.GetFileName(path), reference.Path);
                    Assert.Equal("UnitTestDir", reference.Persister);

                    var readHandle = reference.GetReadHandle<string>();
                    var persistenceResult = await readHandle.Retrieve();

                    Assert.True(persistenceResult.Flags.HasFlag(PersistenceResultFlags.Success));
                    Assert.Equal(testContents, readHandle.Value);
                }

                File.Delete(path);
                Assert.False(File.Exists(path));

                Directory.Delete(dir);
                Assert.False(Directory.Exists(dir));


            });
        }

        [Fact]
        public async void Ex_ReadHandle_NamedProvider_Unknown()
        {
            await PersistersHost.Create()
            .ConfigureServices((context, services) =>
            {
                services
                    .AddFilesystem()
                    .Configure<FilesystemPersisterOptions>("UnitTestRoot", c => c.RootDirectory = FsTestUtils.DataDir)
                ;
            })
            .Run(async () =>
            {
                var path = FsTestUtils.TestFile + ".txt";

                var reference = new ProviderFileReference("UnitTestRoot-Unknown1234567890", Path.GetFileName(path));
                Assert.Equal("UnitTestRoot-Unknown1234567890", reference.Persister);

                var readHandle = reference.GetReadHandle<string>();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                await Assert.ThrowsAsync<UnknownPersisterException>(() => readHandle.Retrieve());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            });
        }

    }
}
