using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace LionFire.Avalon
{

    public class ColorChanger : DependencyObject, IValueConverter
    {
        #region Color

        /// <summary>
        /// Color Dependency Property
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorChanger),
                new FrameworkPropertyMetadata((Color)Colors.White,
                    new PropertyChangedCallback(OnColorChanged)));

        /// <summary>
        /// Gets or sets the Color property. This dependency property 
        /// indicates ....
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Color property.
        /// </summary>
        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorChanger target = (ColorChanger)d;
            Color oldColor = (Color)e.OldValue;
            Color newColor = target.Color;
            target.OnColorChanged(oldColor, newColor);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Color property.
        /// </summary>
        protected virtual void OnColorChanged(Color oldColor, Color newColor)
        {
        }

        #endregion



        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ColorShift : MarkupExtension, IValueConverter
    {
        public ColorShift()
        {
            //AddHue = 0;
            MulHue = 1;
            MulSaturation = 1;
            MulValue = 1;
            
        }

        public Color Color { get; set; }
        public ColorShift ColorShiftX { get; set; }

        public double AddHue { get; set; }
        public double AddSaturation { get; set; }
        public double AddValue { get; set; }

        public double MulHue { get; set; }
        public double MulSaturation { get; set; }
        public double MulValue { get; set; }

        //#region MulValue

        ///// <summary>
        ///// MulValue Dependency Property
        ///// </summary>
        //public static readonly DependencyProperty MulValueProperty =
        //    DependencyProperty.Register("MulValue", typeof(double), typeof(ColorShift),
        //        new FrameworkPropertyMetadata((double)1.0));

        ///// <summary>
        ///// Gets or sets the MulValue property. This dependency property 
        ///// indicates ....
        ///// </summary>
        //public double MulValue
        //{
        //    get { return (double)GetValue(MulValueProperty); }
        //    set { SetValue(MulValueProperty, value); }
        //}

        //#endregion

        private Color Convert(Color color)
        {
            //color.R = (byte)(AddValue + (byte)(MulValue * color.R));
            //color.G = (byte)(AddValue + (byte)(MulValue * color.G));
            //color.B = (byte)(AddValue + (byte)(MulValue * color.B));

            var hsv = color.ToHsb();

            hsv.V =
                //AddValue +
                (MulValue * hsv.V);
            //hsv.H = AddHue + (MulHue * hsv.H);
            //hsv.S = AddSaturation + (MulSaturation * hsv.S);

            color = hsv.ToRgb();
            return color;
            //return this;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Convert(Color);
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Color color = Color;

            //color.R = (byte) (AddValue + (byte)(MulValue * color.R));
            //color.G =(byte) ( AddValue + (byte)(MulValue * color.G));
            //color.B =(byte) ( AddValue + (byte)(MulValue * color.B));

            //return color;
            //return Convert(Color);
            return Convert((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LionColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(!(value is Color))return null;

            Color color = (Color)value;
            if (color == null) return value;

            ColorShift cs = parameter as ColorShift;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
