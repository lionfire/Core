using LionFire.Shell;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace LionFire.UI
{

    public class WpfWindowManager
    {

        #region Parent

        public WpfNavigator WpfNavigator { get; }
        public IOptionsMonitor<UIOptions> UIOptionsMonitor { get; }

        #endregion

        #region Construction

        public WpfWindowManager(WpfNavigator wpfNavigator, IOptionsMonitor<UIOptions> uiOptionsMonitor)
        {
            WpfNavigator = wpfNavigator;
            UIOptionsMonitor = uiOptionsMonitor;
        }

        #endregion


        public virtual Size DefaultWindowedSize
        {
            get
            {
                switch (UIOptionsMonitor.CurrentValue.DisplayKind)
                {
                    case DisplayKind.Unspecified:
                    case DisplayKind.PC:
                    default:
                        return new Size(1368, 768);
                    case DisplayKind.Tablet:
                    case DisplayKind.Phone:
                    case DisplayKind.Television:
                        return new Size(1024, 768);
                }
            }
        }

        internal Window GetOrCreateWindow(string key)
        {

        }
    }
    
}
