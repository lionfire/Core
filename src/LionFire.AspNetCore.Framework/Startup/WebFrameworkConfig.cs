﻿using LionFire.Net;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace LionFire.AspNetCore;

public class WebFrameworkConfig : WebHostConfig
{
    // See also:
    // .ConfigureHostConfiguration(b =>
    //    {
    //    b.AddInMemoryCollection(new Dictionary<string, string?>
    //    {
    //        ["Kestrel:Endpoints:Http:Url"] = $"http://{config.HttpInterface}:{config.HttpPort}",
    //    });

    #region Construction

    public WebFrameworkConfig(IConfiguration configuration) : base(configuration)
    {
    }

    #endregion


    #region Imported

    #endregion

    #region Swagger

    public bool Swagger => RequiresSwagger;
    public bool SwaggerInDevOnly { get; set; } = false;
    public virtual bool RequiresSwagger => false;

    #endregion

    #region Health Checks

    public bool HealthChecks { get; set; } = true;
    public bool HealthChecksUI { get; set; } = true;

    #endregion

    #region Diagnostics

    public virtual bool DiagnosticWebUI => DiagnosticBlazorServerUI;

    public bool DiagnosticBlazorServerUI { get; set; } = false; // REVIEW - rethink this. Maybe just DiagnosticUI?

    #endregion

    #region Program Requirements: set in derived classes

    #region Overrides

    public override bool RequiresStaticFiles => base.RequiresStaticFiles || HealthChecksUI || DiagnosticWebUI || Swagger;

    public override bool RequiresBlazorServer => DiagnosticBlazorServerUI || base.BlazorServer;

    #endregion

    public virtual bool RequiresMudBlazor => true;

    public override bool RequiresMvcCore => base.RequiresMvcCore || Swagger || RequiresSwagger;
    #endregion


    #region Derived

    public override bool HasAnyWebUI => base.HasAnyWebUI || DiagnosticWebUI || HealthChecksUI;

    public override bool HasAnyFeatures => base.HasAnyFeatures || HealthChecks || HealthChecksUI || DiagnosticBlazorServerUI;
    #endregion
}

