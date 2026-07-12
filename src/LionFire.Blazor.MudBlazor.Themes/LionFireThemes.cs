using MudBlazor;
using MudBlazor.Utilities;

namespace LionFire.Blazor.MudBlazor_.Theming;

/// <summary>
/// The LionFire named-palette catalog: one light theme and eleven dark variants.
///
/// A palette is selected by name (see <see cref="AvailableThemes"/>). The colour
/// palette is independent of the "style" axis (fonts/radius, see <see cref="ThemeStyles"/>)
/// and of the background-brightness offset, so any combination composes.
/// </summary>
public static class LionFireThemes
{
    public const string DefaultLight = "Light";
    public const string DefaultDark = "Dark";

    public static IReadOnlyList<string> AvailableThemes { get; } = new[]
    {
        "Light",
        "Dark",
        "Dark Ocean",
        "Dark Forest",
        "Dark Sunset",
        "Dark Slate",
        "Dark Crimson",
        "Dark Violet",
        "Dark Amber",
        "Dark Emerald",
        "Dark Nord",
        "Dark Dracula",
    };

    public static MudTheme GetTheme(string themeName) => themeName switch
    {
        "Light" => LightTheme,
        "Dark" => DarkTheme,
        "Dark Ocean" => DarkOceanTheme,
        "Dark Forest" => DarkForestTheme,
        "Dark Sunset" => DarkSunsetTheme,
        "Dark Slate" => DarkSlateTheme,
        "Dark Crimson" => DarkCrimsonTheme,
        "Dark Violet" => DarkVioletTheme,
        "Dark Amber" => DarkAmberTheme,
        "Dark Emerald" => DarkEmeraldTheme,
        "Dark Nord" => DarkNordTheme,
        "Dark Dracula" => DarkDraculaTheme,
        _ => DarkTheme,
    };

    public static bool IsDarkTheme(string themeName) => themeName != "Light";

    /// <summary>
    /// Maps an arbitrary input (including the legacy lowercase "light"/"dark" values)
    /// onto a canonical name from <see cref="AvailableThemes"/>, or null if unknown.
    /// </summary>
    public static string? Normalize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        var trimmed = input.Trim();

        if (string.Equals(trimmed, "light", StringComparison.OrdinalIgnoreCase)) return "Light";
        if (string.Equals(trimmed, "dark", StringComparison.OrdinalIgnoreCase)) return "Dark";

