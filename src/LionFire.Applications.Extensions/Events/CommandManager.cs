using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Events
{
    // OLD - Replaced with LionFire.UI's LionFireCommandManager.  Or should that one be replaced with this one?  Or LionFire.Commands?

    // REVIEW - what is the purpose, and should it be DI-oriented rather than static?
    public static class ApplicationCommandManager
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
}
