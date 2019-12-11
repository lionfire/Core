//using LionFire.ObjectBus.Filesystem.Persisters;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using LionFire.ObjectBus.Filesystem;
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
using LionFire.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Persistence.Filesystem.Tests;
using LionFire.Persistence.Filesystem;

namespace FilesystemPersister_
{
    
    namespace NewtonsoftJson
    {
        public class _Create
        {
            #region Pass

            [Fact]
            public async void P_TestObj()
            {
                await PersistersHost.Create()
                    .ConfigureServices(services =>
                    {
                        services.Configure<SerializationOptions>(o =>
                        {
                            o.SerializeExtensionScoring = FileExtensionScoring.RewardMatch;
                        });
                    })
                    .Run(async () =>
                {
                    var path = FsTestUtils.TestFile + ".json";

                    var testContents = TestClass1.Create;
                    var serializedTestContents = DependencyLocator.Get<NewtonsoftJsonSerializer>().ToString(testContents).String;

                    Assert.False(File.Exists(path));

                    await DependencyLocator.Get<FilesystemPersister>().Create(path.ToFileReference(), testContents);

                    Assert.True(File.Exists(path));

                    var fromFile = File.ReadAllText(path);
                    Assert.Equal(serializedTestContents, fromFile);

                    File.Delete(path);

                    Assert.False(File.Exists(path));
                });
            }

            [Fact]
            public async void P_string()
            {
                await PersistersHost.Create().Run(async () =>
                {
                    var path = FsTestUtils.TestFile + ".txt";
                    Assert.False(File.Exists(path));

                    var testContents = "testing123";
                    await DependencyLocator.Get<FilesystemPersister>().Create(path.ToFileReference(), testContents);

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
                await PersistersHost.Create().Run(async () =>
                {
                    var path = FsTestUtils.TestFile + ".bin";
                    Assert.False(File.Exists(path));

                    var testContents = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 32, 33, 34, 35, 64, 65, 66, 67, 68 };
                    await DependencyLocator.Get<FilesystemPersister>().Create(path.ToFileReference(), testContents);
                    Assert.True(File.Exists(path));

                    var fromFile = File.ReadAllBytes(path);
                    Assert.Equal(testContents, fromFile);

                    File.Delete(path);
                    Assert.False(File.Exists(path));
                });
            }

            
            // This is a STUB sample of how to do streams now. TODO: Don't use StreamToBytes -- buffer the output
            [Fact]
            public async void P_Stream()
            {
                await PersistersHost.Create().Run(async () =>
                {
                    var path = FsTestUtils.TestFile + ".bin";
                    Assert.False(File.Exists(path));

                    var testContents = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 32, 33, 34, 35, 64, 65, 66, 67, 68 };
                    var ms = new MemoryStream();
                    ms.Write(new ReadOnlySpan<byte>(testContents));
                    ms.Seek(0, SeekOrigin.Begin);

                    await DependencyLocator.Get<FilesystemPersister>().Create(path.ToFileReference(), ms.StreamToBytes());
                    Assert.True(File.Exists(path));

                    var fromFile = File.ReadAllBytes(path);
                    Assert.Equal(testContents, fromFile);

                    File.Delete(path);
                    Assert.False(File.Exists(path));
                });
            }

            #endregion

            #region Fail: Already exists

            [Fact]
            public async void F_TestObj_Already()
            {
                await PersistersHost.Create()
                    //.ConfigureServices(s =>
                    //{
                    //    s.AddSingleton<FilesystemPersistenceOptions>(new FilesystemPersistenceOptions
                    //    {
                            
                    //    });
                    //})
                    .Run(async () =>
                {
                    var path = FsTestUtils.TestFile + ".json";
                    Assert.False(File.Exists(path));

                    File.WriteAllText(path, TestClass1.ExpectedNewtonsoftJson);
                    Assert.True(File.Exists(path));

                    var testContents2 = TestClass1.Create;
                    testContents2.StringProp = "Contents #2";
                    testContents2.IntProp++;
                    var serializedTestContents2 = DependencyLocator.Get<NewtonsoftJsonSerializer>().ToString(testContents2).String;

                    await Assert.ThrowsAsync<AlreadySetException>(async () => await DependencyLocator.Get<FilesystemPersister>().Create(path.ToFileReference(), testContents2));
                    Assert.True(File.Exists(path));

                    var fromFile = File.ReadAllText(path);
                    Assert.Equal(TestClass1.ExpectedNewtonsoftJson, fromFile); // Original data

                    File.Delete(path);
                    Assert.False(File.Exists(path));
                });
            }

            [Fact]
            public async void F_string_Already()
            {
                await PersistersHost.Create().Run(async () =>
                {
                    var path = FsTestUtils.TestFile + ".txt";
                    Assert.False(File.Exists(path));

                    var testContents = "testing123";
                    File.WriteAllText(path, testContents);

                    var testContents2 = "test456";
                    await Assert.ThrowsAsync<AlreadySetException>(async () => await DependencyLocator.Get<FilesystemPersister>().Create(path.ToFileReference(), testContents2));
                    Assert.True(File.Exists(path));

                    var fromFile = File.ReadAllText(path);
                    Assert.Equal(testContents, fromFile); // Original data

                    File.Delete(path);
                    Assert.False(File.Exists(path));
                });
            }

            [Fact]
            public async void F_bytes_Already()
            {
                await PersistersHost.Create().Run(async () =>
                {

                    var path = FsTestUtils.TestFile + ".bin";
                    Assert.False(File.Exists(path));

                    var testContents = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 32, 33, 34, 35, 64, 65, 66, 67, 68 };

                    File.WriteAllBytes(path, testContents);
                    Assert.True(File.Exists(path));

                    var testContents2 = new byte[] { 100, 200, 30, 40, 50, 60, 70, 80, 90, 100, 132, 133, 134, 135, 1, 2, 0, 0 };

                    await Assert.ThrowsAsync<AlreadySetException>(async () => await DependencyLocator.Get<FilesystemPersister>().Create(path.ToFileReference(), testContents2));
                    Assert.True(File.Exists(path));

                    var fromFile = File.ReadAllBytes(path);
                    Assert.Equal(testContents, fromFile); // Original data

                    File.Delete(path);
                    Assert.False(File.Exists(path));
                });
            }

            #endregion
        }
    }
}