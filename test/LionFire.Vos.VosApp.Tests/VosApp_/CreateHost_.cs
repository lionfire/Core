using LionFire.Hosting;
using LionFire.Vos.VosApp;
using Microsoft.Extensions.Hosting;
using System;
using Xunit;

namespace VosApp_

{
    public class CreateHost_
    {
        [Fact]
        public async void Pass()
        {
            await Host.CreateDefaultBuilder().LionFire(b => b.VosApp())
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
