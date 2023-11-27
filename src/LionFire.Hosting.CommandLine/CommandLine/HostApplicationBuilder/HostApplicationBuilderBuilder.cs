#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting.CommandLine.HostApplicationBuilder_;

public class HostApplicationBuilderBuilder : BuilderBuilderBase<HostApplicationBuilder>
{
    public bool? DisableDefaults { get; set; } = null;

    public HostApplicationBuilderProgram? HostApplicationBuilderProgram => Program as HostApplicationBuilderProgram;

    protected override HostApplicationBuilder CreateBuilder()
    {
        var disableDefaults = HostApplicationBuilderProgram != null ? HostApplicationBuilderProgram.DisableDefaults : false;

        if (disableDefaults)
        {
            return new HostApplicationBuilder(new HostApplicationBuilderSettings
            {
                DisableDefaults = true,
                //Args = ..., // TODO?
                //Configuration = ..., //TODO: Parent Configuration?
                //ApplicationName =
            });
        }
        else return new HostApplicationBuilder();
    }


    public override IHostingBuilderBuilder ConfigureServices(Action<IServiceCollection> services)
    {
        if (Builder != null) Builder.ConfigureServices(services);
        else AddInitializer((_, hab) => services(hab.Services));
        return this;
    }
    public override IHost Build(HostApplicationBuilder builder) => builder.Build();

    // REVIEW - delete this?
    //protected override Task _RunConsoleAsync(HostApplicationBuilder builder, CancellationToken cancellationToken = default) => builder.Build().RunAsync(cancellationToken);

}
