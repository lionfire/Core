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
using LionFire.Serialization.Json.Newtonsoft;
using LionFire.Dependencies;
using LionFire.Persistence.Filesystem.Tests;
using LionFire.Persistence.Filesystem;
using System.Threading.Tasks;
using LionFire.Persistence.Persisters;
using LionFire.Persistence.Testing;

namespace UniversalPersister_
{
    // TODO: Prefer Run argument injection over DependencyLocator here and in other unit tests?  DependencyLocator relies on ambient IServiceProvider

    namespace NewtonsoftJson
    {
        // TODO MOVE: These are Filesystem-specific tests.  
        public class _Update
        {
      

            #region Fail - Missing

            [Fact]
            public async void F_TestObj_Missing()
            {
                await TestHostBuilders.CreateFileNewtonsoftHost().RunAsync(async () =>
                {
                    var path = FsTestUtils.TestFile + ".json";
                    Assert.False(File.Exists(path));

                    // Don't create:
                    //File.WriteAllText(path, TestClass1.ExpectedNewtonsoftJson);
                    //Assert.True(File.Exists(path));

                    var testContents2 = TestClass1.Create;
                    testContents2.StringProp = "Contents #2";
                    testContents2.IntProp++;
                    var serializedTestContents2 = ServiceLocator.Get<NewtonsoftJsonSerializer>().ToString(testContents2).String;

                    await Assert.ThrowsAsync<NotFoundException>(() => ServiceLocator.Get<FilesystemPersister>().Update(path.ToFileReference(), testContents2));

                    Assert.False(File.Exists(path));
                });
            }

            #endregion

            #region Pass

            [Fact]
            public async void P_TestObj()
            {
                await TestHostBuilders.CreateFileNewtonsoftHost().RunAsync(async () =>
                {
                    var path = FsTestUtils.TestFile + ".json";
                    Assert.False(File.Exists(path));

                    File.WriteAllText(path, TestClass1.ExpectedNewtonsoftJson);
                    Assert.True(File.Exists(path));

                    var testContents2 = TestClass1.Create;
                    testContents2.StringProp = "Contents #2";
                    testContents2.IntProp++;
                    var serializedTestContents2 = ServiceLocator.Get<NewtonsoftJsonSerializer>().ToString(testContents2).String;


                    await ServiceLocator.Get<FilesystemPersister>().Update(path.ToFileReference(), testContents2);
                    Assert.True(File.Exists(path));

                    var fromFile = File.ReadAllText(path);
                    Assert.Equal(serializedTestContents2, fromFile);

                    File.Delete(path);
                    Assert.False(File.Exists(path));
                });
            }

            [Fact]
            public async void P_string()
            {
                await TestHostBuilders.CreateFileNewtonsoftHost().RunAsync(async () =>
                {
                    var path = FsTestUtils.TestFile + ".txt";
                    Assert.False(File.Exists(path));

                    var testContents = "testing123";
                    File.WriteAllText(path, testContents);

                    var testContents2 = "test456";
                    await ServiceLocator.Get<FilesystemPersister>().Update(path.ToFileReference(), testContents2);
                    Assert.True(File.Exists(path));

                    var fromFile = File.ReadAllText(path);
                    Assert.Equal(testContents2, fromFile);

                    File.Delete(path);
                    Assert.False(File.Exists(path));
                });
            }

            //[Theory]
            //[ClassData(typeof(UniversalPersistersGenerator))]
            [Fact]
            public async void P_bytes()
            {
                await TestHostBuilders.CreateFileNewtonsoftHost()
                    .RunAsync(async (FilesystemPersister persister) =>
                  {
                      var path = FsTestUtils.TestFile + ".bin";
                      Assert.False(File.Exists(path));

                      var testContents = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 32, 33, 34, 35, 64, 65, 66, 67, 68 };

                      File.WriteAllBytes(path, testContents);
                      Assert.True(File.Exists(path));

                      var testContents2 = new byte[] { 100, 200, 30, 40, 50, 60, 70, 80, 90, 100, 132, 133, 134, 135, 1, 2, 0, 0 };

                      //var persister = (IPersister)sp.GetService(initializer.PersisterType); // REVIEW - Not going to work.  IPersister requires generic parameter.

                      await persister.Update(path.ToFileReference(), testContents2);
                      Assert.True(File.Exists(path));

                      var fromFile = File.ReadAllBytes(path);
                      Assert.Equal(testContents2, fromFile);

                      File.Delete(path);
                      Assert.False(File.Exists(path));
                  });
            }

            #endregion
        }
    }
}