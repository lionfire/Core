using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Globalization;


namespace LionFire.Avalon.Tree
{
    /// <summary>
    /// Convert Level to left margin
    /// </summary>
	public class LevelToIndentConverter : IValueConverter
    {
        //public static double IndentSize = 19.0;
        public static double IndentSize = 10.0;
		
		public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            return new Thickness((int)o * IndentSize, 0, 0, 0);
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class CanExpandConverter : IValueConverter
	{
		public object Convert(object o, Type type, object parameter, CultureInfo culture)
		{
			if ((bool)o)
				return Visibility.Visible;
			else
				return Visibility.Hidden;
		}

		public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
