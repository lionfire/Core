﻿// Retrieved from http://www.codeproject.com/Articles/49650/WPF-Color-Conversions
// on March 30, 2013 under CPOL license

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace System.Windows.Media
{
    public struct HsvColor
    {
        public double A;
        public double H;
        public double S;
        public double V;
    }

    public static class ColorExtensions
    {

        #region RGB-HSB Conversion

        /// <summary>
        /// Converts an RGB color to an HSB color.
        /// </summary>
        /// <param name="rgbColor">The RGB color to convert.</param>
        /// <returns>The HSB color equivalent of the RGBA color passed in.</returns>
        /// <remarks>Source: http://msdn.microsoft.com/en-us/library/ms771620.aspx</remarks>
        public static HsvColor ToHsb(this Color rgbColor)
        {
            /* Hue values range between 0 and 360. All 
             * other values range between 0 and 1. */

            // Create HSB color object
            var hsbColor = new HsvColor();

            // Get RGB color component values
            var r = (int)rgbColor.R;
            var g = (int)rgbColor.G;
            var b = (int)rgbColor.B;
            var a = (int)rgbColor.A;

            // Get min, max, and delta values
            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);
            double delta = max - min;

            /* Black (max = 0) is a special case. We 
             * simply set HSB values to zero and exit. */

            // Black: Set HSB and return
            if (max == 0.0)
            {
                hsbColor.H = 0.0;
                hsbColor.S = 0.0;
                hsbColor.V = 0.0;
                hsbColor.A = a;
                return hsbColor;
            }

            /* Now we process the normal case. */

            // Set HSB Alpha value
            var alpha = (double)a;
            hsbColor.A = alpha / 255;

            // Set HSB Hue value
            if (r == max) hsbColor.H = (g - b) / delta;
            else if (g == max) hsbColor.H = 2 + (b - r) / delta;
            else if (b == max) hsbColor.H = 4 + (r - g) / delta;
            hsbColor.H *= 60;
            if (hsbColor.H < 0.0) hsbColor.H += 360;

            // Set other HSB values
            hsbColor.S = delta / max;
            hsbColor.V = max / 255;

            // Set return value
            return hsbColor;
        }

        /// <summary>
        /// Converts an HSB color to an RGB color.
        /// </summary>
        /// <param name="hsbColor">The HSB color to convert.</param>
        /// <returns>The RGB color equivalent of the HSB color passed in.</returns>
        /// Source: http://msdn.microsoft.com/en-us/library/ms771620.aspx
        public static Color ToRgb(this HsvColor hsbColor)
        {
            // Initialize
            var rgbColor = new Color();

            /* Gray (zero saturation) is a special case.We simply
             * set RGB values to HSB Brightness value and exit. */

            // Gray: Set RGB and return
            if (hsbColor.S == 0.0)
            {
                rgbColor.A = (byte)(hsbColor.A * 255);
                rgbColor.R = (byte)(hsbColor.V * 255);
                rgbColor.G = (byte)(hsbColor.V * 255);
                rgbColor.B = (byte)(hsbColor.V * 255);
                return rgbColor;
            }

            /* Now we process the normal case. */

            var h = (hsbColor.H == 360) ? 0 : hsbColor.H / 60;
            var i = (int)(Math.Truncate(h));
            var f = h - i;

            var p = hsbColor.V * (1.0 - hsbColor.S);
            var q = hsbColor.V * (1.0 - (hsbColor.S * f));
            var t = hsbColor.V * (1.0 - (hsbColor.S * (1.0 - f)));

            double r, g, b;
            switch (i)
            {
                case 0:
                    r = hsbColor.V;
                    g = t;
                    b = p;
                    break;

                case 1:
                    r = q;
                    g = hsbColor.V;
                    b = p;
                    break;

                case 2:
                    r = p;
                    g = hsbColor.V;
                    b = t;
                    break;

                case 3:
                    r = p;
                    g = q;
                    b = hsbColor.V;
                    break;

                case 4:
                    r = t;
                    g = p;
                    b = hsbColor.V;
                    break;

                default:
                    r = hsbColor.V;
                    g = p;
                    b = q;
                    break;
            }

            // Set WPF Color object
            rgbColor.A = (byte)(hsbColor.A * 255);
            rgbColor.R = (byte)(r * 255);
            rgbColor.G = (byte)(g * 255);
            rgbColor.B = (byte)(b * 255);

            // Set return value
            return rgbColor;
        }

        #endregion
    
      
    }
}

namespace LionFire.Avalon
{
    [ValueConversion(typeof(string), typeof(string))]
    public class HsvValueConverter : IValueConverter
    {
    
        #region IValueConverter Members

        /// <summary>
        /// Adjusts an RGB color by a specified percentage.
        /// </summary>
        /// <param name="value">The hex representation of the RGB color to adjust.</param>
        /// <param name="targetType">WPF Type.</param>
        /// <param name="parameter">The percentage by which the color should be adjusted, as a decimal.</param>
        /// <param name="culture">WPF CultureInfo.</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Validate params
            if (value == null) throw new ArgumentNullException("value");
            if (parameter == null) throw new ArgumentNullException("parameter");

            // Get HSB values of color passed in
            var brush = (SolidColorBrush)value;
            var rgbColorIn = brush.Color;
            var hsvColor = rgbColorIn.ToHsb();

            // Adjust color by factor passed in
            var brightnessAdjustment = Double.Parse((parameter.ToString()));
            hsvColor.V *= brightnessAdjustment;

            // Return result
            var rgbColorOut = hsvColor.ToRgb();
            var brushOut = new SolidColorBrush();
            brushOut.Color = rgbColorOut;
            return brushOut;
        }

        /// <summary>
        /// Not implemented in this converter; will throw an exception if called.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}