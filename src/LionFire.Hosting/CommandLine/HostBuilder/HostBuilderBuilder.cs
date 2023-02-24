#nullable enable
using LionFire;
using LionFire.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting.CommandLine.HostBuilder_;

public class HostBuilderBuilder : BuilderBuilderBase<IHostBuilder>
{
    public override IHostingBuilderBuilder ConfigureServices(Action<IServiceCollection> services)
    {
        if (Builder != null) Builder.ConfigureServices(services);
        else AddInitializer((_, b) => b.ConfigureServices(services));
        return this;
    }

    public override IHost Build(IHostBuilder builder) => builder.Build();

    protected override IHostBuilder CreateBuilder() => new HostBuilder();
}
