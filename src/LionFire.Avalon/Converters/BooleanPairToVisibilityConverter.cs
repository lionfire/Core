// Retrieved from http://stackoverflow.com/questions/2946048/adornedelement-properties-in-a-trigger on June 26, 2012
// Assume Public Domain license

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace LionFire.Avalon
{
    public class BooleanPairToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (2 != values.Length) throw new ArgumentException("values");
            return ((bool)values[0] || (bool)values[1]) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotSupportedException(); }
    }
}
