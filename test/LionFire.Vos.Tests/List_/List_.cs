using LionFire.Hosting;
using System;
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

namespace List_
{
    public class List_
    {
        private readonly ITestOutputHelper output;
        
        public List_(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async void P_List()
        {
            await VosHost.Create()
                .ConfigureServices((_, s) => s.VobEnvironment("key1", "value1"))
                .RunAsync(serviceProvider =>
                {
                    var root = serviceProvider.GetRootVob();
                    var env = root.Environment();

                    Assert.Equal("value1", root.Environment()["key1"]);

                    var child = root["$key1"];

                    Assert.Equal("/value1", child.Path);  // Primary Assertion
                    Assert.Equal("/value1", child.Reference.Path);
                    Assert.Single(child.Reference.PathChunks, "value1");


                    #region Changed value

                    root.Environment("key1", "value1-changed");
                    Assert.Equal("value1-changed", root.Environment("key1"));

                    child = root["$key1"];

                    Assert.Equal("/value1-changed", child.Path);
                    Assert.Equal("/value1-changed", child.Reference.Path);
                    Assert.Single(child.Reference.PathChunks, "value1-changed");

                    #endregion

                    //root.Environment("key1", new VosReference("/value1-changed-to-VosReference"));
                    //Assert.IsType<VosReference>(root.Environment("key1"));
                    //Assert.Equal("/value1-changed-to-VosReference", ((VosReference)root.Environment("key1")).Path);
                });
        }
    }
}
