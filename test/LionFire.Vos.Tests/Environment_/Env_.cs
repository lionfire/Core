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

namespace Environment_
{
    public class Env_
    {
        [Fact]
        public void Pass()
        {
            VosHost.Create()
                .ConfigureServices((_, s) => s.VobEnvironment("key1", "value1"))
                .Run(serviceProvider =>
                {
                    var root = serviceProvider.GetRootVob();
                    var env = root.Environment();

                    Assert.Equal("value1", root.Environment()["key1"]);
                    Assert.Equal("value1", root.Environment("key1"));

                    root.Environment("key1", "value1-changed");
                    Assert.Equal("value1-changed", root.Environment("key1"));

                    root.Environment("key1", new VosReference("/value1-changed-to-VosReference"));
                    Assert.IsType<VosReference>(root.Environment("key1"));
                    Assert.Equal("/value1-changed-to-VosReference", ((VosReference)root.Environment("key1")).Path);
                });
        }
    }
}

