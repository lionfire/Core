using LionFire.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace LionFire.UI.Commands
{
    // MOVE to LionFire.UI.Wpf dll

    public static class WpfCommandManager
    {
        public static void Initialize()
        {
            LionFireCommandManager.InvalidateRequerySuggested = CommandManager.InvalidateRequerySuggested;
            LionFireCommandManager.AddRequerySuggested = value => CommandManager.RequerySuggested += value;
            LionFireCommandManager.RemoveRequerySuggested = value => CommandManager.RequerySuggested -= value;
        }
    }
}
