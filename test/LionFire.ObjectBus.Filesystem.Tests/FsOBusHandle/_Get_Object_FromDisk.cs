using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using LionFire.Hosting;
using LionFire.Hosting.ExtensionMethods;
using LionFire.ObjectBus;
using LionFire.ObjectBus.Filesystem;
using LionFire.ObjectBus.Filesystem.Tests;
using LionFire.ObjectBus.Testing;
using LionFire.Referencing;
using Xunit;

namespace Handle
{
    public class _Get_Object_FromDisk
    {
        [Fact]
        public async void Pass_WithoutExtension() => await _Pass(withExtension: false);
        [Fact]
        public async void Pass_WithExtension() => await _Pass(withExtension: true);

        private async Task _Pass(bool withExtension)
        {
            await FrameworkHost.Create(
                //serializers: s => s.AddJson()
                )
                    .AddObjectBus<FsOBus>()
                    .Run(() =>
                    {
                        var pathWithoutExtension = FsTestUtils.TestFile;
                        var path = pathWithoutExtension + ".json";

                        File.WriteAllText(path, PersistenceTestUtils.TestClass1Json);

                        var reference = new LocalFileReference(withExtension ? path : pathWithoutExtension); // -------- With / WithoutExtension

                        var h = reference.GetHandle<TestClass1>();

                        var obj = h.Object; // --------------- Object

                        Assert.NotNull(obj);
                        Assert.IsType<TestClass1>(obj);
                        FsTestUtils.AssertEqual(TestClass1.Create, obj);

                        FsTestUtils.CleanPath(path);
                    });
        }
    }
}