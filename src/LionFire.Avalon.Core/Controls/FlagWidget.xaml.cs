using LionFire.Structures;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class MagnitudeConverter : IValueConverter
    {
        private static readonly ILogger l = Log.Get();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = value as string;
            if (String.IsNullOrWhiteSpace(s)) return value;

            try
            {
                var d = double.Parse(s);
                if (d == 0)
                {
                    return "";
                }
                //if (d == 1) return "";

                return value;

            }
            catch (Exception ex) { l.Warn(ex.ToString()); return value; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FlagVM
    {
        public Flag Flag { get; set; }
        public string DisplayName
        {
            get
            {
                if (Flag == null) return null;
                if (Flag.Name == "Billed") return "$";
                if (Flag.Name == "Non-billed") return "X$";
                if (Flag.Name == "Work") return "W";
                if (Flag.Name == "personal") return "Per";
                return Flag.Name;
            }
        }
        public FlagVM(string flagName) { this.Flag = flagName; }
        public FlagVM(Flag flag) { this.Flag = flag; }

    }
    public class FlagCollectionVM
    {
        public FlagCollection FlagCollection { get { return flagCollection; } }
        FlagCollection flagCollection;

        #region Construction

        public FlagCollectionVM() { }
        public FlagCollectionVM(FlagCollection flagCollection)
        { this.flagCollection = flagCollection; }

        #endregion

    }
    /// <summary>
    /// Interaction logic for FlagWidget.xaml
    /// </summary>
    public partial class FlagWidget : UserControl, INotifyPropertyChanged
    {
        #region Ontology

        public Flag Flag
        {
            get
            {
                var flag = DataContext as Flag;
                if (flag != null) return flag;

                if (FlagVM == null) return null;
                return FlagVM.Flag;
            }
        }

        public FlagVM FlagVM
        {
            get { return DataContext as FlagVM; }
            set { DataContext = value; }
        }

        public FlagCollectionVM FlagCollectionVM
        {
            get
            {
                return Tag as FlagCollectionVM;
            }
        }
        public FlagCollection FlagCollection
        {
            get
            {
                var fc = Tag as FlagCollection;
                if (fc != null) return fc;

                if (FlagCollectionVM != null)
                {
                    fc = FlagCollectionVM.FlagCollection;
                    if (fc != null) return fc;
                }

                DependencyObject dep = (DependencyObject)this;
                while ((dep != null) && !(dep is FlagsWrapPanel))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                if (dep != null)
                {
                    var wp = dep as FlagsWrapPanel;
                    if (wp != null) fc = wp.DataContext as FlagCollection;
                }
                return fc;
            }
        }

        #endregion

        #region Settings

        public static double DefaultMouseWheelInterval = 0.1;
        public double MouseWheelInterval { get { return DefaultMouseWheelInterval; } }

        #endregion

        #region Construction

        public FlagWidget()
        {
            InitializeComponent();

            this.MouseWheel += FlagWidget_MouseWheel;
            this.MouseUp += FlagWidget_MouseUp;
            this.DataContextChanged += FlagWidget_DataContextChanged;
        }

        void FlagWidget_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged("Flag");
            OnPropertyChanged("FlagVM");
            //l.Trace("FlagWidget DC: " + this.DataContext.ToTypeNameSafe());
            if (DataContext is Flag) { this.DataContext = new FlagVM((Flag)DataContext); return; }
        }
        private static readonly ILogger l = Log.Get();

        #endregion

        #region Event Handling

        void FlagWidget_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Toggle();
                e.Handled = true;
            }
            else if (e.ChangedButton == MouseButton.Middle)
            {
                Delete();
                e.Handled = true;
            }
        }

        void FlagWidget_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Flag == null) return;

            var delta = MouseWheelInterval;

            if (e.Delta < 0) delta *= -1;

            var mag = Flag.EffectiveMagnitude;

            mag += delta;
            mag = Math.Round(mag, 1);
            Flag.Magnitude = mag;
        }

        #endregion

        #region ToggleToMinusOne

        public bool ToggleToMinusOne
        {
            get { return toggleToMinusOne; }
            set { toggleToMinusOne = value; }
        } private bool toggleToMinusOne = false;

        #endregion

        #region ToggleToZero

        public bool ToggleToZero
        {
            get { return toggleToZero; }
            set { toggleToZero = value; }
        } private bool toggleToZero = true;

        #endregion

        #region ToggleToOne

        public bool ToggleToOne
        {
            get { return toggleToOne; }
            set { toggleToOne = value; }
        } private bool toggleToOne = true;

        #endregion


        public void Delete()
        {
            if (FlagCollection == null) return;
            if (Flag == null) return;
            FlagCollection.Remove(Flag);
        }

        public void Toggle()
        {

            if (Flag == null) return;

            if (Flag.EffectiveMagnitude >= 1.0)
            {
                if (ToggleToMinusOne)
                {
                    Flag.Magnitude = -1;
                }
                else
                {
                    if (ToggleToZero)
                    {
                        Flag.Magnitude = 0;
                    }
                    else
                    {
                    }
                }
            }
            else if (Flag.Magnitude < 0)
            {
                if (ToggleToZero)
                {
                    Flag.Magnitude = 0;
                }
                else
                {
                    if (ToggleToOne)
                    {
                        Flag.ClearMagnitude();
                    }
                }
            }
            else // if (Flag.Magnitude == 0)
            {
                if (ToggleToOne)
                {
                    Flag.ClearMagnitude();
                }
                else if (ToggleToMinusOne)
                {
                    Flag.Magnitude = -1;
                }
            }
        }

        #region Misc


        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var ev = PropertyChanged;
            if (ev != null) ev(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #endregion
    }
}
