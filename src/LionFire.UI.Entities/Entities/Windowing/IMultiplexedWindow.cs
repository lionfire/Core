namespace LionFire.UI.Entities
{
    public interface IMultiplexedWindow
    {
        IWindow FullScreenWindow { get; }
        IWindow WindowedWindow { get; }

        IWindow CurrentWindow { get; }

        bool IsFullScreen { get; set; }
    }
}
