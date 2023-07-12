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
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using DeepEqual.Syntax;
using LionFire.Persistence.Testing;

namespace Universal_.ListHandle_
{
    //public class ListingComparer<T> : IEqualityComparer<Listing<T>>
    //{
    //    public bool Equals([AllowNull] Listing<T> x, [AllowNull] Listing<T> y) => DeepEqual.E
    //    public int GetHashCode([DisallowNull] Listing<T> obj) => throw new NotImplementedException();
    //}
    public class List_
    {
        public static readonly string[] FileList = new string[]
       {
            "file1",
            "file2",
            ".hidden1",
            ".hidden2",
            "_special1",
            "_special2",
            "^meta1",
            "^meta2"
       };
        public static bool SanityChecks => false;  // MOVE

        [Theory]
        [ClassData(typeof(UniversalPersistersGenerator))]
        public async void P_List_Untyped(IPersisterTestInitializer initializer)
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
                IReference parentReference = initializer.GetReferenceForTestPath(testPath);

                var testData = "Test data: " + Guid.NewGuid().ToString();

                #region Create

                foreach (var filename in FileList)
                {
                    var reference = parentReference.GetChild(filename);
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
                        var result = await rwh.Put();
                        Assert.True(result.IsSuccess);
                    }
                    #endregion

                    #region Exists

                    #endregion

                    #region Retrieve // REDUNDANT
                    if (SanityChecks)
                    {
                        var rh = reference.GetReadHandle<string>();
                        var retrieveResult = (await rh.Get()).ToRetrieveResult();
                        Assert.True(retrieveResult.IsSuccess());
                        Assert.True(retrieveResult.IsFound());
                        Assert.Equal(testData, rh.Value);
                    }
                    #endregion
                }

                #endregion

                #region List (Primary assertion)

                var listHandle = parentReference.GetListingsHandle();

                await listHandle.Get().ConfigureAwait(false);

                listHandle.Value.Value.OrderBy(l => l.Name)
                    .WithDeepEqual(FileList.Select(f => new Listing<object>(f)).OrderBy(l => l.Name))
                    .Assert();

                //          Assert.True(
                //listHandle.Value.Value.OrderBy(l => l.Name).IsDeepEqual(
                //FileList2.Select(f => new Listing<object>(f)).OrderBy(l => l.Name)
                //    )
                //);
                //Assert.Equal(
                //, 
                //DeepEqual.DefaultComparison
                //EqualityComparer<Listing<object>>.Default
                //);

                #endregion

                #region Cleanup

                foreach (var filename in FileList)
                {
                    var reference = parentReference.GetChild(filename);

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
                }

                // FIXME: Delete directory

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

#if FUTURE // Not sure how to detect types yet.
        [Theory]
        [ClassData(typeof(UniversalPersistersGenerator))]
        public async void P_List_Typed(IPersisterTestInitializer initializer)
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
                IReference parentReference = initializer.GetReferenceForTestPath(testPath);

                var testData = "Test data: " + Guid.NewGuid().ToString();

                string[] TypedFiles = new string[]
                {
                    "Item1a",
                    "Item2a",
                    "Item1b",
                    "Item2b",
                    "Item1c",
                    "Item2c",
                };

        #region Create

                foreach (var filename in FileList)
                {
                    var reference = parentReference.GetChild(filename);
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
                        var result = await rwh.Put();
                        Assert.True(result.IsSuccess);
                    }
        #endregion

        #region Exists

        #endregion

