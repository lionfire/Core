#nullable enable
using LionFire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting.CommandLine.HostApplicationBuilder_;

public class HostApplicationBuilderBuilder : BuilderBuilderBase<HostApplicationBuilder>
{
    public override IHostingBuilderBuilder ConfigureServices(Action<IServiceCollection> services)
    {
        if (Builder != null) Builder.ConfigureServices(services);
        else AddInitializer((_, hab) => services(hab.Services));
        return this;
    }
    public override IHost Build(HostApplicationBuilder builder) => builder.Build();
    //protected override Task _RunConsoleAsync(HostApplicationBuilder builder, CancellationToken cancellationToken = default) => builder.Build().RunAsync(cancellationToken);

}
