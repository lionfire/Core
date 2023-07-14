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
using LionFire.Serialization.Json.Newtonsoft;
using LionFire.Dependencies;
using LionFire.Referencing;
using LionFire.Services;
using LionFire.IO;
using Microsoft.Extensions.DependencyInjection;

namespace Universal_.ReadWriteHandle_
{
    public class _Upsert_Already
    {


        [Theory]
        [ClassData(typeof(UniversalPersistersGenerator))]
        public async void P_TestObj(IPersisterTestInitializer initializer)
        {
            await TestHostBuilders.CreateFileNewtonsoftHost(initializer)
                .RunAsync(async () =>
            {
                var testPath = $"UnitTest - {Guid.NewGuid().ToString()}.json";
                IReference reference = initializer.GetReferenceForTestPath(testPath);

                var testData = "Test data: " + Guid.NewGuid().ToString();

                #region !Exists
                {
                    var rh = reference.GetReadHandle<string>();
                    Assert.False(await rh.Exists(), "Unexpected: exists before creation.");
                }
                #endregion

                #region Create

                // TODO: Try other types of data
                {
                    var rwh = reference.GetReadWriteHandle<string>();
                    rwh.Value = testData;
                    var result = await rwh.Set();
                    Assert.True(result.IsSuccess);
                }
                #endregion

                #region Exists

                #endregion

                #region Retrieve
                {
                    var rh = reference.GetReadHandle<string>();
                    var retrieveResult = (await rh.Get());
                    Assert.True(retrieveResult.IsSuccess());
                    Assert.True(retrieveResult.IsFound());
                    Assert.Equal(testData, rh.Value);
                }
                #endregion

                #region Delete
                {
                    var rwh = reference.GetReadWriteHandle<string>();
                    var deleteResult = await rwh.Delete();
                    Assert.True(deleteResult != false);
                }
                #endregion

                #region !Exists
                {
                    var rh = reference.GetReadHandle<string>();
                    Assert.False(await rh.Exists(), "Still exists after deletion");
                }
                #endregion

                // TODO: Assert existing handles get deletion event

                // OLD - review and delete
                //var path = FsTestSetup.TestFile;
                //Assert.False(File.Exists(path));

                //File.WriteAllText(path, TestClass1.ExpectedNewtonsoftJson);
                //Assert.True(File.Exists(path));

                //var testContents2 = TestClass1.Create;
                //testContents2.StringProp = "Contents #2";
                //testContents2.IntProp++;
                //var serializedTestContents2 = DependencyLocator.Get<NewtonsoftJsonSerializer>().ToString(testContents2).String;

                //var reference = new CouchDBReference(path);

                //await DependencyLocator.Get<FilesystemPersister>().Upsert(path.ToFileReference(), testContents2);
                //Assert.True(File.Exists(path));

                //var fromFile = File.ReadAllText(path);
                //Assert.Equal(serializedTestContents2, fromFile);

                //File.Delete(path);
                //Assert.False(File.Exists(path));
            });
        }


