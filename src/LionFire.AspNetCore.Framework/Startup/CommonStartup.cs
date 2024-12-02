

using LionFire.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LionFire.AspNetCore;

public class CommonStartup<TConfig>
    where TConfig : WebFrameworkConfig//, new()
{
    public IConfiguration Configuration { get; }

    public CommonStartup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public TConfig GetConfig() => (TConfig)Activator.CreateInstance(typeof(TConfig), Configuration)!;

    public virtual void ConfigureServices(IServiceCollection services)
    {
        /*var mvcBuilder = */
        services.AddForOptions(GetConfig());
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //var options = app.ApplicationServices.GetRequiredService<IOptionsMonitor<TConfig>>().CurrentValue; // OLD
        //var options = app.ApplicationServices.GetRequiredService<TConfig>(); // OLD
        var options = WebHostConfigX.GetConfig<TConfig>(Configuration);

        options.Configure(app, env, configureEndpoints: configureEndpoints);

        //ConfigureCore(app, env);
    }

    //protected virtual void ConfigureCore(IApplicationBuilder app, IWebHostEnvironment env) // OLD
    //{
    //    //var options = app.ApplicationServices.GetRequiredService<IOptionsMonitor<TConfig>>().CurrentValue;
    //    //var options = app.ApplicationServices.GetRequiredService<TConfig>();
    //    var options = WebHostConfigX.GetConfig<TConfig>(Configuration);

    //    options.Configure(app, env, configureEndpoints: configureEndpoints);
    //}

    protected virtual Action<IEndpointRouteBuilder>? configureEndpoints => null;

}
