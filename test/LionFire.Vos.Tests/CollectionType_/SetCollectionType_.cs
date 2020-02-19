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
using LionFire.Vos.Collections;
using Xunit.Abstractions;

namespace List_
{
    public class SetCollectionType_
    {
        const string path = "/test/testClassCollection";
        private readonly ITestOutputHelper output;

        public SetCollectionType_(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async void Pass()
        {
            await VosHost.Create()
                .ConfigureServices((_, s) => s.InitializeVob(path, v => v.SetCollectionType<LionFire.Persistence.Testing.TestClass1>()))
                .RunAsync(serviceProvider =>
                {
                    var vob = serviceProvider.GetRootVob()[path];
                    Assert.Equal(typeof(LionFire.Persistence.Testing.TestClass1), vob.GetVobCollectionType());

                    output.WriteLine($"Collection type: {vob.GetVobCollectionType().FullName}");
                });
        }

        [Fact]
        public async void P_None()
        {
            await VosHost.Create()
                // Skip: .ConfigureServices((_, s) => s.InitializeVob(path, v => v.SetCollectionType<LionFire.Persistence.Testing.TestClass1>()))
                .RunAsync(serviceProvider =>
                {
                    var vob = serviceProvider.GetRootVob()[path];
                    Assert.Null(vob.GetVobCollectionType());

                    output.WriteLine($"Collection type: {vob.GetVobCollectionType()?.FullName}");
                });
        }
    }
}
