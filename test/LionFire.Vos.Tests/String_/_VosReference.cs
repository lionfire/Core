using System;
using LionFire.Applications.Hosting;
using LionFire.ObjectBus;
using Xunit;

namespace LionFire.Valor.Tests
{
    public class _VosReference
    {
        [Fact]
        public async void String_to_Reference()
        {
            await new AppHost()
                .AddObjectBus()
                .AddVos()
                .RunNowAndWait(() =>
                {
                    var str = "vos:/path/to/vob";
                    var reference = str.ToReference();

                    Assert.IsType<VosReference>

                });

        }
    }
}
