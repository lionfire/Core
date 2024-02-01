using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LionFire.Avalon
{
    public class DesignerUtils
    {
        public static bool IsInDesignMode {
            get {
                // Alternative approach: LicenseManager.UsageMode == LicenseUsageMode.Runtime
                var result = (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty,
                typeof(DependencyObject)).Metadata.DefaultValue;
                return result;
            }
        }
    }
}
