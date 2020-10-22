namespace LionFire.UI
{
    public interface IDisplayInfo
    {
        DisplayKind DisplayKind { get; }
    }

    public class UIOptions
    {
        public DisplayKind DisplayKind { get; set; } = DisplayKind.PC;
    }
}

