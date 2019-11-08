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
            await FrameworkHostBuilder.Create()
                .AddObjectBus<FSOBus>()
                .Run(() =>
                {
                    Assert.NotNull(ManualSingleton<FSOBus>.Instance); // Created in AddObjectBus<FSOBus>()

                    var pathWithoutExtension = Guid.NewGuid().ToString();
                    var reference = new FileReference(pathWithoutExtension);
                    IReadWriteHandleBase<TestClass1> h;
                    h = reference.ToHandle<TestClass1>();

                    Assert.NotNull(ManualSingleton<FSOBus>.Instance);

                    Assert.Same(reference, h.Reference);
                    Assert.IsAssignableFrom<IReadHandleBase<TestClass1>>(h);
                    Assert.IsAssignableFrom<IReadWriteHandleBase<TestClass1>>(h);
                    Assert.IsType<OBaseHandle<TestClass1>>(h);

                    var obh = (OBaseHandle<TestClass1>)h;
                    Assert.Same(FSOBase.Instance, obh.OBase);
                    Assert.Same(ManualSingleton<FSOBus>.Instance, obh.OBase.OBus);
                });
        }

        [Fact]
        public async Task F_MissingFsOBus()
        {
            await FrameworkHostBuilder.Create()
                //.AddObjectBus<FSOBus>() // MISSING
                .Run(() =>
                {
                        //Assert.Null(ManualSingleton<FSOBus>.Instance); // Null until used - TODO: don't use singletons

                        var pathWithoutExtension = Guid.NewGuid().ToString();
                        var reference = new FileReference(pathWithoutExtension);
                        IReadWriteHandleBase<TestClass1> h;
                        Assert.Throws<HasUnresolvedDependenciesException>(() => h = reference.ToHandle<TestClass1>());
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

        //            var handle = reference.ToHandle<TestClass1>();

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

        //            var handle = reference.ToHandle<TestClass1>();

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
