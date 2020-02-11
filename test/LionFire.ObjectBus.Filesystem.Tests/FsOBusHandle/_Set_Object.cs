using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using LionFire.Hosting;
using LionFire.Services;
using LionFire.ObjectBus;
using LionFire.ObjectBus.ExtensionlessFs;
using LionFire.ObjectBus.Filesystem;
using LionFire.ObjectBus.Filesystem.Tests;
using LionFire.ObjectBus.Testing;
using LionFire.Persistence;
using LionFire.Persistence.Filesystem.Tests;
using LionFire.Referencing;
using Xunit;
using LionFire;
using LionFire.Persistence.Filesystem;

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
            var host = FrameworkHostBuilder.Create(
                //serializers: s => s.AddJson()
                );

            if (!withExtension)
            {
                host.AddObjectBus<ExtensionlessFSOBus>();
            }

            await FilesystemTestHost.Create()
                .RunAsync(async () =>
                {
                    var extension = ".json";
                    var pathWithoutExtension = FsTestUtils.TestFile;
                    var path = pathWithoutExtension + extension;

                    var savePath = withExtension ? path : pathWithoutExtension;

                    IReference reference;
                    if (withExtension)
                    {
                        reference = new FileReference(savePath);
                    }
                    else
                    {
                        reference = new AutoExtensionFileReference(savePath);
                    }

                    var h = reference.ToHandle<TestClass1>();
                    h.Value = TestClass1.Create;

                    var commitResult = await h.Put().ConfigureAwait(false); // --------- Save
                    Assert.True(commitResult.IsSuccess());
                    try
                    {
                        //if (withExtension)
                        //{
                        //    Assert.True(path.EndsWith(extension), "Wrong file extension");
                        //}

                        Assert.True(File.Exists(path), "Missing file: " + path);
                        var json = File.ReadAllText(path);
                        Assert.Equal(PersistenceTestUtils.TestClass1Json, json);
                    }
                    finally
                    {
                        FsTestUtils.CleanPath(savePath);
                    }
                });
        }
    }
}