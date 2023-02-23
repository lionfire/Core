﻿#nullable enable
using LionFire;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting.CommandLine.HostApplicationBuilder_;

public class HostApplicationBuilderBuilder : BuilderBuilderBase<HostApplicationBuilder>
{
    public override IHostingBuilderBuilder ConfigureServices(Action<IServiceCollection> services) { Initializers.Add((_, hab) => services(hab.Services)); return this; }
    public override IHost Build(HostApplicationBuilder builder) => builder.Build();
    //protected override Task _RunConsoleAsync(HostApplicationBuilder builder, CancellationToken cancellationToken = default) => builder.Build().RunAsync(cancellationToken);

}