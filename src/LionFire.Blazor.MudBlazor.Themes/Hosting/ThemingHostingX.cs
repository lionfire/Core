using LionFire.Blazor.MudBlazor_.Theming.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LionFire.Blazor.MudBlazor_.Theming;

public static class ThemingHostingX
{
    /// <summary>
    /// Registers the LionFire theming stack: a scoped <see cref="IThemeService"/> over an
    /// <see cref="IThemePreferenceStore"/>.
    ///
    /// The store defaults to <see cref="FileThemePreferenceStore"/> (one file per preference
    /// under ~/.config/lionfire/appearance). A host that already has a per-user preference
    /// mechanism should register its own <see cref="IThemePreferenceStore"/> — this method
    /// uses TryAdd, so a store registered beforehand wins.
    ///
    /// Then, in your app:
    ///   • &lt;head&gt;: link _content/LionFire.Blazor.MudBlazor.Themes/lionfire-themes.css
    ///   • MainLayout: &lt;LionFireThemeProvider /&gt; in place of &lt;MudThemeProvider /&gt;
    ///   • MudAppBar: &lt;ThemeToggle /&gt;
    /// </summary>
    public static IServiceCollection AddLionFireMudTheming(this IServiceCollection services)
    {
        services.TryAddScoped<IThemePreferenceStore>(_ => new FileThemePreferenceStore());
        services.TryAddScoped<IThemeService, ThemeService>();
        return services;
    }

    /// <summary>
    /// Registers the theming stack with a specific preference store implementation.
    /// </summary>
    public static IServiceCollection AddLionFireMudTheming<TStore>(this IServiceCollection services)
        where TStore : class, IThemePreferenceStore
    {
        services.TryAddScoped<IThemePreferenceStore, TStore>();
        services.TryAddScoped<IThemeService, ThemeService>();
        return services;
    }
}
