using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace LionFire.UI.Commands
{
    public static class LionFireCommandManager
    {
        public static event EventHandler RequerySuggested
        {
            add => AddRequerySuggested?.Invoke(value);
            remove => RemoveRequerySuggested?.Invoke(value);
        }

        public static Action InvalidateRequerySuggested { get; set; } = () => { };

        public static Action<EventHandler> AddRequerySuggested { get; set; }
        public static Action<EventHandler> RemoveRequerySuggested { get; set; }
    }
 

}
