using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Applications.Hosting;
using LionFire.Hosting;
using LionFire.ObjectBus;
using LionFire.ObjectBus.Redis;
using LionFire.ObjectBus.RedisPub;
using LionFire.Persistence;
using LionFire.Referencing;
using Xunit;
using LionFire.Persistence.Handles;

namespace RedisOBoc_
{
    public class Sync
    {
        [Fact]
        public async void Pass()
        {
            const string testChildVal = "testChildVal";

            await FrameworkHostBuilder.Create()
                .AddObjectBus<RedisOBus>()
                .AddObjectBus<RedisPubOBus>()
                .Run(async () =>
                {
                    var dir = @"\temp\tests\" + this.GetType().FullName + @"\" + nameof(Pass) + @"\";
                    var rDir = new RedisReference(dir);
                    
                    {
                        var r = new RedisReference(dir + "testChild");
                        var h = r.ToHandle<string>();
                        Assert.False(await h.Exists(), "test object already exists: " + r.Path);
                        h.Object = testChildVal;
                        await h.Commit();
                        Assert.True(await h.Exists(), "test object does not exist after saving: " + r.Path);
                    }

                    {
                        var rc = rDir.GetReadCollectionHandle();
                        int changeCount = 0;

                        rc.Entries.CollectionChanged += e =>
                       {
                           changeCount++;
                       };
                    }

                    //var obj = TestClass1.Create;
                    //{
                    //    //await OBus.Set(reference, obj);
                    //    await rDir.Set(obj);  // ----------------------- Set
                    //    Assert.True(File.Exists(pathWithExtension));
                    //}

                    //var textFromFile = File.ReadAllText(pathWithExtension);
                    //var expectedJson = PersistenceTestUtils.TestClass1Json;
                    //Assert.Equal(expectedJson, textFromFile);

                    //{
                    //    var deserialized = await OBus.Get(rDir);  // ----------------------- Get
                    //    Assert.Equal(typeof(TestClass1), deserialized.GetType());
                    //    TestClass1 obj2 = (TestClass1)deserialized;
                    //    Assert.Equal(obj.StringProp, obj2.StringProp);
                    //    Assert.Equal(obj.IntProp, obj2.IntProp);
                    //    Assert.Equal(obj.Object.StringProp2, obj2.Object.StringProp2);
                    //    Assert.Equal(obj.Object.IntProp2, obj2.Object.IntProp2);
                    //}

                    //await OBus.Delete(rDir);   // ----------------------- Delete
                    //Assert.False(File.Exists(pathWithExtension));
                });
        }

        private void Entries_CollectionChanged(LionFire.Collections.INotifyCollectionChangedEventArgs<ICollectionEntry> e) => throw new NotImplementedException();
    }
}
