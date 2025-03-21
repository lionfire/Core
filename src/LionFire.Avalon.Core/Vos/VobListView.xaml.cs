using Microsoft.Extensions.Logging;
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

namespace LionFire.Avalon.Vos
{
    /// <summary>
    /// Interaction logic for VobListView.xaml
    /// </summary>
    public partial class VobListView : UserControl
    {
        public ListView ListView { get { return this.listView;  } }

        public VobListView()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            l.Info("VobListView.TextBlock_MouseDown_1");
        }

        #region Misc

        private static readonly ILogger l = Log.Get();
		
        #endregion
    }
}
