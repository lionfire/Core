using System;
using LionFire.Applications.Hosting;
#if TODO
using LionFire.ObjectBus;
using Xunit;
using LionFire.Hosting;
using LionFire.Vos;
using LionFire.Services;

namespace LionFire.Vos.Tests
{
    public class _VosReference
    {
        [Fact]
        public async void String_to_Reference()
        {
            await FrameworkHostBuilder.Create()
                .AddVosApp()
                .Run(() =>
                {
                    var str = "vos:/path/to/vob";
                    var reference = str.GetReference();

                    Assert.IsType<VosReference>(reference);

                });

        }
    }
}
#endif