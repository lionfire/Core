using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using LionFire.ObjectBus;
using LionFire.ObjectBus.Filesystem;
using LionFire.ObjectBus.Filesystem.Tests;
using LionFire.Referencing;
using LionFire.Structures;
using Xunit;

namespace LocalFileReference_
{
    public class _GetHandle
    {
        [Fact]
        public async void Pass()
        {
            await new AppHost()
                .AddSerialization()
                .AddNewtonsoftJson()
                .AddObjectBus()
                .AddFilesystemObjectBus() // Adds HandleProvider for LocalFileReference
                .RunNowAndWait(() =>
                {
                    Assert.Null(ManualSingleton<FsOBus>.Instance); // Null until used (?) Not essential

                    var pathWithoutExtension = Guid.NewGuid().ToString();
                    var reference = new LocalFileReference(pathWithoutExtension);
                    H<TestClass1> h;
                    h = reference.GetHandle<TestClass1>(); // --------------- GetHandle

                    Assert.NotNull(ManualSingleton<FsOBus>.Instance);

                    Assert.Same(reference, h.Reference);
                    Assert.IsAssignableFrom<R<TestClass1>>(h);
                    Assert.IsAssignableFrom<H<TestClass1>>(h);
                    Assert.IsType<OBusHandle<TestClass1>>(h);

                    var obh = (OBusHandle<TestClass1>)h;
                    Assert.Same(FsOBase.Instance, obh.OBase);
                    Assert.Same(ManualSingleton<FsOBus>.Instance, obh.OBus);
                });
        }

        //[Fact]
        //public async void F_MissingFsOBus()
        //{
        //    await new AppHost()
        //        .AddSerialization()
        //        .AddNewtonsoftJson()
        //        .AddObjectBus()
        //        // MISSING .AddFilesystemObjectBus() // Adds HandleProvider for file:
        //        .RunNowAndWait(async () =>
        //        {
        //            var pathWithoutExtension = Guid.NewGuid().ToString();
        //            var reference = new LocalFileReference(pathWithoutExtension);
        //            H<TestClass1> h;
        //            await Assert.ThrowsAsync<Exception>(() => Task.FromResult(h = reference.GetHandle<TestClass1>()));
        //            //Assert.Equal($"Failed to provide handle for reference with scheme {reference.Scheme}.  Have you registered the relevant IHandleProvider service?", e.Message);
        //        });
        //}

        //[Fact]
        //public async void _WithoutExtension_ToHandle()
        //{
        //    await new AppHost()
        //        .AddSerialization()
        //        .AddNewtonsoftJson()
        //        .AddObjectBus()
        //        .AddFilesystemObjectBus() // Adds HandleProvider for file:
        //        .RunNowAndWait(() =>
        //        {

        //            var json = TestClass1Json;
        //            var pathWithoutExtension = Path.GetTempFileName();

        //            #region Write File to disk manually

        //            var path = pathWithoutExtension + ".json";
        //            Assert.False(File.Exists(path));
        //            File.WriteAllText(path, json);

        //            #endregion

        //            var reference = new LocalFileReference(pathWithoutExtension);

        //            Assert.Equal("file://" + pathWithoutExtension, reference.Key);

        //            var handle = reference.GetHandle<TestClass1>();

        //            PersistenceTestUtils.AssertEqual(TestClass1.Create, handle.Object);

        //            var obj = handle.Object;

        //            #region Cleanup

        //            File.Delete(path);
        //            Assert.False(File.Exists(path));

        //            #endregion
        //        });
        //}

        //[Fact]
        //public async void _ToObject()
        //{
        //    await new AppHost()
        //        .AddSerialization()
        //        .AddNewtonsoftJson()
        //        .AddObjectBus()
        //        .AddFilesystemObjectBus() // Adds HandleProvider for file:
        //        .RunNowAndWait(() =>
        //        {

        //            var json = TestClass1Json;
        //            var pathWithoutExtension = Path.GetTempFileName();

        //            #region Write File to disk manually

        //            var path = pathWithoutExtension + ".json";
        //            Assert.False(File.Exists(path));
        //            File.WriteAllText(path, json);

        //            #endregion

        //            var reference = new LocalFileReference(pathWithoutExtension);

        //            Assert.Equal("file://" + pathWithoutExtension, reference.Key);

        //            var handle = reference.GetHandle<TestClass1>();

        //            PersistenceTestUtils.AssertEqual(TestClass1.Create, handle.Object);

        //            var obj = handle.Object;

        //            #region Cleanup

        //            File.Delete(path);
        //            Assert.False(File.Exists(path));

        //            #endregion
        //        });
        //}


    }
}
