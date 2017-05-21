using System;

namespace LionFire.Notifications
{
    public interface IAlert
    {
        bool IsVisible { get; set; }

        string Message { get; set; }
        string Detail { get; set; }

    }

    [Flags]
    public enum AlertCapabilities
    {
        None,
        Visual = 1 << 0,
        Sound = 1 << 1,
    }

    public interface IAlertService
    {
        AlertCapabilities Capabilities { get; }

        void Show(IAlert alert);
        void Hide(IAlert alert);

    }
    public static class IAlertServiceExtensions
    {
        public static bool? CanAlert(this IAlertService service, AlertCapabilities capabilities)
        {
            throw new NotImplementedException();
        }
    }
}

