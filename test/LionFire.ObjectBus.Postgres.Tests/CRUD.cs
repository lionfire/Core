using LionFire.Applications.Hosting;
using LionFire.ObjectBus;
using LionFire.ObjectBus.Redis;
using LionFire.ObjectBus.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace LionFire.ObjectBus.Postgres.Tests
{
    public class CRUD_
    {
        [Fact]
        public async void Pass()
        {
            //if (isDevelopment)
            //{
            //    builder.AddUserSecrets<Program>();
            //}

            await new AppHost()
                .AddSerialization()
                .AddNewtonsoftJson()
                .AddObjectBus()
                .AddPostgresObjectBus()
                .RunNowAndWait(async () =>
                {
                    var path = @"\temp\tests\" + GetType().FullName + @"\" + nameof(Pass) + @"\TestFile";
                    var pathWithExtension = path + ".json";
                    var reference = new SqlReference(path);

                    var obj = TestClass1.Create;
                    {
                        //await OBus.Set(reference, obj);
                        await reference.Set(obj);  // ----------------------- Set
                        Assert.True(File.Exists(pathWithExtension));
                    }

                    var textFromFile = File.ReadAllText(pathWithExtension);
                    var expectedJson = PersistenceTestUtils.TestClass1Json;
                    Assert.Equal(expectedJson, textFromFile);

                    {
                        var deserialized = await OBus.Get(reference);  // ----------------------- Get
                        Assert.Equal(typeof(TestClass1), deserialized.GetType());
                        TestClass1 obj2 = (TestClass1)deserialized;
                        Assert.Equal(obj.StringProp, obj2.StringProp);
                        Assert.Equal(obj.IntProp, obj2.IntProp);
                        Assert.Equal(obj.Object.StringProp2, obj2.Object.StringProp2);
                        Assert.Equal(obj.Object.IntProp2, obj2.Object.IntProp2);
                    }

                    await OBus.Delete(reference);   // ----------------------- Delete
                    Assert.False(File.Exists(pathWithExtension));
                });
        }
    }
}
