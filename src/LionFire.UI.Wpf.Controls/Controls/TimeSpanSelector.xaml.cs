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
    /// Interaction logic for TimeSpanSelector.xaml
    /// </summary>
    public partial class TimeSpanSelector : UserControl, INotifyPropertyChanged
    {
        public TimeSpanSelector()
        {
            InitializeComponent();
            UpdateNegativeIcon();

            ColonText.MouseLeftButtonDown += ColonText_MouseLeftButtonDown;

            MinuteTextBox.SizeChanged += MinuteTextBox_SizeChanged;
        }

        #region Properties

        #region HourVisibility

        /// <summary>
        /// HourVisibility Dependency Property
        /// </summary>
        public static readonly DependencyProperty HourVisibilityProperty =
            DependencyProperty.Register("HourVisibility", typeof(Visibility), typeof(TimeSpanSelector),
                new FrameworkPropertyMetadata((Visibility)Visibility.Visible,
                    FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the HourVisibility property. This dependency property 
        /// indicates ....
        /// </summary>
        public Visibility HourVisibility
        {
            get { return (Visibility)GetValue(HourVisibilityProperty); }
            set { SetValue(HourVisibilityProperty, value); }
        }

        #endregion
        #region SecondVisibility

        /// <summary>
        /// SecondVisibility Dependency Property
        /// </summary>
        public static readonly DependencyProperty SecondVisibilityProperty =
            DependencyProperty.Register("SecondVisibility", typeof(Visibility), typeof(TimeSpanSelector),
                new FrameworkPropertyMetadata((Visibility)Visibility.Collapsed,
                    FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the SecondVisibility property. This dependency property 
        /// indicates ....
        /// </summary>
        public Visibility SecondVisibility
        {
            get { return (Visibility)GetValue(SecondVisibilityProperty); }
            set { SetValue(SecondVisibilityProperty, value); }
        }

        #endregion

        #endregion

        void MinuteTextBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MinuteTextBox.MinWidth < MinuteTextBox.ActualWidth)
            {
                MinuteTextBox.MinWidth = MinuteTextBox.ActualWidth;
            }
        }
        void SecondTextBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (SecondTextBox.MinWidth < SecondTextBox.ActualWidth)
            {
                SecondTextBox.MinWidth = SecondTextBox.ActualWidth;
            }
        }

        void ColonText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Value = TimeSpan.Zero;
        }

        private void UpdateNegativeIcon()
        {
            NegIcon.Visibility = IsNegative ? Visibility.Visible : Visibility.Hidden;
        }

        public bool IsNegative
        {
            get { return Value < TimeSpan.Zero; }
        }

        public string Hour
        {
            get
            {
                if (!Value.HasValue) return "";

                if (NegativeIconVisibility == System.Windows.Visibility.Visible)
                {
                    return Math.Abs(Value.Value.Hours).ToString();
                }
                else
                {
                    return Value.Value.Hours.ToString();
                }
            }
            set
            {
                int val;
                if (!Int32.TryParse(value, out val)) return;

                var v = Value.HasValue ? Value.Value : TimeSpan.Zero;
                Value = new TimeSpan(val, v.Minutes, v.Seconds);
            }
        }
        public string Minute
        {
            get
            {
                if (!Value.HasValue) return "";

                return Math.Abs(Value.Value.Minutes).ToString("00");
            }
            set
            {
                int val;
                if (!Int32.TryParse(value, out val)) return;

                var v = Value.HasValue?Value.Value : TimeSpan.Zero;
                Value = new TimeSpan(v.Hours, val, v.Seconds);
            }
        }
        public string Second
        {
            get
            {
                if (!Value.HasValue) return "";

                return Math.Abs(Value.Value.Seconds).ToString("00");
            }
            set
            {
                int val;
                if (!Int32.TryParse(value, out val)) return;

                var v = Value.HasValue ? Value.Value : TimeSpan.Zero;
                Value = new TimeSpan(v.Hours, v.Minutes, val);
            }
        }

        #region IsReadOnly

        /// <summary>
        /// IsReadOnly Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TimeSpanSelector),
                new FrameworkPropertyMetadata((bool)false,
                    FrameworkPropertyMetadataOptions.AffectsRender,
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
            TimeSpanSelector target = (TimeSpanSelector)d;
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

        #region NegativeIconVisibility

        /// <summary>
        /// NegativeIconVisibility Dependency Property
        /// </summary>
        public static readonly DependencyProperty NegativeIconVisibilityProperty =
            DependencyProperty.Register("NegativeIconVisibility", typeof(Visibility), typeof(TimeSpanSelector),
                new FrameworkPropertyMetadata((Visibility)Visibility.Visible,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the NegativeIconVisibility property. This dependency property 
        /// indicates ....
        /// </summary>
        public Visibility NegativeIconVisibility
        {
            get { return (Visibility)GetValue(NegativeIconVisibilityProperty); }
            set { SetValue(NegativeIconVisibilityProperty, value); }
        }

        #endregion

        #region CornerRadius

        /// <summary>
        /// CornerRadius Dependency Property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(TimeSpanSelector),
                new FrameworkPropertyMetadata((CornerRadius)new CornerRadius(4.0),
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the CornerRadius property. This dependency property 
        /// indicates ....
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        #endregion

        #region Value

        /// <summary>
        /// Value Dependency Property
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(TimeSpan?), typeof(TimeSpanSelector),
                new UIPropertyMetadata((TimeSpan?)default(TimeSpan),
                    new PropertyChangedCallback(OnValueChanged),
                    new CoerceValueCallback(CoerceValue)));

        /// <summary>
        /// Gets or sets the Value property. This dependency property 
        /// indicates ....
        /// </summary>
        public TimeSpan? Value
        {
            get { return (TimeSpan?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Value property.
        /// </summary>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeSpanSelector target = (TimeSpanSelector)d;
            TimeSpan? oldValue = (TimeSpan?)e.OldValue;
            TimeSpan? newValue = target.Value;
            target.OnValueChanged(oldValue, newValue);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Value property.
        /// </summary>
        protected virtual void OnValueChanged(TimeSpan? oldValue, TimeSpan? newValue)
        {
            UpdateNegativeIcon();
            RaiseValueChanged();

            OnPropertyChanged("Value");
        }

        /// <summary>
        /// Coerces the Value value.
        /// </summary>
        private static object CoerceValue(DependencyObject d, object value)
        {
            TimeSpanSelector target = (TimeSpanSelector)d;
            TimeSpan? desiredValue = (TimeSpan?)value;

            if (desiredValue >= TimeSpan.FromDays(1))
            {
                desiredValue = TimeSpan.FromDays(1) - TimeSpan.FromMinutes(1);
            }
            if (desiredValue <= TimeSpan.FromDays(-1))
            {
                desiredValue = TimeSpan.FromDays(-1) + TimeSpan.FromMinutes(1);
            }
            return desiredValue;
        }

        #endregion

        private void SecondTextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (IsReadOnly) return;
            
            e.Handled = true;

            int val;
            if (!Int32.TryParse(this.Second, out val))
            {
                if (e.Delta > 0)
                {
                    Second = "1";
                }
                else
                {
                    Second = "0";
                }
                return;
            }

            if (e.Delta > 0)
            {
                Value += TimeSpan.FromSeconds(1);
            }
            else
            {
                Value -= TimeSpan.FromSeconds(1);
            }
            SecondTextBox.FocusAncestorIfFocused();
        }
        private void MinuteTextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (IsReadOnly) return;
            e.Handled = true;

            int val;
            if (!Int32.TryParse(this.Minute, out val))
            {
                if (e.Delta > 0)
                {
                    Minute = "1";
                }
                else
                {
                    Minute = "0";
                }
                return;
            }

            if (e.Delta > 0)
            {
                Value += TimeSpan.FromMinutes(1);
            }
            else
            {
                Value -= TimeSpan.FromMinutes(1);
            }
            MinuteTextBox.FocusAncestorIfFocused();
        }

        private void HourTextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            
            if (IsReadOnly) return;
            e.Handled = true;
            int val;
            if (!Int32.TryParse(this.Hour, out val))
            {
                if (e.Delta > 0)
                {
                    Hour = "1";
                }
                else
                {
                    Hour = "0";
                }
                return;
            }
         
            if (e.Delta > 0)
            {
                Value += TimeSpan.FromHours(1);
            }
            else
            {
                Value -= TimeSpan.FromHours(1);
            }

            HourTextBox.FocusAncestorIfFocused();
        }

        void RaiseValueChanged()
        {
            OnPropertyChanged("Hour");
            OnPropertyChanged("Minute");
            OnPropertyChanged("Second");
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
