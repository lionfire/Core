using System;
using System.IO;
using LionFire.Applications.Hosting;
using LionFire.ObjectBus.Filesystem.Tests;
using LionFire.Persistence.Tests;
using LionFire.Referencing;
using Xunit;
using Xunit.Abstractions;
using static LionFire.Persistence.Tests.PersistenceTestUtils;

namespace LionFire.Persistence.Tests
{
    public class PersistenceTestUtils
    {
        public const string TestClass1Json = @"{""$type"":""LionFire.ObjectBus.Filesystem.Tests.TestClass1, LionFire.ObjectBus.Filesystem.Tests"",""StringProp"":""string1"",""IntProp"":1,""Object"":{""StringProp2"":""string2"",""IntProp2"":2}}";

        public static void AssertEqual(TestClass1 obj, object deserialized)
        {
            Assert.Equal(typeof(TestClass1), deserialized.GetType());
            var obj2 = (TestClass1)deserialized;
            Assert.Equal(obj.StringProp, obj2.StringProp);
            Assert.Equal(obj.IntProp, obj2.IntProp);
            Assert.Equal(obj.Object.StringProp2, obj2.Object.StringProp2);
            Assert.Equal(obj.Object.IntProp2, obj2.Object.IntProp2);
        }
    }
}

namespace LionFire.ObjectBus.Filesystem.Tests
{
    public class FileReferenceHandleProviderTests
    {
        //[Fact]
        //public async void DeserializeFromFileUriToFileHandleWithoutExtension()
        //{
        //    await new AppHost()
        //        .AddSerialization()
        //        .AddNewtonsoftJson()
        //        .AddObjectBus()
        //        .AddFilesystemObjectBus()
        //        .RunNowAndWait(() =>
        //        {
        //            var json = TestClass1Json;
        //            var pathWithoutExtension = Guid.NewGuid().ToString();

        //            #region Write File to disk manually

        //            var path = pathWithoutExtension + ".json";
        //            Assert.False(File.Exists(path));
        //            File.WriteAllText(path, json);

        //            #endregion

        //            var reference = ("file:" + pathWithoutExtension).ToReference();

        //            // TODO
        //        };
        //}


