using LionFire.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using Xunit;

namespace LionFire.Assets.Tests
{
#if false
    public class UnitTest1
    {
        public IHostBuilder CreateHostBuilder()
        {
            return FrameworkHostBuilder.CreateDefault();
        }

        [Fact]
        public void TestResource()
        {
            CreateHostBuilder().Run(() =>
            {
                Assert.False(true);
            });
        }
    }
#endif
}
