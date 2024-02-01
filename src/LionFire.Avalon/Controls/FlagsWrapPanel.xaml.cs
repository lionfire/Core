using LionFire.Structures;
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

namespace LionFire.Avalon
{
    /// <summary>
    /// Interaction logic for FlagsWrapPanel.xaml
    /// </summary>
    public partial class FlagsWrapPanel : UserControl
    {

        #region Ontology

        public FlagCollectionVM FlagCollectionVM
        {
            get;
            set;
        }

        public FlagCollection FlagCollection
        {
            get { 
                return DataContext as FlagCollection; }
        }

        #endregion

        public List<string> TestCommonFlags = new List<string>
        {
            "work",
            "personal",
            "prayer",
            "self-improvement",
            "finance",
            "personal",
            "business",
        };

        public FlagsWrapPanel()
        {
            InitializeComponent();
            
            NewTextBox.ItemsSource = TestCommonFlags;
            NewTextBox.KeyDown += NewBox_KeyDown;
            NewTextBox.TextChanged += NewTextBox_TextChanged;
            NewTextBox.GotFocus += NewTextBox_GotFocus;
            NewTextBox.LostFocus += NewTextBox_LostFocus;
            NewTextBox.LostKeyboardFocus += NewTextBox_LostKeyboardFocus;
            NewTextBox.GotKeyboardFocus += NewTextBox_GotKeyboardFocus;
            
            this.DataContextChanged += FlagsWrapPanel_DataContextChanged;
            this.MouseUp += OnMouseUp;
        }

        void NewTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            UpdateWatermark();
        }

        void NewTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            UpdateWatermark();
        }

        void NewTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateWatermark();
        }

        private void UpdateWatermark()
        {
            WatermarkText.Visibility =  (!NewTextBox.IsKeyboardFocused && String.IsNullOrEmpty(NewTextBox.Text)) ? Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        //void uc_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    NewTextBox.Focus();
        //}
        void NewTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            UpdateWatermark();
        }

        void NewTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWatermark();
            
        }

        void FlagsWrapPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (FlagCollectionVM != null)
            {
                if (FlagCollectionVM.FlagCollection == DataContext)
                {
                    l.Warn("UNTESTED - set to same FlagCollection.  Using same VM");
                    return;
                }
                else
                {
                    l.Warn("UNTESTED - changing FlagCollectionVM");
                }
            }

            var fc = FlagCollection;

            if (DataContext == null || fc == null)
            {
                l.Trace("UNTESTED - set to null");
                FlagCollectionVM = null;
                this.ItemsControl.ItemsSource = null;
                return;
            }

            FlagCollectionVM = new FlagCollectionVM(fc);

            //l.Trace("DC: " + this.DataContext.ToStringSafe());

            //this.ItemsControl.ItemsSource = DataContext as System.Collections.IEnumerable;

            var cvs = (CollectionViewSource)this.FindResource("cvs");
            cvs.Source = FlagCollection;
            
            this.ItemsControl.ItemsSource = cvs.View;  // FlagCollection;
        }

        private void NewBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key==Key.Return)
            {
                
                //l.Trace("NewBox_KeyDown " + NewTextBox.Text);

                if (!String.IsNullOrWhiteSpace(NewTextBox.Text))
                {
                    e.Handled = true;

                    if (FlagCollection == null)
                    {
                        l.Warn("Key down: but FlagCollection == null");
                        return;
                    }

                    Flag flag;
                    if (Flag.TryParse(NewTextBox.Text, out flag))
                    {
                        NewTextBox.Text = "";
                        FlagCollection.SetFlag(flag);
                    }
                }
            }
        }

        private void NewBox_Selected(object sender, RoutedEventArgs e)
        {
            //ComboBox a;

            l.Trace("NewBox_Selected " + NewBox.SelectedItem.ToStringSafe());
        }

        void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {

                DependencyObject dep = (DependencyObject)e.OriginalSource;
                while ((dep != null) && !(dep is FlagWidget))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                if (dep == null) return;
                var flagWidget = dep as FlagWidget;
                if (flagWidget == null) return;

                var flag = flagWidget.Flag;
                if (flag == null) return;

                flagWidget.Toggle();
            }
            if (e.ChangedButton == MouseButton.Middle)
            {
                DependencyObject dep = (DependencyObject)e.OriginalSource;
                while ((dep != null) && !(dep is FlagWidget))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                if (dep == null) return;
                var flagWidget = dep as FlagWidget;
                if (flagWidget == null) return;

                var flag = flagWidget.Flag;
                if (flag == null) return;

                FlagCollection.Remove(flag);
            }
        }

        private static readonly ILogger l = Log.Get();

        public event FilterEventHandler Filter;

        //public Func<object, FilterEventArgs, bool> FilterMethod { get; set; }
        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            var ev = Filter;
            if (ev != null) ev(sender, e);
            
            //if (FilterMethod == null) e.Accepted = true;
            //else FilterMethod(sender, e);
        }

        //private void NewBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    e.Handled = false;
        //}

        //private void NewBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    e.Handled = false;
        //}

        //private void NewBox_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    e.Handled = false;
        //}

        //private void NewBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    e.Handled = false;
        //}

    }
}
