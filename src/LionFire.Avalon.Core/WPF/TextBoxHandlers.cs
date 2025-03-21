
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;

namespace LionFire.Avalon
{
    public static class TextBoxHandlers
    {
        public static void FocusOnEnter(this FrameworkElement fe, KeyEventArgs args)
        {
            if (args.Key == Key.Enter)
            {
                fe.Focus();
            }
            
        }

        //static TextBoxHandlers()
        //{
        //    DefocusCommand = new DelegateCommand<object>((obj) =>
        //        {
        //            MessageBox.Show("Got defocus command");
        //        });
        //}
        //public static ICommand DefocusCommand { get; set; }

        //private static readonly ILogger l = Log.Get();
		
    }
}
