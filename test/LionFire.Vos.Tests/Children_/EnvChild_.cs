using LionFire.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using LionFire.Services;
using LionFire.Vos;
using LionFire.Vos.Environment;
using LionFire.Data.Async.Gets.ChainResolving;
using LionFire.FlexObjects;
using Microsoft.Extensions.DependencyInjection;

namespace Environment_
{
    public class EnvChild_
    {
        [Fact]
        public void P_Get()
        {
            VosHost.Create()
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

                    //root.Environment("key1", new VobReference("/value1-changed-to-VobReference"));
                    //Assert.IsType<VobReference>(root.Environment("key1"));
                    //Assert.Equal("/value1-changed-to-VobReference", ((VobReference)root.Environment("key1")).Path);
                });
        }
        [Fact]
        public void P_Query()
        {
            VosHost.Create()
                .ConfigureServices((_, s) => s.VobEnvironment("key1", "value1"))
                .RunAsync(serviceProvider =>
                {
                    var root = serviceProvider.GetRootVob();
                    var env = root.Environment();

                    Assert.Equal("value1", root.Environment()["key1"]);

                    var child = root.QueryChild("$key1");
                    Assert.Null(child);

                    {
                        var createChild = root["value1"];
                        var createChild2 = root["value1-changed"];
                    }

                    child = root["$key1"];

                    Assert.NotNull(child);
                    Assert.Equal("/value1", child.Path); // Primary Assertion
                    Assert.Equal("/value1", child.Reference.Path);
                    Assert.Single(child.Reference.PathChunks, "value1");

                    #region Changed value

                    root.Environment("key1", "value1-changed");
                    Assert.Equal("value1-changed", root.Environment("key1"));

                    child = root["$key1"];

                    Assert.NotNull(child);
                    Assert.Equal("/value1-changed", child.Path);
                    Assert.Equal("/value1-changed", child.Reference.Path);
                    Assert.Single(child.Reference.PathChunks, "value1-changed");

                    #endregion

                    //root.Environment("key1", new VobReference("/value1-changed-to-VobReference"));
                    //Assert.IsType<VobReference>(root.Environment("key1"));
                    //Assert.Equal("/value1-changed-to-VobReference", ((VobReference)root.Environment("key1")).Path);
                });

        }
    }

    //public class GetEmptyEnvChild_
    //{

    //    [Fact]
    //    public void Pass()
    //    {
    //        // TOTEST:   $empty = "", $empty2 = <unset>,  Assert.Equal("/test/$empty/$empty2/asdf".ResolveVosPath(), "/test/asdf")

    //    }
    //}
}

