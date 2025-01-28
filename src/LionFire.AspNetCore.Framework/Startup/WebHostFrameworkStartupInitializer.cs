using HealthChecks.UI.Client;
using LionFire.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;

namespace LionFire.Hosting;

public static class WebHostFrameworkStartupInitializer
{
    // TODO: LionFire Builder?  Add Startup class?  lf.WebHost<TStartup> - do I already have this?
    // TODO: Review accessibility and streamline all of this

    public static IMvcBuilder? AddForOptions(this IServiceCollection services, WebFrameworkConfig o)
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
                         setup.AddHealthCheckEndpoint("self-detail", $"/health/detail");
                     })
                     //.AddSqliteStorage("Data Source=HealthCheckHistory.sqlite3")
                     .AddInMemoryStorage()
                     ;
            }
        }
        if (o.Swagger || o.RequiresSwagger)
        {
            services.AddEndpointsApiExplorer();
            services.AddMvcCore()
                .AddApiExplorer();

#if Swashbuckle
            services.AddSwaggerGen();
#else // NSwag:
            services.AddOpenApiDocument(); // add OpenAPI v3 document
                                           //     services.AddSwaggerDocument(); // add Swagger v2 document
#endif
        }
        //if (o.NSwag)
        //{
        //    //services.AddOpenApiDocument(); // add OpenAPI v3 document
        // //      services.AddSwaggerDocument(); // add Swagger v2 document

        //}

        var mvcBuilder = AddAspNetCore(services, o);

        return mvcBuilder;
    }
    private static IMvcBuilder? AddAspNetCore(this IServiceCollection services, WebFrameworkConfig o)
    {
        IMvcBuilder? mvcBuilder = null;

        if (o.RequiresMvc) { mvcBuilder = services.AddMvc(); }
        else if (o.RequiresMvcCore) { /*mvcBuilder = */services.AddMvcCore(); } // TODO - this is not returned

        if (o.RequiresControllersWithViews) { mvcBuilder = services.AddControllersWithViews(); }
        else if (o.RequiresControllers) { mvcBuilder = services.AddControllers(); }

        if (o.RequiresBlazorInteractiveServer)
        {
            services
                .AddRazorComponents()
                .AddInteractiveServerComponents();
            if (o.RequiresMudBlazor) { services.AddMudServices(); }
        }

        if (o.RequiresRazorPages /* OLD: || o.RequiresBlazorInteractiveServer */) { mvcBuilder = services.AddRazorPages(); }


        if (o.RequiresAuth)
        {
            services.AddAuthentication();
            services.AddAuthorization();
        }
        return mvcBuilder;
    }

    public static WebFrameworkConfig Configure(this WebFrameworkConfig options, IApplicationBuilder app, IWebHostEnvironment env, bool swagger = true, bool endpoints = true, Action<IEndpointRouteBuilder>? configureEndpoints = null)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }
        //app.UseHttpsRedirection(); // TODO: based on option, and if both http and https interfaces are configured

        //if (options.RequiresStaticFiles) // Moved to Endpoints / MapStaticFiles. 
        //{
        //    // TODO REVIEW where and whether to set the dir here.
        //    var rootDir = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        //    if (Directory.Exists(rootDir))
        //    {
        //        app.UseStaticFiles(new StaticFileOptions
        //        {
        //            FileProvider = new PhysicalFileProvider(rootDir)
        //        });
        //    }
        //    //else
        //    {
        //        app.UseStaticFiles();
        //    }
        //}
        //if (env.IsDevelopment())
        {
            //StaticWebAssetsLoader.UseStaticWebAssets(ctx.HostingEnvironment, ctx.Configuration);
            StaticWebAssetsLoader.UseStaticWebAssets(env, app.ApplicationServices.GetRequiredService<IConfiguration>());
        }
        if (swagger) options.ConfigureSwagger(app, env);

        app.UseRouting();

        options.ConfigureAuth(app, env);

        if (endpoints)
        {
            app.UseEndpoints(endpoints =>
            {
                // ENH: this could probably be reworked to be a little more flexible
                configureEndpoints?.Invoke(endpoints);
                options.MapEndpoints(endpoints);
            });
        }

        return options;
    }

    public static WebFrameworkConfig ConfigureSwagger(this WebFrameworkConfig options, IApplicationBuilder app, IWebHostEnvironment env, Action<IApplicationBuilder, IWebHostEnvironment>? customInit = null)
    {
        if (options.Swagger || options.RequiresSwagger)
        {
            if (!options.SwaggerInDevOnly || env.IsDevelopment())
            {
                if (customInit != null) customInit(app, env);
                else
                {
#if Swashbuckle
                    app.UseSwagger();
                    app.UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                        //options.RoutePrefix = string.Empty;
                    });
#else // NSwag
                    // Add OpenAPI 3.0 document serving middleware
                    // Available at: http://localhost:<port>/swagger/v1/swagger.json
                    app.UseOpenApi(); // serve OpenAPI/Swagger documents

                    // Add web UIs to interact with the document
                    // Available at: http://localhost:<port>/swagger
                    app.UseSwaggerUi(); // serve Swagger UI
                    app.UseReDoc(s => s
                        .DocumentPath = "/swagger/v1/swagger.json" // Default: /swagger/{documentName}/swagger.json').
                    ); // serve ReDoc UI
#endif
                }
            }
        }
        return options;
    }

    public static WebFrameworkConfig ConfigureAuth(this WebFrameworkConfig options, IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (options.RequiresAuth)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        // Between UseRouting and UseEndpoints, if they exist.
        // Must be after UseAuthentication and UseAuthorization if they exist.
        app.UseAntiforgery();

        return options;
    }

    private static WebFrameworkConfig MapHealthCheckEndpoints(this WebFrameworkConfig options, IEndpointRouteBuilder endpoints)
    {
        if (options.HealthChecks)
        {
            endpoints.MapHealthChecks("/health");
            endpoints.MapHealthChecks("/health/detail", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            if (options.HealthChecksUI)
            {
                endpoints.MapHealthChecksUI(config => config.UIPath = "/health-ui");
            }
        }

        return options;
    }

    public static WebFrameworkConfig MapEndpoints(this WebFrameworkConfig options, IEndpointRouteBuilder endpoints)
    {
        options.MapHealthCheckEndpoints(endpoints);

        if (options.RequiresControllers || options.RequiresControllersWithViews) endpoints.MapControllers();

        RazorComponentsEndpointConventionBuilder? razorComponentsEndpointConventionBuilder = null;

        if (options.RequiresStaticFiles)
        {
            endpoints.MapStaticAssets();
        }

        if (options.RootComponent != null)
        {
            razorComponentsEndpointConventionBuilder = (RazorComponentsEndpointConventionBuilder)typeof(RazorComponentsEndpointRouteBuilderExtensions).GetMethod("MapRazorComponents")!.MakeGenericMethod(options.RootComponent).Invoke(null, [endpoints!])!;

            if (options.AdditionalRazorAssemblies.Length > 0)
            {
                razorComponentsEndpointConventionBuilder.AddAdditionalAssemblies(options.AdditionalRazorAssemblies);
            }
        }

        if (options.RequiresBlazorInteractiveServer)
        {
            if (options.RootComponent != null && razorComponentsEndpointConventionBuilder != null)
            {
                razorComponentsEndpointConventionBuilder.AddInteractiveServerRenderMode();
            }
            else
            {
                //throw new Exception("TODO - REVIEW");
                //endpoints.MapBlazorHub(); // Older versions of .NET
            }
        }
        if (/* OLD - RazorPages no longer needed to bootstrap Blazor  options.RequiresBlazorInteractiveServer || */ options.RequiresRazorPages)
        {
            endpoints.MapRazorPages();
        }

        if (options.Swagger || options.RequiresSwagger)
        {
#if Swashbuckle
            endpoints.MapSwagger();
#else
#endif
        }

        return options;
    }
}
