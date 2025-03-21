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
    //public class DayTimeSelectorVM
    //{
    //    public DateTime MainDay { get; set; }

    //    //public DayTimeSelectorVM
    //    public DateTime? DateTime { get; set; }
    //}

    //public class DayTimeSelectorVMConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        if((!(value is Nullable<DateTime>)) return null;
            
    //        Nullable<DateTime> v = (Nullable<DateTime>)value;
    //        return new DayTimeSelectorVM(v);
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        if((!(value is DayTimeSelectorVM)) return null;

    //        DayTimeSelectorVM v = (DayTimeSelectorVM)value;
    //        return v.DateTime;
    //    }
    //}

    /// <summary>
    /// Interaction logic for DayTimeSelector.xaml
    /// </summary>
    public partial class DayTimeSelector : UserControl, INotifyPropertyChanged
    {
        public DayTimeSelector()
        {
            InitializeComponent();
            
            //DayTimeSelectorVMConverter conv = (DayTimeSelectorVMConverter)this.FindResource("DayTimeSelectorVMConverter");

            UpdatePrevNextIndicator();
            ColonText.MouseLeftButtonDown += ColonText_MouseLeftButtonDown;
        }

        void ColonText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Value = DateTime.Now;
        }

        #region CornerRadius

        /// <summary>
        /// CornerRadius Dependency Property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(DayTimeSelector),
                new FrameworkPropertyMetadata((CornerRadius)new CornerRadius(),
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

        #region IsReadOnly

        /// <summary>
        /// IsReadOnly Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(DayTimeSelector),
                new FrameworkPropertyMetadata((bool)false,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the IsReadOnly property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        #endregion


        #region HourVisibility

        /// <summary>
        /// HourVisibility Dependency Property
        /// </summary>
        public static readonly DependencyProperty HourVisibilityProperty =
            DependencyProperty.Register("HourVisibility", typeof(Visibility), typeof(DayTimeSelector),
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
            DependencyProperty.Register("SecondVisibility", typeof(Visibility), typeof(DayTimeSelector),
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


        private void UpdatePrevNextIndicator()
        {
            
            if (Value.HasValue)
            {
                if (Value.Value.Year == MainDay.Year
                    && Value.Value.Month == MainDay.Month
                    && Value.Value.Day == MainDay.Day)
                {
                    PrevIcon.Visibility = Visibility.Hidden;
                    NextIcon.Visibility = Visibility.Hidden;
                }
                else
                {
                    if (Value.Value > MainDay)
                    {
                        PrevIcon.Visibility = Visibility.Hidden;
                        NextIcon.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        PrevIcon.Visibility = Visibility.Visible;
                        NextIcon.Visibility = Visibility.Hidden;
                    }
                }

            }
            else
            {
                PrevIcon.Visibility = Visibility.Hidden;
                NextIcon.Visibility = Visibility.Hidden;

            }
        }

        #region MainDay

        /// <summary>
        /// MainDay Dependency Property
        /// </summary>
        public static readonly DependencyProperty MainDayProperty =
            DependencyProperty.Register("MainDay", typeof(DateTime), typeof(DayTimeSelector),
                new FrameworkPropertyMetadata((DateTime)default(DateTime)));

        /// <summary>
        /// Gets or sets the MainDay property. This dependency property 
        /// indicates ....
        /// </summary>
        public DateTime MainDay
        {
            get { return (DateTime)GetValue(MainDayProperty); }
            set { SetValue(MainDayProperty, value); }
        }

        #endregion

        public string Hour
        {
            get
            {
                return Value.HasValue ? Value.Value.Hour.ToString() : "";
            }
            set
            {
                int val;
                if (!Int32.TryParse(value, out val)) return;
                
                if (Value.HasValue)
                {
                    var v = Value.Value;
                    Value = new DateTime(v.Year, v.Month, v.Day, val, v.Minute, v.Second);
                }
                else
                {
                    var v = MainDay == default(DateTime) ? DateTime.Now : MainDay;
                    Value = new DateTime(v.Year, v.Month, v.Day, val, v.Minute, v.Second);
                }                
            }
        }
        
        public string Minute
        {
            get
            {
                return Value.HasValue ? Value.Value.Minute.ToString("00") : "";
            }
            set
            {
                int val;
                if (!Int32.TryParse(value, out val)) return;

                if (Value.HasValue)
                {
                    var v = Value.Value;
                    Value = new DateTime(v.Year, v.Month, v.Day, v.Hour, val, v.Second);
                }
                else
                {
                    var v = MainDay == default(DateTime) ? DateTime.Now : MainDay;
                    Value = new DateTime(v.Year, v.Month, v.Day, v.Hour, val, v.Second);
                }
            }
        }

        public string Second
        {
            get
            {
                return Value.HasValue ? Value.Value.Second.ToString("00") : "";
            }
            set
            {
                int val;
                if (!Int32.TryParse(value, out val)) return;

                if (Value.HasValue)
                {
                    var v = Value.Value;
                    Value = new DateTime(v.Year, v.Month, v.Day, v.Hour, v.Minute, val);
                }
                else
                {
                    var v = MainDay == default(DateTime) ? DateTime.Now : MainDay;
                    Value = new DateTime(v.Year, v.Month, v.Day, v.Hour, v.Minute, val);
                }
            }
        }


        #region Value

        /// <summary>
        /// Value Dependency Property
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(DateTime?), typeof(DayTimeSelector),
                new UIPropertyMetadata((DateTime?)null,
                    new PropertyChangedCallback(OnValueChanged),
                    new CoerceValueCallback(CoerceValue)));

        /// <summary>
        /// Gets or sets the Value property. This dependency property 
        /// indicates ....
        /// </summary>
        public DateTime? Value
        {
            get { return (DateTime?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Value property.
        /// </summary>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DayTimeSelector target = (DayTimeSelector)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = target.Value;
            target.OnValueChanged(oldValue, newValue);
            
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Value property.
        /// </summary>
        protected virtual void OnValueChanged(DateTime? oldValue, DateTime? newValue)
        {
            UpdatePrevNextIndicator();
            Raise();
            OnPropertyChanged("Value");
        }

        /// <summary>
        /// Coerces the Value value.
        /// </summary>
        private static object CoerceValue(DependencyObject d, object value)
        {
            DayTimeSelector target = (DayTimeSelector)d;
            DateTime? desiredValue = (DateTime?)value;

            if (target.MainDay != default(DateTime) && desiredValue.HasValue)
            {
                var dv = desiredValue.Value;
                var m = target.MainDay;

                var mPrev = m - TimeSpan.FromDays(1);
                mPrev = new DateTime(mPrev.Year, mPrev.Month, mPrev.Day);
                
                var mNext = m + TimeSpan.FromDays(1);
                mNext = new DateTime(mNext.Year, mNext.Month, mNext.Day, 23, 59, 59);

                if (dv < mPrev)
                {
                    desiredValue = mPrev;
                }
                if (dv > mNext)
                {
                    desiredValue = mNext;
                }
            }

            return desiredValue;
        }

        #endregion

        private void SecondTextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int val;
            if (!Int32.TryParse(this.Minute, out val))
            {
                //val = 0;
                Second = "0";
                return;
            }
            if (!Value.HasValue)
            {
                Value = DateTime.Now;
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
        }
        private void MinuteTextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int val;
            if (!Int32.TryParse(this.Minute, out val))
            {
                //val = 0;
                Minute = "0";
                return;
            }
            if(!Value.HasValue)
            {
                Value = DateTime.Now;
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
        }

        private void HourTextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int val;
            if (!Int32.TryParse(this.Hour, out val))
            {
                //val = 0;
                Hour = "0";
                return;
            }
            if (!Value.HasValue)
            {
                Value = DateTime.Now;
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
        }

        void Raise()
        {
            OnPropertyChanged("Hour");
            OnPropertyChanged("Minute");
            //OnPropertyChanged("Value");
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
