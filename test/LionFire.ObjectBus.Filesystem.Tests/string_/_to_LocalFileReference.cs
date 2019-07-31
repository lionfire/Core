using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using LionFire.ObjectBus.Filesystem;
using LionFire.ObjectBus;
using LionFire.Referencing;
using Xunit;
using LocalFileReference_;
using LionFire.Hosting;
using Microsoft.Extensions.DependencyInjection;
using LionFire;

namespace string_
{
    //public static class LionFireFrameworkInitializer
    //{
    //    public static IAppHost AddDefaults(this IAppHost app)
    //    {
    //        new AppHost()
    //            .GenericHost
    //    }
    //}

    public class _to_LocalFileReference
    {
        [Fact]
        public async void Pass()
        {
            await FrameworkHost.Create()
                .AddObjectBus<FsOBus>()
                .Run(() =>
                {
                    var str = @"file:///c:\test\string\reference.txt#1234?zxcv(asdf)";
                    var reference = str.ToReference();

                    Assert.IsAssignableFrom<IReference>(reference);
                    Assert.IsType<LocalFileReference>(reference);

                    Assert.Equal("file", reference.Scheme);
                    Assert.Equal(@"c:/test/string/reference.txt#1234?zxcv(asdf)", reference.Path);
                    //Assert.IsType<FsOBus>(reference.GetOBus());
                    //Assert.IsType<FsOBase>(reference.GetOBase());
                });
        }

        [Fact]
        public async Task Fail_No_Scheme_Throws()
        {
            await FrameworkHost.Create()
                .AddObjectBus<FsOBus>()
                .Run(() =>
                {
                    var path = @"c:\Temp\Path\Test\" + Guid.NewGuid().ToString();

                    Assert.Throws<NotFoundException>(() => path.ToReference());
                });
        }

        [Fact]
        public async Task Fail_No_Scheme_Null()
        {
            await FrameworkHost.Create()
            .AddObjectBus<FsOBus>()
            .Run(() =>
            {
                var path = @"c:\Temp\Path\Test\" + Guid.NewGuid().ToString();

                Assert.Null(path.TryToReference());
            });
        }

        //public class _from_ToReference
        //{

        //    [Fact]
        //    public void Pass()
        //    {
        //        var pathWithoutExtension = @"c:\Temp\Path\Test\" + Guid.NewGuid().ToString();

        //        var reference = ("file:///" + pathWithoutExtension).ToReference();

        //        Assert.IsType<LocalFileReference>(reference);

        //        Assert.Equal("file:///" + pathWithoutExtension.Replace('\\', '/'), reference.Key);
        //    }
        //}

    }
}
