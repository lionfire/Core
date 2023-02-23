#nullable enable
using LionFire;
using LionFire.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting.CommandLine.HostBuilder_;

public class HostBuilderBuilder : BuilderBuilderBase<IHostBuilder>
{
    public override IHostingBuilderBuilder ConfigureServices(Action<IServiceCollection> services) { Initializers.Add((_, hb) => hb.ConfigureServices(services)); return this; }
    public override IHost Build(IHostBuilder builder) => builder.Build();

    protected override IHostBuilder CreateBuilder() => new HostBuilder();



}
