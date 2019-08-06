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
using LionFire.ObjectBus.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using LionFire.DependencyInjection;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using LionFire.Hosting;
using LionFire.Persistence;

namespace LocalFileReference_
{

    public class _GetHandle
    {
        [Fact]
        public async Task Pass() // No persistence
        {
            await FrameworkHost.Create()
                .AddObjectBus<FsOBus>()
                .Run(() =>
                {
                    Assert.NotNull(ManualSingleton<FsOBus>.Instance); // Created in AddObjectBus<FsOBus>()

                    var pathWithoutExtension = Guid.NewGuid().ToString();
                    var reference = new FileReference(pathWithoutExtension);
                    H<TestClass1> h;
                    h = reference.GetHandle<TestClass1>();

                    Assert.NotNull(ManualSingleton<FsOBus>.Instance);

                    Assert.Same(reference, h.Reference);
                    Assert.IsAssignableFrom<RH<TestClass1>>(h);
                    Assert.IsAssignableFrom<H<TestClass1>>(h);
                    Assert.IsType<OBaseHandle<TestClass1>>(h);

                    var obh = (OBaseHandle<TestClass1>)h;
                    Assert.Same(FsOBase.Instance, obh.OBase);
                    Assert.Same(ManualSingleton<FsOBus>.Instance, obh.OBase.OBus);
                });
        }

        [Fact]
        public async Task F_MissingFsOBus()
        {
            await FrameworkHost.Create()
                //.AddObjectBus<FsOBus>() // MISSING
                .Run(() =>
                {
                        Assert.Null(ManualSingleton<FsOBus>.Instance); // Null until used

                        var pathWithoutExtension = Guid.NewGuid().ToString();
                        var reference = new FileReference(pathWithoutExtension);
                        H<TestClass1> h;
                        Assert.Throws<HasUnresolvedDependenciesException>(() => h = reference.GetHandle<TestClass1>());
                });
        }

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
