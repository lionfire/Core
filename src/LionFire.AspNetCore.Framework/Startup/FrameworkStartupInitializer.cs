using HealthChecks.UI.Client;
using LionFire.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;

namespace LionFire.Valor.Action.Silo;

public static class FrameworkStartupInitializer
{
    public static IMvcBuilder? AddForOptions(this IServiceCollection services, WebFrameworkOptions o)
    {

        if (o.HealthChecks)
        {
            services.AddHealthChecks()
            // TODO - Valor Silo specific health checks
            ;

            if (o.HealthChecksUI)
            {
                services
                     .AddHealthChecksUI(setupSettings: setup =>
                     {
                         setup.AddHealthCheckEndpoint("self-detail", $"/health/detail"); // REVIEW - https cert failing?
                     })
                     //.AddSqliteStorage("Data Source=HealthCheckHistory.sqlite3")
                     .AddInMemoryStorage()
                     ;
            }
        }

        var mvcBuilder = AddAspNetCore(services, o);

        return mvcBuilder;
    }
    private static IMvcBuilder? AddAspNetCore(this IServiceCollection services, WebFrameworkOptions o)
    {
        IMvcBuilder? mvcBuilder = null;

        if (o.RequiresMvc) { mvcBuilder = services.AddMvc(); }

        if (o.RequiresControllersWithViews) { mvcBuilder = services.AddControllersWithViews(); }
        else if (o.RequiresControllers) { mvcBuilder = services.AddControllers(); }

        if (o.RequiresServerSideBlazor)
        {
            services.AddServerSideBlazor();
            if (o.RequiresMudBlazor) { services.AddMudServices(); }
        }
        if (o.RequiresRazorPages || o.RequiresServerSideBlazor) { mvcBuilder = services.AddRazorPages(); }

        return mvcBuilder;
    }

    public static WebFrameworkOptions Configure(this WebFrameworkOptions options, IApplicationBuilder app, IWebHostEnvironment env, bool swagger = true, bool endpoints = true, Action<IEndpointRouteBuilder>? configureEndpoints = null)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        if (options.RequiresStaticFiles)
        {
#if DEBUG
            app.UseStaticFiles();
#else
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(AppContext.BaseDirectory,"wwwroot"))
        });
#endif
        }

        if(swagger) options.ConfigureSwagger(app, env);

        app.UseRouting();

        options.ConfigureAuth(app, env);

        if (endpoints)
        {
            app.UseEndpoints(endpoints =>
            {
                configureEndpoints?.Invoke(endpoints);
                options.MapEndpoints(endpoints);
            });
        }

        return options;
    }

    public static WebFrameworkOptions ConfigureSwagger(this WebFrameworkOptions options, IApplicationBuilder app, IWebHostEnvironment env, Action<IApplicationBuilder, IWebHostEnvironment>? customInit = null)
    {
        if (options.SwaggerInDevOnly)
        {
            if (!options.SwaggerInDevOnly || env.IsDevelopment())
            {
                if (customInit != null) customInit(app, env);
                else
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                        //options.RoutePrefix = string.Empty;
                    });
                }
            }
        }
        return options;
    }

    public static WebFrameworkOptions ConfigureAuth(this WebFrameworkOptions options, IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (options.RequiresAuth)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        return options;
    }

    private static WebFrameworkOptions MapHealthCheckEndpoints(this WebFrameworkOptions options, IEndpointRouteBuilder endpoints)
    {
        if (options.HealthChecks)
        {
            endpoints.MapHealthChecks("/health");
            endpoints.MapHealthChecks("/health/detail", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        }
        if (options.HealthChecksUI)
        {
            endpoints.MapHealthChecksUI(config => config.UIPath = "/health-ui");
        }

        return options;
    }

    public static WebFrameworkOptions MapEndpoints(this WebFrameworkOptions options, IEndpointRouteBuilder endpoints)
    {
        options.MapHealthCheckEndpoints(endpoints);

        if (options.RequiresServerSideBlazor)
        {
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        }
        if (options.RequiresServerSideBlazor || options.RequiresRazorPages)
        {
            endpoints.MapRazorPages();
        }
        if(options.RequiresControllers || options.RequiresControllersWithViews) endpoints.MapControllers();
        
        return options;
    }
}
