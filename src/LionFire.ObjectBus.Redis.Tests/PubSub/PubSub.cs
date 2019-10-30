using System;
using LionFire.Applications.Hosting;
using LionFire.Hosting;
using LionFire.ObjectBus;
using LionFire.ObjectBus.RedisPub;
using LionFire.Persistence;
using LionFire.Referencing;
using Xunit;

namespace RedisPub_
{
    public class PubSub
    {
        private string ReceivedMessage = null;
        private const string TestMessage = "this is a test message";

        [Fact]
        public async void Pass()
        {
            var url = "redispub:test";

            await FrameworkHostBuilder.Create()
            .AddObjectBus<RedisPubOBus>()
            .Run(async () =>
            {
                {
                    var refRead = new RedisPubReference(url);
                    var rRead = refRead.ToReadHandle<object>();
                    rRead.ObjectChanged += H_ObjectChanged;
                }
                {
                    var refWrite = new RedisPubReference(url);
                    var hWrite = refWrite.ToHandle<object>();
                    hWrite.Value = "testMessage";
                    await hWrite.Put();
                    Assert.Equal(TestMessage, ReceivedMessage);
                }
            });

        }

        private void H_ObjectChanged(RH<object> obj)
        {
            Assert.NotNull(obj);
            Assert.NotNull(obj.Value);
            Assert.IsType<string>(obj.Value);
            ReceivedMessage = obj.Value.ToString();
        }
    }
}
