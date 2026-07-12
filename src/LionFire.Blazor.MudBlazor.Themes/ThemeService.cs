using LionFire.Blazor.MudBlazor_.Theming.Persistence;
using MudBlazor;
using System.Globalization;

namespace LionFire.Blazor.MudBlazor_.Theming;

/// <inheritdoc cref="IThemeService"/>
public sealed class ThemeService : IThemeService
{
    private readonly IThemePreferenceStore _store;

    private string _theme = LionFireThemes.DefaultDark;
    private string _lastDarkTheme = LionFireThemes.DefaultDark;
    private string _style = ThemeStyles.Default;
    private double _brightness;

    // Cached rather than a bool flag: the provider and the app-bar toggle both call
    // InitializeAsync during the same render pass. A "set _initialized then await" guard
    // would let the second caller through with the defaults still in place, and it would
    // never learn the real values. Handing every caller the same Task makes them all wait
    // for the one load.
    private Task? _initTask;

    public ThemeService(IThemePreferenceStore store) => _store = store;

    public event EventHandler<string>? ThemeChanged;

    public string Theme => _theme;
    public string Style => _style;
    public double BackgroundBrightness => _brightness;
    public bool IsDark => LionFireThemes.IsDarkTheme(_theme);

    public MudTheme EffectiveTheme =>
        LionFireThemes.WithBackgroundBrightness(LionFireThemes.GetTheme(_theme), _brightness);

    public Task InitializeAsync() => _initTask ??= LoadAsync();

    private async Task LoadAsync()
    {
        var savedTheme = LionFireThemes.Normalize(await _store.GetAsync(ThemePreferenceKeys.Theme));
        if (savedTheme is not null)
        {
            _theme = savedTheme;
            if (LionFireThemes.IsDarkTheme(savedTheme)) _lastDarkTheme = savedTheme;
        }

        _style = ThemeStyles.Normalize(await _store.GetAsync(ThemePreferenceKeys.Style));

        var savedBrightness = await _store.GetAsync(ThemePreferenceKeys.BackgroundBrightness);
        if (double.TryParse(savedBrightness, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
        {
            _brightness = Clamp(parsed);
        }
    }

    public async Task SetThemeAsync(string theme)
    {
        var normalized = LionFireThemes.Normalize(theme)
            ?? throw new ArgumentException(
                $"Unknown theme '{theme}'. Valid names: {string.Join(", ", LionFireThemes.AvailableThemes)}",
                nameof(theme));

        _theme = normalized;
        if (LionFireThemes.IsDarkTheme(normalized)) _lastDarkTheme = normalized;

        await _store.SetAsync(ThemePreferenceKeys.Theme, normalized);
        ThemeChanged?.Invoke(this, _theme);
    }

    public async Task<string> ToggleThemeAsync()
    {
        var next = IsDark ? LionFireThemes.DefaultLight : _lastDarkTheme;
        await SetThemeAsync(next);
        return next;
    }

    public async Task SetStyleAsync(string style)
    {
        _style = ThemeStyles.Normalize(style);
        await _store.SetAsync(ThemePreferenceKeys.Style, _style);

        // Style is applied via a data-style attribute rather than a MudTheme rebuild, but
        // subscribers still want to know (e.g. to push the attribute onto <html>).
        ThemeChanged?.Invoke(this, _theme);
    }

    public async Task SetBackgroundBrightnessAsync(double brightness)
    {
        _brightness = Clamp(brightness);
        await _store.SetAsync(
            ThemePreferenceKeys.BackgroundBrightness,
            _brightness.ToString("G", CultureInfo.InvariantCulture));

        ThemeChanged?.Invoke(this, _theme);
    }

    private static double Clamp(double v) =>
        Math.Clamp(v, IThemeService.MinBrightness, IThemeService.MaxBrightness);
}
