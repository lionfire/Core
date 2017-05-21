using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LionFire.Notifications
{
    /// <summary>
    /// Interaction logic for PopupAlertView.xaml
    /// </summary>
    public partial class PopupAlertView : UserControl
    {
        public PopupAlertView()
        {
            InitializeComponent();
            //this.Loaded += PopupAlertView_Loaded;

            //Task.Run(async () =>
            //{
            //    await Task.Delay(5000);
            //    Execute.BeginOnUIThread(() =>
            //    {
            //        var w = Window.GetWindow(this);
            //        w.Left = 500;
            //        w.Top = 0;
            //        w.Topmost = true;
            //    });
            //});
        }

        //private void PopupAlertView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var w = Window.GetWindow(this);
        //    w.Left = 500;
        //    w.Top = 0;
        //    w.Topmost = true;
        //}
    }
}