        #region Retrieve // REDUNDANT
                    if (SanityChecks)
                    {
                        var rh = reference.GetReadHandle<string>();
                        var retrieveResult = (await rh.Get()).ToRetrieveResult();
                        Assert.True(retrieveResult.IsSuccess());
                        Assert.True(retrieveResult.IsFound());
                        Assert.Equal(testData, rh.Value);
                    }
        #endregion
                }

                foreach (var filename in TypedFiles)
                {
                    var reference = parentReference.GetChild(filename);
        #region !Exists
                    {
                        var rh = reference.GetReadHandle<string>();
                        Assert.False(await rh.Exists(), "Unexpected: exists before creation.");
                    }
        #endregion

        #region Create

                    if (filename.Contains('1'))
                    {
                        var rwh = reference.GetReadWriteHandle<TestClass1>();
                        rwh.Value = TestClass1.Create;
                        var result = await rwh.Put();
                        Assert.True(result.IsSuccess);
                    }
                    else
                    {
                        var rwh = reference.GetReadWriteHandle<TestClass2>();
                        rwh.Value = TestClass2.Create;
                        var result = await rwh.Put();
                        Assert.True(result.IsSuccess);
                    }
        #endregion

        #region Exists

        #endregion

        #region Retrieve // REDUNDANT
                    if (SanityChecks)
                    {
                        if (filename.Contains('1'))
                        {
                            var rh = reference.GetReadHandle<TestClass1>();
                            var retrieveResult = (await rh.Get()).ToRetrieveResult();
                            Assert.True(retrieveResult.IsSuccess());
                            Assert.True(retrieveResult.IsFound());
                            TestClass1.Create.WithDeepEqual(rh.Value).Assert();
                        }
                        else
                        {
                            var rh = reference.GetReadHandle<TestClass2>();
                            var retrieveResult = (await rh.Get()).ToRetrieveResult();
                            Assert.True(retrieveResult.IsSuccess());
                            Assert.True(retrieveResult.IsFound());
                            TestClass2.Create.WithDeepEqual(rh.Value).Assert();
                        }
                    }
        #endregion
                }

        #endregion

        #region List (Primary assertion)

        #region All
                {
                    var listHandle = parentReference.GetListHandle();
                    await listHandle.Get().ConfigureAwait(false);
                    listHandle.Value.Value.OrderBy(l => l.Name)
                        .WithDeepEqual(FileList.Concat(TypedFiles).Select(f => new Listing<object>(f)).OrderBy(l => l.Name))
                        .Assert();
                }
        #endregion

        #region string
                {
                    var listHandle = parentReference.GetListHandle<string>();
                    await listHandle.Get().ConfigureAwait(false);
                    listHandle.Value.Value.OrderBy(l => l.Name)
                        .WithDeepEqual(FileList.Select(f => new Listing<string>(f)).OrderBy(l => l.Name))
                        .Assert();
                }
        #endregion

        #region TestClass1
                {
                    var listHandle = parentReference.GetListHandle<TestClass1>();
                    await listHandle.Get().ConfigureAwait(false);
                    listHandle.Value.Value.OrderBy(l => l.Name)
                        .WithDeepEqual(TypedFiles.Where(f => f.Contains('1')).Select(f => new Listing<TestClass1>(f)).OrderBy(l => l.Name))
                        .Assert();
                }
        #endregion

        #region TestClass2
                {
                    var listHandle = parentReference.GetListHandle<TestClass2>();
                    await listHandle.Get().ConfigureAwait(false);
                    listHandle.Value.Value.OrderBy(l => l.Name)
                        .WithDeepEqual(TypedFiles.Where(f => f.Contains('2')).Select(f => new Listing<TestClass2>(f)).OrderBy(l => l.Name))
                        .Assert();
                }
        #endregion

                //          Assert.True(
                //listHandle.Value.Value.OrderBy(l => l.Name).IsDeepEqual(
                //FileList2.Select(f => new Listing<object>(f)).OrderBy(l => l.Name)
                //    )
                //);
                //Assert.Equal(
                //, 
                //DeepEqual.DefaultComparison
                //EqualityComparer<Listing<object>>.Default
                //);

        #endregion

        #region Cleanup

                foreach (var filename in FileList)
                {
                    var reference = parentReference.GetChild(filename);

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
                }

                // FIXME: Delete directory

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
#endif
    }
}