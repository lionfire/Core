using LionFire.Hosting;
using System;
using LionFire.Services;
using System.Collections.Generic;
using System.Text;
using Xunit;
using LionFire.Services;
using LionFire.Vos;
using LionFire.Vos.Environment;
using LionFire.Resolves.ChainResolving;
using LionFire.FlexObjects;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using LionFire.Persistence.Testing;
using LionFire.Referencing;

namespace Write_
{

    public class WriteHandle_
    {
        private readonly ITestOutputHelper output;

        public WriteHandle_(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async void P_Write()
        {
            await VosTestHost.Create().RunAsync(async serviceProvider =>
                {
                    var root = serviceProvider.GetRootVob();
                    var test = "$test".ToVobReference();

                    var child1 = test.GetChild("child1.json").GetReadWriteHandle<TestClass1>();
                    var child2 = test.GetChild("child2.json").GetReadWriteHandle<TestClass2>();

                    child1.Value = TestClass1.Create;
                    await child1.Put();
                    child2.Value = new TestClass2() { IntProp2 = 123, StringProp2 = "4S6" };
                    await child2.Put();

                    //Assert.NotNull(result.Value);
                    //Assert.NotEmpty(result.Value);
                });
        }
    }
}
