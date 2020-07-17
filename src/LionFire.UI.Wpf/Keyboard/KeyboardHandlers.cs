
using System.Windows;
using System.Windows.Input;

namespace LionFire.Avalon
{
    public static class KeyboardHandlers
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
