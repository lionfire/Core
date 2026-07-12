namespace LionFire.Blazor.MudBlazor_.Theming.Persistence;

/// <summary>
/// Where a user's appearance preferences are persisted. Kept deliberately narrow — a
/// string-keyed get/set — so a host can back it with whatever it already has: a config
/// file, browser localStorage, a database row, or (in the Trading silo) an Orleans grain.
///
/// Implementations must not throw on a read or write failure. A preference store that is
/// unavailable should degrade to "no saved preference", never break the app shell.
/// </summary>
public interface IThemePreferenceStore
{
    /// <summary>Returns the persisted value for <paramref name="key"/>, or null if unset.</summary>
    ValueTask<string?> GetAsync(string key);

    /// <summary>Persists <paramref name="value"/> under <paramref name="key"/>. Null clears it.</summary>
    ValueTask SetAsync(string key, string? value);
}

/// <summary>The keys <see cref="ThemeService"/> reads and writes.</summary>
public static class ThemePreferenceKeys
{
    public const string Theme = "appearance.theme";
    public const string BackgroundBrightness = "appearance.backgroundBrightness";
    public const string Style = "appearance.style";
}
