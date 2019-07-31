using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using Xunit;
using LionFire.Hosting;
using LionFire.Hosting.ExtensionMethods;

namespace Mount_
{
    public class _SimpleMount
    {
        [Fact]
        public async void Pass()
        {
            await FrameworkHost.Create()
                .AddVos()
                .Run(async () =>
                {
                    await Task.Delay(1);
                });
        }
    }
}
