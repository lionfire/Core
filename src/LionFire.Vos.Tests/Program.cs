using LionFire.Applications.Hosting;
using System;
using Xunit;

namespace LionFire.Vos.Tests
{
    public class VosTests
    {
        IAppHost app;
        public VosTests()
        {
            app = new AppHost()
                .UseFsBus()
                .UseVos()
                .Initialize()
                // TODO: Mount some test mount points
                ;
        }
        [Fact]
        public void TestTest()
        {
            Assert.Equal(1, 2 - 1);
        }
    }
}
