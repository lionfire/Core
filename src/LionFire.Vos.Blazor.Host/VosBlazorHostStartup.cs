using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LionFire.Services;
using LionFire.Persistence.Filesystem;
using LionFire.Hosting;
using LionFire.Serialization.Json.JsonEx;
using MudBlazor.Services;

namespace LionFire.Vos.Blazor;

public class VosBlazorHostStartup
{
    public IWebHostEnvironment Env { get; set; }

    public bool Blazorise { get; set; } = false;

    public VosBlazorHostStartup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        Env = env;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        var mvcBuilder = services.AddRazorPages();
#if DEBUG
        
        if (Env.IsDevelopment())
        {
            mvcBuilder.AddRazorRuntimeCompilation();
        }
        //services.AddRazorComponentsRuntimeCompilation();
#endif

        services.AddServerSideBlazor();

        services
            .AddTypeNameRegistry() // REVIEW - move this somewhere?  Needed by KnownTypes
            //.RegisterTypesNamesWithAttribute(typeof(TimeTrackingItem).Assembly)

            .AddReferenceProvider()
            .AddPersisters()
            .AddFilesystem()

            .AddNewtonsoftJson()


            .AddJsonEx() // OLD?
            .VosMount("/temp".ToVobReference(), @"c:\temp".ToFileReference())
            .VosMount("/TimeTracker".ToVobReference(), @"C:\st\jvos\LionFire\TimeTracker".ToFileReference())
            ;
        ;

        if (Blazorise)
        {
            throw new NotImplementedException(); // OLD
            //services
            //   .AddBlazorise(options =>
            //   {
            //       options.ChangeTextOnKeyPress = true; // optional
            //   })
            //   .AddBootstrapProviders()
            //   .AddFontAwesomeIcons();
        }

        services.AddMudServices();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //app.UseRazorComponentsRuntimeCompilation();
        app.ApplicationServices.InitializeDependencyContextServiceProvider();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        //if (Blazorise)
        //{
        //    app.ApplicationServices
        //      .UseBootstrapProviders()
        //      .UseFontAwesomeIcons()
        //      ;
        //}

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_VosHost");
        });
    }
}