        return AvailableThemes.FirstOrDefault(
            t => string.Equals(t, trimmed, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Three representative swatch colours for <paramref name="themeName"/>, for rendering
    /// a preview chip in a theme picker.
    /// </summary>
    public static IReadOnlyList<string> SwatchColors(string themeName)
    {
        var theme = GetTheme(themeName);
        var palette = IsDarkTheme(themeName)
            ? (Palette)theme.PaletteDark
            : theme.PaletteLight;

        return new[]
        {
            palette.Background.ToString(MudColorOutputFormats.Hex),
            palette.Surface.ToString(MudColorOutputFormats.Hex),
            palette.Primary.ToString(MudColorOutputFormats.Hex),
        };
    }

    /// <summary>
    /// Returns a clone of <paramref name="baseTheme"/> whose background-surface palette
    /// entries (Background, Surface, BackgroundGray, DrawerBackground, AppbarBackground)
    /// have their HSL lightness shifted by <paramref name="brightness"/>. All other entries
    /// — text, accent, and semantic colours — are copied verbatim, so contrast relationships
    /// that the palette author chose are preserved.
    ///
    /// <paramref name="brightness"/> is a signed fraction of the [0,1] HSL lightness axis
    /// (+0.15 lightens by 15 percentage points). MudColor.ChangeLightness clamps to [0,1].
    ///
    /// The static theme instances are never mutated; a fresh MudTheme is returned each call.
    /// </summary>
    public static MudTheme WithBackgroundBrightness(MudTheme baseTheme, double brightness)
    {
        if (brightness == 0d) return baseTheme;

        var result = new MudTheme
        {
            PaletteLight = ShiftLight(baseTheme.PaletteLight, brightness),
            PaletteDark = ShiftDark(baseTheme.PaletteDark, brightness),
        };
        return result;
    }

    private static PaletteLight ShiftLight(PaletteLight src, double b)
    {
        var p = new PaletteLight();
        CopyCommon(src, p);
        ShiftSurfaces(src, p, b);
        return p;
    }

    private static PaletteDark ShiftDark(PaletteDark src, double b)
    {
        var p = new PaletteDark();
        CopyCommon(src, p);
        ShiftSurfaces(src, p, b);
        return p;
    }

    /// <summary>The five background-surface entries, lightness-shifted.</summary>
    private static void ShiftSurfaces(Palette src, Palette dst, double b)
    {
        dst.Background = src.Background.ChangeLightness(b);
        dst.Surface = src.Surface.ChangeLightness(b);
        dst.BackgroundGray = src.BackgroundGray.ChangeLightness(b);
        dst.DrawerBackground = src.DrawerBackground.ChangeLightness(b);
        dst.AppbarBackground = src.AppbarBackground.ChangeLightness(b);
    }

    /// <summary>Everything that is not a background surface, copied verbatim.</summary>
    private static void CopyCommon(Palette src, Palette dst)
    {
        dst.Primary = src.Primary;
        dst.PrimaryContrastText = src.PrimaryContrastText;
        dst.PrimaryDarken = src.PrimaryDarken;
        dst.PrimaryLighten = src.PrimaryLighten;
        dst.Secondary = src.Secondary;
        dst.SecondaryContrastText = src.SecondaryContrastText;
        dst.SecondaryDarken = src.SecondaryDarken;
        dst.SecondaryLighten = src.SecondaryLighten;
        dst.Tertiary = src.Tertiary;
        dst.TertiaryContrastText = src.TertiaryContrastText;
        dst.TertiaryDarken = src.TertiaryDarken;
        dst.TertiaryLighten = src.TertiaryLighten;
        dst.Info = src.Info;
        dst.InfoContrastText = src.InfoContrastText;
        dst.InfoDarken = src.InfoDarken;
        dst.InfoLighten = src.InfoLighten;
        dst.Success = src.Success;
        dst.SuccessContrastText = src.SuccessContrastText;
        dst.SuccessDarken = src.SuccessDarken;
        dst.SuccessLighten = src.SuccessLighten;
        dst.Warning = src.Warning;
        dst.WarningContrastText = src.WarningContrastText;
        dst.WarningDarken = src.WarningDarken;
        dst.WarningLighten = src.WarningLighten;
        dst.Error = src.Error;
        dst.ErrorContrastText = src.ErrorContrastText;
        dst.ErrorDarken = src.ErrorDarken;
        dst.ErrorLighten = src.ErrorLighten;
        dst.Dark = src.Dark;
        dst.DarkContrastText = src.DarkContrastText;
        dst.DarkDarken = src.DarkDarken;
        dst.DarkLighten = src.DarkLighten;
        dst.TextPrimary = src.TextPrimary;
        dst.TextSecondary = src.TextSecondary;
        dst.TextDisabled = src.TextDisabled;
        dst.ActionDefault = src.ActionDefault;
        dst.ActionDisabled = src.ActionDisabled;
        dst.ActionDisabledBackground = src.ActionDisabledBackground;
        dst.DrawerText = src.DrawerText;
        dst.DrawerIcon = src.DrawerIcon;
        dst.AppbarText = src.AppbarText;
        dst.Divider = src.Divider;
        dst.DividerLight = src.DividerLight;
        dst.TableLines = src.TableLines;
        dst.TableStriped = src.TableStriped;
        dst.TableHover = src.TableHover;
        dst.LinesDefault = src.LinesDefault;
        dst.LinesInputs = src.LinesInputs;
        dst.HoverOpacity = src.HoverOpacity;
        dst.GrayDefault = src.GrayDefault;
        dst.GrayLight = src.GrayLight;
        dst.GrayLighter = src.GrayLighter;
        dst.GrayDark = src.GrayDark;
        dst.GrayDarker = src.GrayDarker;
        dst.OverlayDark = src.OverlayDark;
        dst.OverlayLight = src.OverlayLight;
        dst.Black = src.Black;
        dst.White = src.White;
        dst.BorderOpacity = src.BorderOpacity;
        dst.RippleOpacity = src.RippleOpacity;
        dst.RippleOpacitySecondary = src.RippleOpacitySecondary;
        dst.Skeleton = src.Skeleton;
    }

    #region Theme definitions

    /// <summary>MudBlazor default light theme.</summary>
    public static MudTheme LightTheme => new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = Colors.Blue.Default,
            Secondary = Colors.Pink.Accent2,
            AppbarBackground = Colors.Blue.Default,
            Background = Colors.Gray.Lighten5,
            DrawerBackground = "#FFF",
            DrawerText = "rgba(0,0,0, 0.7)",
            Success = Colors.Green.Accent4,
        }
    };

