using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LionFire.Avalon
{
    public class CaptionProviderConverter : IValueConverter
    {
        //{Binding Converter={lfa:CaptionProvider}}

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value==null) return "null";

            return value.GetType().Name.ToDisplayString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
