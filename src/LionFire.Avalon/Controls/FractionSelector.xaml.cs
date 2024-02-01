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
    /// <summary>
    /// Interaction logic for FractionSelector.xaml
    /// </summary>
    public partial class FractionSelector : UserControl, INotifyPropertyChanged
    {
        public FractionSelector()
        {
            InitializeComponent();

            FormatString = "0.#";
            Interval = 0.1;
            Min = 0;
            Max = 1;
            MinuteTextBox.MouseLeftButtonDown += ColonText_MouseLeftButtonDown;
        }

        void ColonText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Value == 1.0)
            {
                Value = 0;
            }
            else
            {
                Value = 1;
            }
        }

        public double Interval { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
        public string FormatString { get; set; }
        public string Minute
        {
            get
            {
                return Value.ToString(FormatString);
            }
            set
            {
                double val;
                if (!double.TryParse(value, out val)) return;

                Value = val;
            }
        }

        #region IsReadOnly

        /// <summary>
        /// IsReadOnly Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(FractionSelector),
                new FrameworkPropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnIsReadOnlyChanged)));

        /// <summary>
        /// Gets or sets the IsReadOnly property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Handles changes to the IsReadOnly property.
        /// </summary>
        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FractionSelector target = (FractionSelector)d;
            bool oldIsReadOnly = (bool)e.OldValue;
            bool newIsReadOnly = target.IsReadOnly;
            target.OnIsReadOnlyChanged(oldIsReadOnly, newIsReadOnly);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the IsReadOnly property.
        /// </summary>
        protected virtual void OnIsReadOnlyChanged(bool oldIsReadOnly, bool newIsReadOnly)
        {
        }

        #endregion

        #region Value

        /// <summary>
        /// Value Dependency Property
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(FractionSelector),
                new UIPropertyMetadata((double)default(double),
                    new PropertyChangedCallback(OnValueChanged),
                    new CoerceValueCallback(CoerceValue)));

        /// <summary>
        /// Gets or sets the Value property. This dependency property 
        /// indicates ....
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Value property.
        /// </summary>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FractionSelector target = (FractionSelector)d;
            double oldValue = (double)e.OldValue;
            double newValue = target.Value;
            target.OnValueChanged(oldValue, newValue);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Value property.
        /// </summary>
        protected virtual void OnValueChanged(double oldValue, double newValue)
        {
            Raise();
            OnPropertyChanged("Value");
        }

        /// <summary>
        /// Coerces the Value value.
        /// </summary>
        private static object CoerceValue(DependencyObject d, object value)
        {
            FractionSelector target = (FractionSelector)d;
            double desiredValue = (double)value;

            if (desiredValue >target.Max)
            {
                desiredValue = target.Max;
            }
            if (desiredValue < target.Min)
            {
                desiredValue = target.Min;
            }
            return desiredValue;
        }

        #endregion

        private void MinuteTextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //if (IsReadOnly) return;
            int val;
            if (!Int32.TryParse(this.Minute, out val))
            {
                Minute = "0";
                return;
            }

            var interval = Interval;
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                interval *= 10;
            }
            else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                interval /= 10;
            }            

            if (e.Delta > 0)
            {
                Value += interval;
            }
            else
            {
                Value -= interval;
            }
        }

        void Raise()
        {
            OnPropertyChanged("Minute");
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var ev = PropertyChanged;
            if (ev != null) ev(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