    /// <summary>MudBlazor default dark theme.</summary>
    public static MudTheme DarkTheme => new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = Colors.Blue.Lighten1,
            Secondary = Colors.Pink.Accent2,
            AppbarBackground = "#27272f",
            Background = "#1a1a27",
            BackgroundGray = "#151521",
            Surface = "#1e1e2d",
            DrawerBackground = "#1a1a27",
            DrawerText = "rgba(255,255,255, 0.7)",
            Success = Colors.Green.Accent3,
            Black = "#27272f",
            White = "rgba(255,255,255, 0.87)",
            TextPrimary = "rgba(255,255,255, 0.87)",
            TextSecondary = "rgba(255,255,255, 0.60)",
            ActionDefault = "#adadb1",
            ActionDisabled = "rgba(255,255,255, 0.26)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)",
            Divider = "rgba(255,255,255, 0.12)",
            DividerLight = "rgba(255,255,255, 0.06)",
            TableLines = "rgba(255,255,255, 0.12)",
            LinesDefault = "rgba(255,255,255, 0.12)",
            LinesInputs = "rgba(255,255,255, 0.3)",
            TextDisabled = "rgba(255,255,255, 0.38)",
        }
    };

    /// <summary>Deep blues with teal accents, inspired by deep sea.</summary>
    public static MudTheme DarkOceanTheme => new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#00ACC1",
            Secondary = "#26C6DA",
            Tertiary = "#0097A7",
            AppbarBackground = "#0D1B2A",
            Background = "#1B263B",
            BackgroundGray = "#0D1B2A",
            Surface = "#1E2D3D",
            DrawerBackground = "#1B263B",
            DrawerText = "rgba(255,255,255, 0.8)",
            Success = "#26A69A",
            Warning = "#FFA726",
            Error = "#EF5350",
            Info = "#29B6F6",
            Black = "#0D1B2A",
            White = "rgba(255,255,255, 0.9)",
            TextPrimary = "rgba(255,255,255, 0.9)",
            TextSecondary = "rgba(255,255,255, 0.65)",
            ActionDefault = "#B0BEC5",
            ActionDisabled = "rgba(255,255,255, 0.3)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)",
            Divider = "rgba(255,255,255, 0.15)",
            DividerLight = "rgba(255,255,255, 0.08)",
            TableLines = "rgba(255,255,255, 0.15)",
            LinesDefault = "rgba(255,255,255, 0.15)",
            LinesInputs = "rgba(255,255,255, 0.35)",
            TextDisabled = "rgba(255,255,255, 0.4)",
        }
    };

    /// <summary>Deep greens with earthy tones, inspired by forest night.</summary>
    public static MudTheme DarkForestTheme => new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#66BB6A",
            Secondary = "#81C784",
            Tertiary = "#4CAF50",
            AppbarBackground = "#1B2618",
            Background = "#263A29",
            BackgroundGray = "#1B2618",
            Surface = "#2D4A2E",
            DrawerBackground = "#263A29",
            DrawerText = "rgba(255,255,255, 0.8)",
            Success = "#81C784",
            Warning = "#FFB74D",
            Error = "#E57373",
            Info = "#64B5F6",
            Black = "#1B2618",
            White = "rgba(255,255,255, 0.9)",
            TextPrimary = "rgba(255,255,255, 0.88)",
            TextSecondary = "rgba(255,255,255, 0.65)",
            ActionDefault = "#A5D6A7",
            ActionDisabled = "rgba(255,255,255, 0.3)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)",
            Divider = "rgba(255,255,255, 0.15)",
            DividerLight = "rgba(255,255,255, 0.08)",
            TableLines = "rgba(255,255,255, 0.15)",
            LinesDefault = "rgba(255,255,255, 0.15)",
            LinesInputs = "rgba(255,255,255, 0.35)",
            TextDisabled = "rgba(255,255,255, 0.4)",
        }
    };

    /// <summary>Purples and oranges, inspired by twilight sky.</summary>
    public static MudTheme DarkSunsetTheme => new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#AB47BC",
            Secondary = "#FF7043",
            Tertiary = "#8E24AA",
            AppbarBackground = "#2D1B2E",
            Background = "#3A2540",
            BackgroundGray = "#2D1B2E",
            Surface = "#4A2F4D",
            DrawerBackground = "#3A2540",
            DrawerText = "rgba(255,255,255, 0.8)",
            Success = "#66BB6A",
            Warning = "#FFA726",
            Error = "#EF5350",
            Info = "#42A5F5",
            Black = "#2D1B2E",
            White = "rgba(255,255,255, 0.9)",
            TextPrimary = "rgba(255,255,255, 0.9)",
            TextSecondary = "rgba(255,255,255, 0.65)",
            ActionDefault = "#CE93D8",
            ActionDisabled = "rgba(255,255,255, 0.3)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)",
            Divider = "rgba(255,255,255, 0.15)",
            DividerLight = "rgba(255,255,255, 0.08)",
            TableLines = "rgba(255,255,255, 0.15)",
            LinesDefault = "rgba(255,255,255, 0.15)",
            LinesInputs = "rgba(255,255,255, 0.35)",
            TextDisabled = "rgba(255,255,255, 0.4)",
        }
    };

    /// <summary>Professional greys and blues, inspired by modern IDEs.</summary>
    public static MudTheme DarkSlateTheme => new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#64B5F6",
            Secondary = "#90CAF9",
            Tertiary = "#42A5F5",
            AppbarBackground = "#1E1E1E",
            Background = "#252526",
            BackgroundGray = "#1E1E1E",
            Surface = "#2D2D30",
            DrawerBackground = "#252526",
            DrawerText = "rgba(255,255,255, 0.8)",
            Success = "#4EC9B0",
            Warning = "#D7BA7D",
            Error = "#F48771",
            Info = "#64B5F6",
            Black = "#1E1E1E",
            White = "rgba(255,255,255, 0.9)",
            TextPrimary = "rgba(255,255,255, 0.87)",
            TextSecondary = "rgba(255,255,255, 0.60)",
            ActionDefault = "#CCCCCC",
            ActionDisabled = "rgba(255,255,255, 0.3)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)",
            Divider = "rgba(255,255,255, 0.12)",
            DividerLight = "rgba(255,255,255, 0.06)",
            TableLines = "rgba(255,255,255, 0.12)",
            LinesDefault = "rgba(255,255,255, 0.12)",
            LinesInputs = "rgba(255,255,255, 0.35)",
            TextDisabled = "rgba(255,255,255, 0.38)",
        }
    };

    /// <summary>Deep reds and burgundy, inspired by wine and roses.</summary>
    public static MudTheme DarkCrimsonTheme => new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#E57373",
            Secondary = "#F06292",
            Tertiary = "#EF5350",
            AppbarBackground = "#3D1F1F",
            Background = "#4A2828",
            BackgroundGray = "#3D1F1F",
            Surface = "#5A3030",
            DrawerBackground = "#4A2828",
            DrawerText = "rgba(255,255,255, 0.8)",
            Success = "#81C784",
            Warning = "#FFB74D",
            Error = "#EF5350",
            Info = "#64B5F6",
            Black = "#3D1F1F",
            White = "rgba(255,255,255, 0.9)",
            TextPrimary = "rgba(255,255,255, 0.9)",
            TextSecondary = "rgba(255,255,255, 0.65)",
            ActionDefault = "#FFCDD2",
            ActionDisabled = "rgba(255,255,255, 0.3)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)",
            Divider = "rgba(255,255,255, 0.15)",
            DividerLight = "rgba(255,255,255, 0.08)",
            TableLines = "rgba(255,255,255, 0.15)",
            LinesDefault = "rgba(255,255,255, 0.15)",
            LinesInputs = "rgba(255,255,255, 0.35)",
            TextDisabled = "rgba(255,255,255, 0.4)",
        }
    };

    /// <summary>Rich purples and lavenders, inspired by night flowers.</summary>
    public static MudTheme DarkVioletTheme => new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#9575CD",
            Secondary = "#B39DDB",
            Tertiary = "#7E57C2",
            AppbarBackground = "#261F34",
            Background = "#352B49",
            BackgroundGray = "#261F34",
            Surface = "#3F3451",
            DrawerBackground = "#352B49",
            DrawerText = "rgba(255,255,255, 0.8)",
            Success = "#81C784",
            Warning = "#FFB74D",
            Error = "#E57373",
            Info = "#64B5F6",
            Black = "#261F34",
            White = "rgba(255,255,255, 0.9)",
            TextPrimary = "rgba(255,255,255, 0.9)",
            TextSecondary = "rgba(255,255,255, 0.65)",
            ActionDefault = "#D1C4E9",
            ActionDisabled = "rgba(255,255,255, 0.3)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)",
            Divider = "rgba(255,255,255, 0.15)",
            DividerLight = "rgba(255,255,255, 0.08)",
            TableLines = "rgba(255,255,255, 0.15)",
            LinesDefault = "rgba(255,255,255, 0.15)",
            LinesInputs = "rgba(255,255,255, 0.35)",
            TextDisabled = "rgba(255,255,255, 0.4)",
        }
    };

    /// <summary>Warm oranges and browns, inspired by autumn leaves.</summary>
    public static MudTheme DarkAmberTheme => new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#FFB74D",
            Secondary = "#FFCC80",
            Tertiary = "#FFA726",
            AppbarBackground = "#3D2817",
            Background = "#4A3020",
            BackgroundGray = "#3D2817",
            Surface = "#5A3C28",
            DrawerBackground = "#4A3020",
            DrawerText = "rgba(255,255,255, 0.8)",
            Success = "#81C784",
            Warning = "#FFA726",
            Error = "#E57373",
            Info = "#64B5F6",
            Black = "#3D2817",
            White = "rgba(255,255,255, 0.9)",
            TextPrimary = "rgba(255,255,255, 0.9)",
            TextSecondary = "rgba(255,255,255, 0.65)",
            ActionDefault = "#FFE0B2",
            ActionDisabled = "rgba(255,255,255, 0.3)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)",
            Divider = "rgba(255,255,255, 0.15)",
            DividerLight = "rgba(255,255,255, 0.08)",
            TableLines = "rgba(255,255,255, 0.15)",
            LinesDefault = "rgba(255,255,255, 0.15)",
            LinesInputs = "rgba(255,255,255, 0.35)",
            TextDisabled = "rgba(255,255,255, 0.4)",
        }
    };

    /// <summary>Rich teals and greens, inspired by precious gems.</summary>
    public static MudTheme DarkEmeraldTheme => new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#4DB6AC",
            Secondary = "#80CBC4",
            Tertiary = "#26A69A",
            AppbarBackground = "#1A3D39",
            Background = "#1E4D47",
            BackgroundGray = "#1A3D39",
            Surface = "#285750",
            DrawerBackground = "#1E4D47",
            DrawerText = "rgba(255,255,255, 0.8)",
            Success = "#66BB6A",
            Warning = "#FFB74D",
            Error = "#E57373",
            Info = "#4FC3F7",
            Black = "#1A3D39",
            White = "rgba(255,255,255, 0.9)",
            TextPrimary = "rgba(255,255,255, 0.9)",
            TextSecondary = "rgba(255,255,255, 0.65)",
            ActionDefault = "#B2DFDB",
            ActionDisabled = "rgba(255,255,255, 0.3)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)",
            Divider = "rgba(255,255,255, 0.15)",
            DividerLight = "rgba(255,255,255, 0.08)",
            TableLines = "rgba(255,255,255, 0.15)",
            LinesDefault = "rgba(255,255,255, 0.15)",
            LinesInputs = "rgba(255,255,255, 0.35)",
            TextDisabled = "rgba(255,255,255, 0.4)",
        }
    };

    /// <summary>The Nord colour scheme (polar night palette).</summary>
    public static MudTheme DarkNordTheme => new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#88C0D0",            // Nord8  — frost blue
            Secondary = "#81A1C1",          // Nord9
            Tertiary = "#5E81AC",           // Nord10
            AppbarBackground = "#2E3440",   // Nord0  — polar night
            Background = "#3B4252",         // Nord1
            BackgroundGray = "#2E3440",     // Nord0
            Surface = "#434C5E",            // Nord2
            DrawerBackground = "#3B4252",   // Nord1
            DrawerText = "rgba(236, 239, 244, 0.8)",
            Success = "#A3BE8C",            // Nord14 — aurora green
            Warning = "#EBCB8B",            // Nord13 — aurora yellow
            Error = "#BF616A",              // Nord11 — aurora red
            Info = "#81A1C1",               // Nord9
            Black = "#2E3440",
            White = "#ECEFF4",              // Nord6
            TextPrimary = "#ECEFF4",
            TextSecondary = "#D8DEE9",      // Nord4
            ActionDefault = "#D8DEE9",
            ActionDisabled = "rgba(236, 239, 244, 0.3)",
            ActionDisabledBackground = "rgba(236, 239, 244, 0.12)",
            Divider = "rgba(236, 239, 244, 0.15)",
            DividerLight = "rgba(236, 239, 244, 0.08)",
            TableLines = "rgba(236, 239, 244, 0.15)",
            LinesDefault = "rgba(236, 239, 244, 0.15)",
            LinesInputs = "rgba(236, 239, 244, 0.35)",
            TextDisabled = "rgba(236, 239, 244, 0.4)",
        }
    };

    /// <summary>The Dracula colour scheme.</summary>
    public static MudTheme DarkDraculaTheme => new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#BD93F9",            // purple
            Secondary = "#FF79C6",          // pink
            Tertiary = "#8BE9FD",           // cyan
            AppbarBackground = "#21222C",
            Background = "#282A36",
            BackgroundGray = "#21222C",
            Surface = "#343746",            // current line
            DrawerBackground = "#282A36",
            DrawerText = "rgba(248, 248, 242, 0.8)",
            Success = "#50FA7B",
            Warning = "#F1FA8C",
            Error = "#FF5555",
            Info = "#8BE9FD",
            Black = "#21222C",
            White = "#F8F8F2",              // foreground
            TextPrimary = "#F8F8F2",
            TextSecondary = "#6272A4",      // comment
            ActionDefault = "#6272A4",
            ActionDisabled = "rgba(248, 248, 242, 0.3)",
            ActionDisabledBackground = "rgba(248, 248, 242, 0.12)",
            Divider = "rgba(248, 248, 242, 0.15)",
            DividerLight = "rgba(248, 248, 242, 0.08)",
            TableLines = "rgba(248, 248, 242, 0.15)",
            LinesDefault = "rgba(248, 248, 242, 0.15)",
            LinesInputs = "rgba(248, 248, 242, 0.35)",
            TextDisabled = "rgba(248, 248, 242, 0.4)",
        }
    };

    #endregion
}
