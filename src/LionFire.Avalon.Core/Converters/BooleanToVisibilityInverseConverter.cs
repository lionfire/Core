using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace LionFire.Avalon
{
    public class BooleanToVisibilityInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool boolValue;

            if(!(value is Boolean)) boolValue = false;
            else boolValue = (bool)value;
            
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility visibility;

            if (!(value is Visibility)) visibility = Visibility.Collapsed;
            else visibility = (Visibility)value;
            return visibility == Visibility.Visible ? false : true;
        }
    }
}
