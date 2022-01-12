using MudBlazor;
using System;

namespace LionFire.Blazor.MudBlazor_
{
    public static class Theme
    {
        public static MudTheme Current
        {
            get => current; set
            {
                current = value;
                CurrentChanged?.Invoke();
            }
        }
        private static MudTheme current = Dark;

        public static event Action CurrentChanged;
        public static MudTheme CurrentDark { get; set; } = Dark;
        public static MudTheme CurrentLight { get; set; } = new MudTheme();

        public static MudTheme Default { get; set; } = new MudTheme()
        {
            Palette = new Palette()
            {
                Black = "#272c34"
            }
        };


        public static void ToggleDarkMode()
        {
            if (Current == CurrentDark)
            {
                Current = CurrentLight;
            }
            else
            {
                Current = CurrentDark;
            }
        }

        public static MudTheme Dark => new()
        {
            Palette = new Palette()
            {
                Black = "#27272f",
                Background = "#32333d",
                BackgroundGrey = "#27272f",
                Surface = "#373740",
                DrawerBackground = "#27272f",
                DrawerText = "rgba(255,255,255, 0.50)",
                DrawerIcon = "rgba(255,255,255, 0.50)",
                AppbarBackground = "#27272f",
                AppbarText = "rgba(255,255,255, 0.70)",
                TextPrimary = "rgba(255,255,255, 0.70)",
                TextSecondary = "rgba(255,255,255, 0.50)",
                ActionDefault = "#adadb1",
                ActionDisabled = "rgba(255,255,255, 0.26)",
                ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                Divider = "rgba(255,255,255, 0.12)",
                DividerLight = "rgba(255,255,255, 0.06)",
                TableLines = "rgba(255,255,255, 0.12)",
                LinesDefault = "rgba(255,255,255, 0.12)",
                LinesInputs = "rgba(255,255,255, 0.3)",
                TextDisabled = "rgba(255,255,255, 0.2)"
            }
        };
    }
}
