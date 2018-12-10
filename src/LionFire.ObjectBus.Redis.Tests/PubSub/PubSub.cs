using System;
using LionFire.Applications.Hosting;
using LionFire.ObjectBus;
using LionFire.ObjectBus.RedisPub;
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

            await new AppHost()
            .AddObjectBus()
            .AddRedisPubObjectBus()
            .RunNowAndWait(async () =>
            {
                {
                    var refRead = new RedisPubReference(url);
                    var rRead = refRead.GetReadHandle<object>();
                    rRead.ObjectChanged += H_ObjectChanged;
                }
                {
                    var refWrite = new RedisPubReference(url);
                    var hWrite = refWrite.GetHandle<object>();
                    hWrite.Object = "testMessage";
                    await hWrite.Commit();
                    Assert.Equal(TestMessage, ReceivedMessage);
                }
            });

        }

        private void H_ObjectChanged(LionFire.Referencing.RH<object> obj)
        {
            Assert.NotNull(obj);
            Assert.NotNull(obj.Object);
            Assert.IsType<string>(obj.Object);
            ReceivedMessage = obj.Object.ToString();
        }
    }
}
