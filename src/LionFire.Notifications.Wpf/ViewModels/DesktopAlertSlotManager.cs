using System.Windows.Input;

namespace LionFire.Notifications
{
    public class DesktopAlertSlotManager : IDesktopAlertSlotManager
    {
        public ModifierKeys DismissModifiers { get; set; }
        public ModifierKeys SnoozeModifiers { get; set; } = ModifierKeys.Shift;
        public ModifierKeys CommandModifiers { get; set; } = ModifierKeys.Control;
        public ModifierKeys AltCommandModifiers { get; set; } = ModifierKeys.Alt;

        public void TrySetSlot(IAlertViewModel alert)
        {
            alert.Slot = 1;
            alert.HotKey = Key.F1;
        }
    }
}
