using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LionFire.Applications.Hosting;
using LionFire.ObjectBus;
using LionFire.ObjectBus.Filesystem;
using LionFire.ObjectBus.Filesystem.Tests;
using LionFire.ObjectBus.Testing;
using Xunit;


namespace OBus_
{
    public class Set_Get_Delete_LFR
    {
        [Fact]
        public async void Pass()
        {
            await new AppHost()
                .AddSerialization()
                .AddNewtonsoftJson()
                .AddObjectBus()
                .AddFilesystemObjectBus()
                .RunNowAndWait(async () =>
                {
                    var path = @"c:\temp\tests\" + this.GetType().FullName + @"\" + nameof(Pass) + @"\TestFile";
                    var pathWithExtension = path + ".json";
                    var reference = new LocalFileReference(path);

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
