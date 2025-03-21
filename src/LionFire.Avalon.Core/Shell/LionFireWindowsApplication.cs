using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LionFire.Avalon
{
    public class LionFireWindowsApplication : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (!Dispatcher.CheckAccess()) { Dispatcher.BeginInvoke(new Action(() => OnStartup(e))); return; }

            BeforeOnStartup(e); // Creates LionFireApp
            base.OnStartup(e); // Raises the Startup event
            AfterOnStartup(e);
        }

        public Action<StartupEventArgs> BeforeOnStartup;
        public Action<StartupEventArgs> AfterOnStartup;
    }
}
