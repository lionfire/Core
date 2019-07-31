using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Applications.Hosting;
using LionFire.Hosting;
using LionFire.ObjectBus;
using LionFire.ObjectBus.Redis;
using Xunit;

namespace Redis_
{
    public class CRDE
    {
        [Fact]
        public async void Pass_String()
        {
            const string testData = "testData";

            await FrameworkHost.Create()
                .AddObjectBus<RedisOBus>()
                .RunNowAndWait(async () =>
                {
                    var path = @"\temp\tests\" + GetType().FullName + @"\" + nameof(Pass_String) + @"\TestFile";

                    var r = new RedisReference(path);

                    {
                        var h = r.GetHandle<string>();

                        Assert.False(await h.Exists(), "test object already exists: " + path);

                        h.Object = testData;

                        await h.Commit();

                        Assert.True(await h.Exists(), "test object does not exist after saving: " + path);
                    }

                    {
                        var h = r.GetHandle<string>();
                        Assert.True(await h.Exists(), "test object does not exist after saving: " + path);

                        var retrievedData = h.Object;
                        Assert.Equal(testData, retrievedData);

                        h.MarkDeleted(); // TODO: make one
                        await h.Commit();
                        Assert.False(await h.Exists(), "test object still exists after deleting: " + path);
                    }
                    {
                        var h = r.GetHandle<string>();
                        Assert.False(await h.Exists(), "test object still exists after deleting: " + path);

                    }
                });
        }

        [Fact]
        public async void Pass_NonGenericHandle()
        {
            const string testData = "testData";

            await FrameworkHost.Create()
                .AddObjectBus<RedisOBus>()
                .RunAndExit(async () =>
                {
                    var path = @"\temp\tests\" + GetType().FullName + @"\" + nameof(Pass_NonGenericHandle) + @"\TestFile";

                    var r = new RedisReference(path);

                    {
                        var h = r.GetHandle();

                        Assert.False(await h.Exists(), "test object already exists: " + path);

                        h.Object = testData;

                        await h.Commit();

                        Assert.True(await h.Exists(), "test object does not exist after saving: " + path);
                    }

                    {
                        var h = r.GetHandle();
                        Assert.True(await h.Exists(), "test object does not exist after saving: " + path);

                        var retrievedData = h.Object;
                        Assert.Equal(testData, retrievedData);

                        h.MarkDeleted(); // TODO: make one
                        await h.Commit();
                        Assert.False(await h.Exists(), "test object still exists after deleting: " + path);
                    }
                    {
                        var h = r.GetHandle();
                        Assert.False(await h.Exists(), "test object still exists after deleting: " + path);

                    }
                });
        }
    }
}
