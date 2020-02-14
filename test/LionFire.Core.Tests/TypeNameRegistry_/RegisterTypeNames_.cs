using LionFire.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using Xunit;
using LionFire.Services;
using Microsoft.Extensions.Options;
using LionFire.ExtensionMethods;
using LionFire.Types;
using Microsoft.Extensions.DependencyInjection;
using LionFire;

namespace TypeNameRegistry_
{
    public class RegisterTypeNames_
    {
        [Fact]
        public async void P_Typical()
        {
            await new HostBuilder()
                .ConfigureServices(services =>
                {
                    services
                        .RegisterTypeNames(typeof(TypeResolver).Assembly, concreteTypesOnly: true)
                    ;
                })
                .RunAsync(serviceProvider =>
                {
                    {
                        var registry = serviceProvider.GetRequiredService<IOptionsMonitor<TypeNameRegistry>>().CurrentValue;
                        var resolvedType = registry.Types.TryGetValue("TypeResolver");
                        Assert.Same(typeof(TypeResolver), resolvedType);
                    }

                    var typeResolver = serviceProvider.GetRequiredService<ITypeResolver>();
                    {
                        var resolvedType = typeResolver.Resolve("TypeResolver");
                        Assert.Same(typeof(TypeResolver), resolvedType);
                    }

                    Assert.Throws<TypeNotFoundException>(() => typeResolver.Resolve("ITypeResolver"));
                    Assert.Null(typeResolver.TryResolve("ITypeResolver"));
                });
        }

        [Fact]
        public async void F_ConcreteOnly()
        {
            await new HostBuilder()
                .ConfigureServices(services =>
                {
                    services
                        .RegisterTypeNames(typeof(TypeResolver).Assembly, concreteTypesOnly: true)
                    ;
                })
                .RunAsync(serviceProvider =>
                {
                    {
                        var registry = serviceProvider.GetRequiredService<IOptionsMonitor<TypeNameRegistry>>().CurrentValue;
                        var resolvedType = registry.Types.TryGetValue("TypeResolver");
                        Assert.Same(typeof(TypeResolver), resolvedType);
                    }

                    var typeResolver = serviceProvider.GetRequiredService<ITypeResolver>();
                    {
                        var resolvedType = typeResolver.Resolve("TypeResolver");
                        Assert.Same(typeof(TypeResolver), resolvedType);
                    }

                    Assert.Throws<TypeNotFoundException>(() => typeResolver.Resolve("ITypeResolver")); // Primary assertion
                    Assert.Null(typeResolver.TryResolve("ITypeResolver")); // Primary assertion
                });
        }

        [Fact]
        public async void P_Filter()
        {
            await new HostBuilder()
                .ConfigureServices(services =>
                {
                    services
                        .RegisterTypeNames(typeof(TypeResolver).Assembly, t => t.Name != "TypeResolver")
                    ;
                })
                .RunAsync(serviceProvider =>
                {
                    {
                        var typeResolver = serviceProvider.GetRequiredService<ITypeResolver>();
                        Assert.Null(typeResolver.TryResolve("TypeResolver"));
                    }
                    {
                        var typeResolver = serviceProvider.GetRequiredService<ITypeResolver>();
                        var resolvedType = typeResolver.Resolve("TypeNameRegistry");
                        Assert.Same(typeof(TypeNameRegistry), resolvedType);
                    }
                });
        }

        [Fact]
        public async void P_Concrete()
        {
            await new HostBuilder()
                .ConfigureServices(services =>
                {
                    services
                        .RegisterTypeNames(typeof(TypeResolver).Assembly, concreteTypesOnly: true)
                    ;
                })
                .RunAsync(serviceProvider =>
                {
                    var typeResolver = serviceProvider.GetRequiredService<ITypeResolver>();
                    Assert.Null(typeResolver.TryResolve("ITypeResolver"));
                    Assert.Same(typeof(TypeResolver), typeResolver.Resolve("TypeResolver"));
                });
        }
    }
}
