using System;
using System.IO;
using LionFire.Applications.Hosting;
using LionFire.Persistence.Tests;
using LionFire.Referencing;
using Xunit;
using Xunit.Abstractions;
using static LionFire.Persistence.Tests.PersistenceTestUtils;

namespace LionFire.ObjectBus.Filesystem.Tests
{
    public class Reference_FsOBaseProvider
    {

        //[Fact]
        //public async void Reference_to_FsOBase()
        //{
        //    await new AppHost()
        //        .AddSerialization()
        //        .AddNewtonsoftJson()
        //        .AddObjectBus()
        //        .AddFilesystemObjectBus() // Adds HandleProvider for file:
        //        .RunNowAndWait(() =>
        //        {
        //            var reference = new LocalFileReference(@"C:\temp\_boguspath.txt");
        //            reference.GetOBases();
        //        });
        //}
    }

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

        
    }
}
