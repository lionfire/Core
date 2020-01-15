using LionFire.Hosting;
using LionFire.Vos.VosApp;
using System;
using Xunit;

namespace VosApp_

{
    public class CreateHost_
    {
        [Fact]
        public void Pass()
        {

            VosAppHost.Create(options: new VosAppOptions
            {

            })
                .ConfigureServices((context, services) =>
                {

                })
                .Run(() =>
                {
                    Assert.True(true);
                });
        }
    }
}
