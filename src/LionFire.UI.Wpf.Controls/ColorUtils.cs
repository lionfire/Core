using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if NOESIS
using Noesis;
#else
using System.Windows.Media;
#endif

namespace LionFire.Extensions.Colors
{

    public static class ColorUtils
    {
        private static byte Lerp(this byte v1, byte v2, double progress)
        {
            if (progress < 0.0 || progress > 1.0) throw new ArgumentOutOfRangeException("progress must be between 0 and 1");

            if (progress == 1.0)
            {
                return v2;
            }
            if (progress == 0.0)
            {
                return v1;
            }
            return (byte)(v1 * progress + v2 * (1.0 - progress));
        }

#region Brightness

        public static Color ToValue(this Color color, double val, double weight = 1.0)
        {
            return color.ToValue((byte)(val * 255.0), weight);
        }

        public static Color ToValue(this Color color, byte val, double weight = 1.0)
        {
            return new Color()
            {
                A = color.A,
                R = color.R.Lerp(val, weight),
                G = color.G.Lerp(val, weight),
                B = color.B.Lerp(val, weight),
                //B = (byte)(color.B * val),
            };
        }

        public static Color ModulateBrightness(this Color color, double val)
        {
            return new Color()
            {
                A = color.A,
                //R = color.R.Lerp(val, weight),
                //G = color.G.Lerp(val, weight),
                //B = color.B.Lerp(val, weight),
                R = (byte)(color.R * val),
                G = (byte)(color.G * val),
                B = (byte)(color.B * val),
            };
        }

#endregion

#region Alpha

        public static Color ToAlpha(this Color color, double val, double weight = 1.0)
        {
            return color.ToAlpha((byte)(val * 255.0), weight);
        }
        public static Color ToAlpha(this Color color, byte val, double weight = 1.0)
        {
            return new Color()
            {
                A = color.A.Lerp(val, weight),
                //A = (byte)(color.A * val),
                R = color.R,
                G = color.G,
                B = color.B,
            };
        }

#endregion

    }

}
