using System;

namespace LionFire.Events;

public static class CommandManager
{
    //public static void RaiseRequerySuggested(object sender, EventArgs e)
    //{
    //    var ev = RequerySuggested;
    //    if(ev!=null) ev(sender, e);
    //}

    public static event EventHandler RequerySuggested { add {
            if (AddRequerySuggested != null) AddRequerySuggested(value);
        }
        remove {
            if (RemoveRequerySuggested != null) RemoveRequerySuggested(value);
        }
    }

    public static Action<EventHandler> AddRequerySuggested = null;
    public static Action<EventHandler> RemoveRequerySuggested = null;

}
