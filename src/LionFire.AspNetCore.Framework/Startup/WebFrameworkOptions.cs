

namespace LionFire.AspNetCore;

public class WebFrameworkOptions
{
    public bool Swagger { get; set; } = true;
    public bool SwaggerInDevOnly { get; set; } = false;
    public bool HealthChecks { get; set; } = true;
    public bool HealthChecksUI { get; set; } = true;

    public bool DiagnosticWebUI { get; set; } = true;

    public bool HasAnyUI { get => RequiresServerSideBlazor || DiagnosticWebUI || HealthChecksUI; }

    #region Program Requriements: set in derived classes

    public virtual bool RequiresAuth => false;
    public virtual bool RequiresServerSideBlazor => false;
    public virtual bool RequiresMudBlazor => true;

    public virtual bool RequiresControllers => false;
    public virtual bool RequiresStaticFiles => RequiresServerSideBlazor || HealthChecksUI || DiagnosticWebUI || Swagger;

    public virtual bool RequiresMvc => false;

    public virtual bool RequiresControllersWithViews => false;

    public virtual bool RequiresRazorPages => RequiresServerSideBlazor;

    #endregion
}