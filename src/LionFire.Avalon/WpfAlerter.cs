using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LionFire.Alerting
{

    public class WpfAlerter : IAlerter
    {
        public string DefaultTitle = "Notice";
        public void Alert(Alert alert)
        {
            if (alert.Flags.HasFlag(AlertFlags.TextEntry)) { System.Windows.MessageBox.Show("TODO: dialog with text entry."); }

            System.Windows.MessageBox.Show(
                alert.Message
                + (alert.Detail == null ? "" : alert.Detail.ToString() + Environment.NewLine) 
                + (alert.Exception == null ? "":alert.Exception.ToString()),
                (alert.Title ?? (Char.IsLetter(alert.LogLevel.ToString()[0]) ? alert.LogLevel.ToString() : DefaultTitle)), MessageBoxButton.OK
            );
            
        }

     
    }
}
