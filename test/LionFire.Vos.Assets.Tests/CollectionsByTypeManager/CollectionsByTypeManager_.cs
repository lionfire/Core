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
using LionFire.Vos.Collections.ByType;
using LionFire.Persistence.Testing;
using LionFire.Vos.Collections;

namespace CollectionsByTypeManager_
{
    public class GetCollectionType_
    {
        private readonly ITestOutputHelper output;

        public GetCollectionType_(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async void Pass()
        {
            await VosHost.Create()
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddAssets()
                    ;
                })
                .RunAsync(serviceProvider =>
                {
                    var root = serviceProvider.GetRootVob();

                    var assetManager = root["assets"];

                    var testClass1Dir = assetManager["TestClass1"];
                    var testClass2Dir = root["TestClass2"];

                    var manager = assetManager.AcquireOwn<ICollectionsByTypeManager>();
                    Assert.NotNull(manager);

                    Assert.Equal(typeof(TestClass1), testClass1Dir.GetVobCollectionType());
                    Assert.Equal(typeof(TestClass2), testClass2Dir.GetVobCollectionType());

                });
        }
    }
}

