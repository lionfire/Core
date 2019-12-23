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
using LionFire.Persistence.Filesystem.Tests;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.CouchDB;
using Microsoft.Extensions.DependencyInjection;
using LionFire.CouchDB;
using LionFire.Services;
using LionFire.Referencing;

namespace UniversalPersister_
{
    public class CouchDBTests : IPersisterTestInitializer
    {
        public string Scheme => "couch";

        public IServiceCollection AddServicesForTest(IServiceCollection services)
        {
            services.AddCouchDB(MyCouchMode.Entity);
            //services.Configure<CouchDBConnectionOptions>(o => o.ConnectionString = "http://unitTest:unitTestPassword@localhost");
            //services.ConfigureConnection<CouchDBConnectionOptions>(new Uri("http://unitTest:unitTestPassword@localhost"));
            services.ConfigureConnection<CouchDBConnectionOptions>("http://unitTest:unitTestPassword@localhost");
            return services;
        }
        public string GetPathForTestPath(string testPath) => Path.Combine(FsTestUtils.DataDir, testPath);
        public IReference GetReferenceForTestPath(string testPath) => new CouchDBReference(GetPathForTestPath(testPath));
    }
    public class FileTests : IPersisterTestInitializer
    {
        public string Scheme => "file";

        public IServiceCollection AddServicesForTest(IServiceCollection services)
        {
            return services
                              .AddFilesystem()
                              ;
        }

        public string GetPathForTestPath(string testPath) => Path.Combine(FsTestUtils.DataDir, testPath);
        public IReference GetReferenceForTestPath(string testPath) => new FileReference(GetPathForTestPath(testPath));

    }

    namespace NewtonsoftJson
    {
        public class _Upsert_Already
        {
            // ENH: pass IServiceCollection initializer in parameters


            [Theory]
            [ClassData(typeof(UniversalPersistersGenerator))]
            public async void P_TestObj(IPersisterTestInitializer initializer)
            {

                await PersistersHost.Create()
                    .ConfigureServices(services =>
                    {
                        initializer.AddServicesForTest(services);

                    })
                    .Run(async () =>
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
                        await rwh.Put();
                    }
                    #endregion

                    #region Exists

                    #endregion

                    #region Retrieve
                    {
                        var rh =  reference.GetReadHandle<string>();
                        var retrieveResult = (await rh.Resolve()).ToRetrieveResult();
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

                    // OLD - review and delete
                    //var path = FsTestUtils.TestFile;
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
                await PersistersHost.Create().Run(async () =>
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
                await PersistersHost.Create().Run(async () =>
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
}