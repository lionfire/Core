using LionFire.Hosting;
using LionFire.Vos.VosApp;
using System;
using Xunit;

namespace VosApp_

{
    public class CreateHost_
    {
        [Fact]
        public async void Pass()
        {
            await VosAppHostBuilder.Create(options: new VosAppOptions
            {

            })
                .ConfigureServices((context, services) =>
                {

                })
                .RunAsync(() =>
                {
                    Assert.True(true);
                });
        }
    }
}
