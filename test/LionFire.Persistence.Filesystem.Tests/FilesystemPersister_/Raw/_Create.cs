using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.IO;
using LionFire.ObjectBus;
using LionFire;
using LionFire.Serialization;
using LionFire.Hosting;
using Microsoft.Extensions.Hosting;
using LionFire.Applications.Hosting;
using LionFire.Persistence;
using LionFire.ObjectBus.Testing;
using LionFire.Serialization.Json.Newtonsoft;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Persistence.Filesystem.Tests;
using LionFire.Persistence.Filesystem;
using LionFire.Dependencies;

namespace FilesystemPersister_
{
    namespace Raw
    {
        public class _Create
        {
            [Fact]
            public async void P_string()
            {
                await PersistersHost.Create()
                .ConfigureServices(services =>
                {
                    services.Configure<SerializationOptions>(o =>
                    {
                        //o.SerializeExtensionScoring = FileExtensionScoring.MustMatch;
                        //o.DeerializeExtensionScoring = FileExtensionScoring.RewardMatch;
                    });
                })
                .RunAsync(async () =>
                {
                    var path = FsTestUtils.TestFile + ".txt";
                    Assert.False(File.Exists(path));

                    var testContents = "testing123";
                    var persistenceResult = await DependencyLocator.Get<FilesystemPersister>().Create(path.ToFileReference(), testContents);

                    Assert.True(persistenceResult.Flags.HasFlag(PersistenceResultFlags.Success));

                    Assert.True(File.Exists(path));
                    var fromFile = File.ReadAllText(path);
                    Assert.Equal(testContents, fromFile);

                    File.Delete(path);
                    Assert.False(File.Exists(path));
                });
            }

            [Fact]
            public async void P_bytes()
            {
                await PersistersHost.Create().RunAsync(async () =>
                {
                    var path = FsTestUtils.TestFile + ".bin";
                    Assert.False(File.Exists(path));
                    
                    var testContents = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 32, 33, 34, 35, 64, 65, 66, 67, 68 };
                    var persistenceResult = await DependencyLocator.Get<FilesystemPersister>().Create(path.ToFileReference(), testContents);
                    
                    Assert.True(persistenceResult.Flags.HasFlag(PersistenceResultFlags.Success), "!PersistenceResultFlags.Success");

                    Assert.True(File.Exists(path));
                    var fromFile = File.ReadAllBytes(path);
                    Assert.Equal(testContents, fromFile);

                    File.Delete(path);
                    Assert.False(File.Exists(path));
                });
            }
        }
    }
}