        [Fact]
        public async void LocalFileReference_WithoutExtension_ToHandle_MissingProvider()
        {
            await new AppHost()
                .AddSerialization()
                .AddNewtonsoftJson()
                .AddObjectBus()
                //.AddFilesystemObjectBus() // Adds HandleProvider for file:
                .AddHandleProvider()
                .RunNowAndWait(() =>
                {
                    var json = TestClass1Json;
                    var pathWithoutExtension = Guid.NewGuid().ToString();
                    var path = pathWithoutExtension + ".json";

                    try
                    {
                        #region Write File to disk manually

                        Assert.False(File.Exists(path));
                        File.WriteAllText(path, json);

                        #endregion

                        var reference = new LocalFileReference(pathWithoutExtension);

                        Assert.Equal("file://" + pathWithoutExtension, reference.Key);

                        Exception e = null;
                        try
                        {
                            var handle = reference.ToHandle<TestClass1>();
                        }
                        catch (Exception ex)
                        {
                            e = ex;
                        }
                        Assert.NotEmpty(e);
                        Assert.Equal($"Failed to provide handle for reference with scheme {reference.Scheme}.  Have you registered the relevant IHandleProvider service?", e.Message);
                    }
                    finally
                    {
                        #region Cleanup

                        File.Delete(path);
                        Assert.False(File.Exists(path));

                        #endregion
                    }
                };
        }


        [Fact]
        public async void LocalFileReference_WithoutExtension_ToHandle_ToObject()
        {
            await new AppHost()
                .AddSerialization()
                .AddNewtonsoftJson()
                .AddObjectBus()
                .AddFilesystemObjectBus() // Adds HandleProvider for file:
                .AddHandleProvider()
                .RunNowAndWait(() =>
                {

                    var json = TestClass1Json;
                    var pathWithoutExtension = Path.GetTempFileName();

                    #region Write File to disk manually

                    var path = pathWithoutExtension + ".json";
                    Assert.False(File.Exists(path));
                    File.WriteAllText(path, json);

                    #endregion

                    var reference = new LocalFileReference(pathWithoutExtension);

                    Assert.Equal("file://" + pathWithoutExtension, reference.Key);

                    var handle = reference.ToHandle<TestClass1>();

                    PersistenceTestUtils.AssertEqual(TestClass1.Create, handle.Object);

                    var obj = handle.Object;

                    #region Cleanup

                    File.Delete(path);
                    Assert.False(File.Exists(path));

                    #endregion
                };
        }
    }

    public class FsOBusTests
    {
        private readonly ITestOutputHelper output;

        public FsOBusTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        //[Fact]
        //public async void FileUriToHandle()
        //{
        //    await new AppHost()
        //        .AddSerialization()
        //        .AddNewtonsoftJson()
        //        .AddObjectBus()
        //        .AddFilesystemObjectBus()
        //        .RunNowAndWait(() =>
        //        {
        //            var path = @"c:\temp\tests\" + typeof(FsOBusTests).FullName + @"\" + nameof(FsRoundTripJson) + @"\TestFile";
        //            var uriString = "file:" + path;

        //            #region Same as FsRoundTripJson

        //            var pathWithExtension = path + ".json";
        //            var reference = new LocalFileReference(path);

        //            var obj = TestClass1.Create;
        //            {
        //                OBus.Set(reference, obj);
        //                Assert.True(File.Exists(pathWithExtension));
        //            }

        //            var textFromFile = File.ReadAllText(pathWithExtension);
        //            var expectedJson = TestClass1Json;
        //            Assert.Equal(expectedJson, textFromFile);

        //            {
        //                var deserialized = OBus.Get(reference);
        //                Assert.Equal(typeof(TestClass1), deserialized.GetType());
        //                TestClass1 obj2 = (TestClass1)deserialized;
        //                Assert.Equal(obj.StringProp, obj2.StringProp);
        //                Assert.Equal(obj.IntProp, obj2.IntProp);
        //                Assert.Equal(obj.Object.StringProp2, obj2.Object.StringProp2);
        //                Assert.Equal(obj.Object.IntProp2, obj2.Object.IntProp2);
        //            }

        //            #endregion

        //            var handleFromUriString = uriString.ToHandle<TestClass1>();

        //            #region Cleanup

        //            OBus.Delete(reference);
        //            Assert.False(File.Exists(pathWithExtension));

        //            #endregion
        //        });
        //}

        [Fact]
        public async void FsRoundTripJson()
        {
            await new AppHost()
                .AddSerialization()
                .AddNewtonsoftJson()
                .AddObjectBus()
                .AddFilesystemObjectBus()
                .RunNowAndWait(() =>
                {
                    var path = @"c:\temp\tests\" + typeof(FsOBusTests).FullName + @"\" + nameof(FsRoundTripJson) + @"\TestFile";
                    var pathWithExtension = path + ".json";
                    var reference = new LocalFileReference(path);

                    var obj = TestClass1.Create;
                    {
                        OBus.Set(reference, obj);
                        Assert.True(File.Exists(pathWithExtension));
                    }

                    var textFromFile = File.ReadAllText(pathWithExtension);
                    var expectedJson = TestClass1Json;
                    Assert.Equal(expectedJson, textFromFile);

                    {
                        var deserialized = OBus.Get(reference);
                        Assert.Equal(typeof(TestClass1), deserialized.GetType());
                        TestClass1 obj2 = (TestClass1)deserialized;
                        Assert.Equal(obj.StringProp, obj2.StringProp);
                        Assert.Equal(obj.IntProp, obj2.IntProp);
                        Assert.Equal(obj.Object.StringProp2, obj2.Object.StringProp2);
                        Assert.Equal(obj.Object.IntProp2, obj2.Object.IntProp2);
                    }

                    OBus.Delete(reference);
                    Assert.False(File.Exists(pathWithExtension));
                });
        }
    }
}
