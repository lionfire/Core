//using LionFire.ObjectBus.Filesystem.Persisters;
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
using LionFire.Dependencies;
using DeepEqual.Syntax;
using LionFire.Persistence.Filesystem.Tests;
using LionFire.Persistence.Filesystem;

namespace UniversalPersister_
{
    namespace NewtonsoftJson
    {
        public class _Retrieve
        {
            [Fact]
            public async void P_TestObj()
            {
                await PersistersHost.Create().Run(async () =>
                {
                    var path = FsTestUtils.TestFile + ".json";
                    Assert.False(File.Exists(path));

                    File.WriteAllText(path, TestClass1.ExpectedNewtonsoftJson);
                    Assert.True(File.Exists(path));

                    var retrieveResult = await DependencyLocator.Get<FilesystemPersister>().Retrieve<TestClass1>(path.ToFileReference());

                    Assert.True(retrieveResult.Value.IsDeepEqual(TestClass1.Create));

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
                    File.WriteAllText(path, testContents);
                    Assert.True(File.Exists(path));

                    var retrieveResult = await DependencyLocator.Get<FilesystemPersister>().Retrieve<string>(path.ToFileReference());

                    Assert.Equal(testContents, retrieveResult.Value);
                    Assert.Equal(PersistenceResultFlags.Found | PersistenceResultFlags.Success, retrieveResult.Flags);

                    File.Delete(path);
                    Assert.False(File.Exists(path));
                });
            }

            // INTERMITTENT Failure?  (191201 -   System.ArgumentNullException : Value cannot be null. (Parameter 'source'), in AutoRetry)
            [Fact]
            public async void P_bytes()
            {
                await PersistersHost.Create().Run(async () =>
                {
                    var path = FsTestUtils.TestFile + ".bin";
                    Assert.False(File.Exists(path));

                    var testContents = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 32, 33, 34, 35, 64, 65, 66, 67, 68 };

                    var testContentsSame = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 32, 33, 34, 35, 64, 65, 66, 67, 68 };
                    Assert.True(((ReadOnlySpan<byte>)testContents).SequenceEqual(testContentsSame));

                    var testContentsDifferent = new byte[] { 11, 2, 3, 4, 5, 6, 7, 88, 99, 10, 32, 33, 34, 35, 64, 65, 66, 67, 68 };
                    Assert.False(((ReadOnlySpan<byte>)testContents).SequenceEqual(testContentsDifferent));

                    File.WriteAllBytes(path, testContents);
                    Assert.True(File.Exists(path));

                    var retrieveResult = await DependencyLocator.Get<FilesystemPersister>().Retrieve<byte[]>(path.ToFileReference());

                    Assert.True(((ReadOnlySpan<byte>)testContents).SequenceEqual(retrieveResult.Value)); // Primary ASSERT
                    Assert.False(((ReadOnlySpan<byte>)testContentsDifferent).SequenceEqual(retrieveResult.Value));
                    Assert.Equal(PersistenceResultFlags.Found | PersistenceResultFlags.Success, retrieveResult.Flags);

                    File.Delete(path);
                    Assert.False(File.Exists(path));
                });
            }
        }
    }
}