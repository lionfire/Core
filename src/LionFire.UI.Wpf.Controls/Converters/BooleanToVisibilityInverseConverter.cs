// Retrieved from http://stackoverflow.com/questions/3128023/wpf-booleantovisibilityconverter-that-converts-to-hidden-instead-of-collapsed-wh
// on March 25, 2013.  Assume Public Domain licences

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LionFire.Avalon
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BooleanToVisibilityInverseConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public BooleanToVisibilityInverseConverter()
        {
            // set defaults
            TrueValue = Visibility.Collapsed;
            FalseValue = Visibility.Visible;
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                if (value == null) return FalseValue;
                else return TrueValue;
                //return null;
                //return FalseValue;
            }
            return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (Equals(value, TrueValue))
                return true;
            if (Equals(value, FalseValue))
                return false;
            return null;
        }
    }
}
