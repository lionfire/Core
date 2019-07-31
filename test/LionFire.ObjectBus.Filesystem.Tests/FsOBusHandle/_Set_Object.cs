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
    public class _Set_Object
    {
#if TODO
        /// <summary>
        /// TODO: Set an option to automatically set the extension. 
        /// </summary>
        [Fact]
        public async void Pass_AutoSetExtension() => await _Pass(withExtension: false);

        /// <summary>
        /// TODO:
        /// </summary>
        [Fact]
        public async void Pass_AutoDetectExtension_Json() => await _Pass(withExtension: false);

        /// <summary>
        /// TODO:
        /// </summary>
        [Fact]
        public async void Pass_AutoDetectExtension_XmlOrSomethingElse() => await _Pass(withExtension: false);

#endif

        /// <summary>
        /// TODO: 
        /// </summary>
        [Fact]
        public async void Pass_WithoutExtension() => await _Pass(withExtension: false);
        [Fact]
        public async void Pass_WithExtension() => await _Pass(withExtension: true);

        private async Task _Pass(bool withExtension)
        {
            await FrameworkHost.Create(
                //serializers: s => s.AddJson()
                )
            //await FrameworkHost.Create()
                    .AddObjectBus<FsOBus>()
                    .Run(async () =>
                    {
                        var pathWithoutExtension = FsTestUtils.TestFile;
                        var path = pathWithoutExtension + ".json";

                        var savePath = withExtension ? path : pathWithoutExtension;

                        var reference = new LocalFileReference(savePath); // -------- With / WithoutExtension

                        var h = reference.GetHandle<TestClass1>();
                        h.Object = TestClass1.Create;

                        await h.Commit().ConfigureAwait(false); // --------- Save

                        Assert.True(File.Exists(savePath), "Missing file: " + path);
                        var json = File.ReadAllText(savePath);
                        Assert.Equal(PersistenceTestUtils.TestClass1Json, json);

                        FsTestUtils.CleanPath(savePath);
                    });
        }
    }
}