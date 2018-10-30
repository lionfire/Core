using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using LionFire.ObjectBus.Filesystem;
using LionFire.ObjectBus;
using LionFire.Referencing;
using Xunit;

namespace string_
{
    public class _to_LocalFileReference
    {
        [Fact]
        public async void Pass()
        {
            await new AppHost()
                .AddObjectBus()
                .AddFilesystemObjectBus()
                .RunNowAndWait(() =>
                {
                    var str = @"file:///c:\test\string\reference.txt#1234?zxcv(asdf)";

                    var reference = str.ToReference();

                    Assert.IsAssignableFrom<IReference>(reference);
                    Assert.IsType<LocalFileReference>(reference);

                    Assert.Equal("file", reference.Scheme);
                    Assert.Equal(@"c:/test/string/reference.txt#1234?zxcv(asdf)", reference.Path);
                    Assert.IsType<FsOBus>(reference.GetOBus());
                    Assert.IsType<FsOBase>(reference.GetOBase());
                });

        }
    }
}
