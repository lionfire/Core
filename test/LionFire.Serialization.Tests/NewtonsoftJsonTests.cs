using System;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using LionFire.Serialization.Json.Newtonsoft;
using Xunit;
using Xunit.Abstractions;

namespace LionFire.Serialization.Tests
{

    public class AppTest : IDisposable
    {
        public void Dispose() => AppHost.Reset();
    }

    public class MissingSerializationProviderAppResetTests : AppTest
    {
        [Fact]
        public async void MissingAddSerialization()
        {
            await _MissingAddSerialization(true);
            AppHost.Reset();
            await AppSerializeToFromJsonStringTest();
            AppHost.Reset();
            await _MissingAddSerialization(false);

            async Task _MissingAddSerialization(bool first)
            {
                await new AppHost()
                    .AddNewtonsoftJson()
                    .RunNowAndWait(() =>
                    {
                        var ser = Defaults.TryGet<ISerializationProvider>();
                        Assert.Null(ser);
                    });
            }
        }

        private async Task AppSerializeToFromJsonStringTest()
        {
            await new AppHost()
                .AddSerialization()
                .AddNewtonsoftJson()
                .RunNowAndWait(() =>
                {
                    var ser = Defaults.TryGet<ISerializationProvider>();
                    var obj = TestClass1.Create;

                    var json = ser.ToString(obj);
                    var expected = @"{""$type"":""LionFire.Serialization.Tests.TestClass1, LionFire.Serialization.Tests"",""StringProp"":""string1"",""IntProp"":1,""Object"":{""StringProp2"":""string2"",""IntProp2"":2}}";
                    Assert.Equal(expected, json);
                    //output.WriteLine("JSON: " + json);

                    var obj2 = ser.ToObject<TestClass1>(json);
                    Assert.Equal(obj.StringProp, obj2.StringProp);
                    Assert.Equal(obj.IntProp, obj2.IntProp);
                    Assert.Equal(obj.Object.StringProp2, obj2.Object.StringProp2);
                    Assert.Equal(obj.Object.IntProp2, obj2.Object.IntProp2);
                });
        }
    }

    public class NewtonsoftJsonTests  : AppTest
    {
        private readonly ITestOutputHelper output;

        public NewtonsoftJsonTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        
        [Fact]
        public async Task AppSerializeToFromJsonStringTest()
        {
            await new AppHost()
                .AddSerialization()
                .AddNewtonsoftJson()
                .RunNowAndWait(() =>
                {
                    var ser = Defaults.TryGet<ISerializationProvider>();
                    var obj = TestClass1.Create;

                    var json = ser.ToString(obj);
                    var expected = @"{""$type"":""LionFire.Serialization.Tests.TestClass1, LionFire.Serialization.Tests"",""StringProp"":""string1"",""IntProp"":1,""Object"":{""StringProp2"":""string2"",""IntProp2"":2}}";
                    Assert.Equal(expected, json);
                    //output.WriteLine("JSON: " + json);

                    var obj2 = ser.ToObject<TestClass1>(json);
                    Assert.Equal(obj.StringProp, obj2.StringProp);
                    Assert.Equal(obj.IntProp, obj2.IntProp);
                    Assert.Equal(obj.Object.StringProp2, obj2.Object.StringProp2);
                    Assert.Equal(obj.Object.IntProp2, obj2.Object.IntProp2);
                });
        }

        [Fact]
        public async void ToUnspecifiedObjectType()
        {
            await new AppHost()
                .AddSerialization()
                .AddNewtonsoftJson()
                .RunNowAndWait(() =>
                {
                    var ser = Defaults.TryGet<ISerializationProvider>();
                    var obj = TestClass1.Create;
                    var json = ser.ToString(obj);

                    var deserialized = ser.ToObject<object>(json);
                    //output.WriteLine("Deserialized type: " + deserialized.GetType().FullName);
                    var obj2 = (TestClass1)deserialized;

                    Assert.Equal(obj.StringProp, obj2.StringProp);
                    Assert.Equal(obj.IntProp, obj2.IntProp);
                    Assert.Equal(obj.Object.StringProp2, obj2.Object.StringProp2);
                    Assert.Equal(obj.Object.IntProp2, obj2.Object.IntProp2);
                });
        }
    }
}
