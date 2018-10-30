using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using Xunit;

namespace Mount_
{
    public class _SimpleMount
    {
        [Fact]
        public async void Pass()
        {
            await new AppHost()
                .AddVos()
                .RunNowAndWait(async () =>
                {

                    await Task.Delay(1);

                });
        }
    }
}
