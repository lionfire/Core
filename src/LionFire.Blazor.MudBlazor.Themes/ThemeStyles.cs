namespace LionFire.Blazor.MudBlazor_.Theming;

/// <summary>
/// A "style personality" — fonts, corner radius, and spacing feel. This axis is
/// independent of the colour palette, so any style composes with any theme
/// (e.g. "Dark Ocean" + "editorial").
///
/// Applied client-side via a <c>data-style</c> attribute on the <c>&lt;html&gt;</c>
/// element; the rules live in <c>wwwroot/lionfire-themes.css</c>, served as
/// <c>_content/LionFire.Blazor.MudBlazor.Themes/lionfire-themes.css</c>. Because
/// MudBlazor emits its typography and radius variables in a single <c>:root{}</c>
/// rule, an <c>html[data-style="…"]</c> block simply outranks them — no MudTheme
/// rebuild is needed to switch style.
/// </summary>
public sealed record ThemeStyle(string Id, string Label, string Hint, string SampleFont);

public static class ThemeStyles
{
    public const string Default = "default";

    public static IReadOnlyList<ThemeStyle> All { get; } = new[]
    {
        new ThemeStyle("default", "Default", "MudBlazor stock — Roboto, 4px radius.", "Roboto, sans-serif"),
        new ThemeStyle("technical", "Technical", "IBM Plex Sans + JetBrains Mono headings, sharp 2px corners.", "'JetBrains Mono', monospace"),
        new ThemeStyle("editorial", "Editorial", "Public Sans body, Fraunces serif headings, soft 9px corners.", "Fraunces, Georgia, serif"),
        new ThemeStyle("humanist", "Humanist", "Hanken Grotesk throughout, moderate 6px corners.", "'Hanken Grotesk', sans-serif"),
        new ThemeStyle("soft", "Soft", "Nunito Sans throughout, generous 13px corners.", "'Nunito Sans', sans-serif"),
    };

    public static IReadOnlyList<string> Ids { get; } = All.Select(s => s.Id).ToArray();

    /// <summary>Maps an input to a known style id, falling back to "default".</summary>
    public static string Normalize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return Default;
        var trimmed = input.Trim();
        return Ids.FirstOrDefault(s => string.Equals(s, trimmed, StringComparison.OrdinalIgnoreCase))
               ?? Default;
    }

    /// <summary>
    /// The Google Fonts stylesheet href covering every style personality's fonts.
    /// Add this to your app's &lt;head&gt; if you offer the style axis. One combined
    /// request; only the selected data-style actually applies them.
    /// </summary>
    public const string GoogleFontsHref =
        "https://fonts.googleapis.com/css2?family=Fraunces:opsz,wght@9..144,400;9..144,600" +
        "&family=Hanken+Grotesk:wght@400;500;600;700" +
        "&family=IBM+Plex+Sans:wght@400;500;600;700" +
        "&family=JetBrains+Mono:wght@400;500;700" +
        "&family=Nunito+Sans:wght@400;600;700" +
        "&family=Public+Sans:wght@400;500;600;700&display=swap";
}
