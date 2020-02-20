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
using LionFire.Types;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        public async void P_Multi()
        {
            await VosHost.Create()
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddAssets()
                        .RegisterTypeName<TestClass1>()
                        .RegisterTypeName<TestClass2>()
                        .Configure<TypeNameRegistry>(registry =>
                        {
                            registry.Types.Add("test", typeof(long));
                            registry.Types.Add("num", typeof(int));
                        })
                        .Configure<TypeNameRegistry>(registry =>
                        {
                            registry.Types.Add("num2", typeof(double));
                        })
                        //.AddSingleton(new TypeNameRegistryInitializer(new Dictionary<string, Type>
                        //{
                        //    ["test"] = typeof(long),
                        //    ["num"] = typeof(int)
                        //}))
                        //.AddSingleton(new TypeNameRegistryInitializer(new Dictionary<string, Type>
                        //{
                        //    ["num2"] = typeof(double)
                        //}))
                        ;
                })
                .RunAsync(serviceProvider =>
                {
                    var root = serviceProvider.GetRootVob();

                    var assets = root["$assets"];

                    var testClass1Dir = assets["TestClass1"];
                    var testClass2Dir = assets["TestClass2"];

                    var provider = assets.AcquireOwn<ICollectionTypeProvider>();
                    Assert.NotNull(provider);

                    Assert.Equal(typeof(TestClass1), testClass1Dir.GetVobCollectionType());
                    Assert.Equal(typeof(TestClass2), testClass2Dir.GetVobCollectionType());

                    Assert.Equal(typeof(long), assets["test"].GetVobCollectionType());
                    Assert.Equal(typeof(int), assets["num"].GetVobCollectionType());
                    Assert.Equal(typeof(double), assets["num2"].GetVobCollectionType());

                    Assert.Null("/testvob/not/in/assets/TestClass1".ToVob().GetVobCollectionType());
                });
        }

        [Fact]
        public async void P_RegisterTypeNamesForAssembly()
        {
            await VosHost.Create()
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddAssets()
                        .RegisterTypeNames(typeof(TestClass1).Assembly)
                        ;
                })
                .RunAsync(serviceProvider =>
                {
                    var assets = serviceProvider.GetRootVob()["$assets"];
                    Assert.Equal(typeof(TestClass1), assets["TestClass1"].GetVobCollectionType());
                    Assert.Equal(typeof(TestClass2), assets["TestClass2"].GetVobCollectionType());
                });
        }
    }
}

