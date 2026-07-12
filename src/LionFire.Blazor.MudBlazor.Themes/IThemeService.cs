using MudBlazor;

namespace LionFire.Blazor.MudBlazor_.Theming;

/// <summary>
/// The app's appearance state, across three independent axes:
///
///   • <see cref="Theme"/>      — a named colour palette from <see cref="LionFireThemes.AvailableThemes"/>.
///   • <see cref="Style"/>      — a style personality (fonts / radius) from <see cref="ThemeStyles"/>.
///   • <see cref="BackgroundBrightness"/> — a signed lightness offset applied to background surfaces only.
///
/// The axes compose: any style works with any palette at any brightness. Every setter
/// persists via the registered <see cref="Persistence.IThemePreferenceStore"/> and raises
/// <see cref="ThemeChanged"/> so the shell rebuilds.
///
/// Registered scoped (per Blazor circuit). Call <see cref="InitializeAsync"/> once before
/// first read — <c>LionFireThemeProvider</c> does this for you.
/// </summary>
public interface IThemeService
{
    /// <summary>Loads persisted preferences. Idempotent; safe to call more than once.</summary>
    Task InitializeAsync();

    /// <summary>Current palette name (a value from <see cref="LionFireThemes.AvailableThemes"/>).</summary>
    string Theme { get; }

    /// <summary>True when <see cref="Theme"/> is a dark variant.</summary>
    bool IsDark { get; }

    /// <summary>
    /// The effective MudTheme: the named palette with <see cref="BackgroundBrightness"/> applied.
    /// Bind this to <c>MudThemeProvider.Theme</c>.
    /// </summary>
    MudTheme EffectiveTheme { get; }

    /// <summary>
    /// Sets the palette and persists it. Unknown names throw <see cref="ArgumentException"/>;
    /// the legacy "light"/"dark" values are accepted and normalized.
    /// </summary>
    Task SetThemeAsync(string theme);

    /// <summary>
    /// Toggles between Light and the most recently used dark palette (defaults to "Dark"
    /// if no dark palette has been used this session). This is the quick-toggle; it does
    /// not lose the user's chosen dark variant.
    /// </summary>
    Task<string> ToggleThemeAsync();

    /// <summary>Current style-personality id (one of <see cref="ThemeStyles.Ids"/>).</summary>
    string Style { get; }

    /// <summary>Sets the style id and persists it. Unknown ids fall back to "default".</summary>
    Task SetStyleAsync(string style);

    /// <summary>Signed HSL-lightness offset for background surfaces; 0 means no adjustment.</summary>
    double BackgroundBrightness { get; }

    /// <summary>Allowed range for <see cref="BackgroundBrightness"/>.</summary>
    static double MinBrightness => -0.40;
    static double MaxBrightness => 0.40;

    /// <summary>Sets the brightness offset (clamped to the allowed range) and persists it.</summary>
    Task SetBackgroundBrightnessAsync(double brightness);

    /// <summary>Raised whenever any axis changes. Carries the current palette name.</summary>
    event EventHandler<string>? ThemeChanged;
}