        // TODO: Combine with P_TestObj via ClassData?
        [Theory]
        [ClassData(typeof(UniversalPersistersGenerator))]
        public async void P_DefaultExtension(IPersisterTestInitializer initializer)
        {
            await TestHostBuilders.CreateFileNewtonsoftHost(initializer)
                .ConfigureServices((context, services) =>
                {
                    services
                        .Configure<SerializationOptions>(so => so.TreatExtensionlessAsExtension = "json")
                        ;
                    //services.TryAddEnumerableSingleton<ISerializeScorer>(new DefaultExtensionScorer("json", IODirection.ReadWrite));
                })
                .RunAsync(async () =>
                {
                    var testPath = $"UnitTest - {Guid.NewGuid().ToString()}";
                    IReference reference = initializer.GetReferenceForTestPath(testPath);

                    var testData = "Test data: " + Guid.NewGuid().ToString();

                    #region !Exists
                    {
                        var rh = reference.GetReadHandle<string>();
                        Assert.False(await rh.Exists(), "Unexpected: exists before creation.");
                    }
                    #endregion

                    #region Create

                    // TODO: Try other types of data
                    {
                        var rwh = reference.GetReadWriteHandle<string>();
                        rwh.Value = testData;
                        var result = await rwh.Set();
                        Assert.True(result.IsSuccess);
                    }
                    #endregion

                    #region Exists

                    #endregion

                    #region Retrieve
                    {
                        var rh = reference.GetReadHandle<string>();
                        var getResult = (await rh.Get());
                        Assert.True(getResult.IsSuccess());
                        Assert.True(getResult.IsFound());
                        Assert.Equal(testData, rh.Value);
                    }
                    #endregion

                    #region Delete
                    {
                        var rwh = reference.GetReadWriteHandle<string>();
                        var deleteResult = await rwh.Delete();
                        Assert.True(deleteResult != false);
                    }
                    #endregion

                    #region !Exists
                    {
                        var rh = reference.GetReadHandle<string>();
                        Assert.False(await rh.Exists(), "Still exists after deletion");
                    }
                    #endregion

                    // TODO: Assert existing handles get deletion event

                    // OLD - review and delete
                    //var path = FsTestSetup.TestFile;
                    //Assert.False(File.Exists(path));

                    //File.WriteAllText(path, TestClass1.ExpectedNewtonsoftJson);
                    //Assert.True(File.Exists(path));

                    //var testContents2 = TestClass1.Create;
                    //testContents2.StringProp = "Contents #2";
                    //testContents2.IntProp++;
                    //var serializedTestContents2 = DependencyLocator.Get<NewtonsoftJsonSerializer>().ToString(testContents2).String;

                    //var reference = new CouchDBReference(path);

                    //await DependencyLocator.Get<FilesystemPersister>().Upsert(path.ToFileReference(), testContents2);
                    //Assert.True(File.Exists(path));

                    //var fromFile = File.ReadAllText(path);
                    //Assert.Equal(serializedTestContents2, fromFile);

                    //File.Delete(path);
                    //Assert.False(File.Exists(path));
                });
        }



#if TOPORT

            [Fact]
            public async void P_string()
            {
                await TestHostBuilders.CreateFileHost().Run(async () =>
                {
                    var path = FsTestUtils.TestFile + ".txt";
                    Assert.False(File.Exists(path));

                    var testContents = "testing123";
                    File.WriteAllText(path, testContents);

                    var testContents2 = "test456";
                    await DependencyLocator.Get<FilesystemPersister>().Upsert(path.ToFileReference(), testContents2);
                    Assert.True(File.Exists(path));

                    var fromFile = File.ReadAllText(path);
                    Assert.Equal(testContents2, fromFile);

                    File.Delete(path);
                    Assert.False(File.Exists(path));
                });
            }

            [Fact]
            public async void P_bytes()
            {
                await TestHostBuilders.CreateFileHost().Run(async () =>
                {

                    var path = FsTestUtils.TestFile + ".bin";
                    Assert.False(File.Exists(path));

                    var testContents = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 32, 33, 34, 35, 64, 65, 66, 67, 68 };

                    File.WriteAllBytes(path, testContents);
                    Assert.True(File.Exists(path));

                    var testContents2 = new byte[] { 100, 200, 30, 40, 50, 60, 70, 80, 90, 100, 132, 133, 134, 135, 1, 2, 0, 0 };

                    await DependencyLocator.Get<FilesystemPersister>().Upsert(path.ToFileReference(), testContents2);
                    Assert.True(File.Exists(path));

                    var fromFile = File.ReadAllBytes(path);
                    Assert.Equal(testContents2, fromFile);

                    File.Delete(path);
                    Assert.False(File.Exists(path));
                });
            }
#endif
    }
}