using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace LionFire.Avalon
{
    public class LionColor2 : System.Windows.DynamicResourceExtension
    {
        #region Construction

        public LionColor2() : base("TestLionColor2Key"){ }
        public LionColor2(Color color)
            : base("TestLionColor2Key")
        {
            this.Color = color;
        }

        #endregion

        [ConstructorArgument("color")]
        public Color Color { get; set; }
        
        #region V

        public float V
        {
            get
            {
                return v;
            }
            set
            {
                if (value > 2) value = 2;
                v = value; Invalidate();
            }
        } private float v = 1.0f;

        #endregion

        #region A

        public float A
        {
            get
            {
                return a;
            }
            set
            {
                a = value;
                Invalidate();
            }
        } private float a = 1.0f;

        #endregion

        private Color EffectiveColor
        {
            get
            {
                return Color.Adjust(v, this.A);
            }
        }

        private void Invalidate()
        {
            brush = null;
        }

        public SolidColorBrush Brush
        {
            get
            {
                if (brush == null)
                {
                    brush = new SolidColorBrush(EffectiveColor);
                }
                return brush;
            }
        }

        private SolidColorBrush brush;

        #region Provide Value

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget pvt = serviceProvider as IProvideValueTarget;
            if (pvt.TargetObject is DependencyObject)
            {
                return Brush;
                //return "ProvideValueString:" + DateTime.Now.ToString();
            }
            else
            {
                return this;
            }

        }

        #endregion

    }

    public static class ColorUtils
    {
        public static Color Adjust(this Color color, float v = 1f, float A = 1f)
        {
            float r = color.ScR;
            float g = color.ScG;
            float b = color.ScB;

            if (v != 1.0f)
            {
                if (v < 1.0f)
                {
                    r *= v;
                    g *= v;
                    b *= v;
                }
                else
                {
                    r = (v - 1) + (2 - v) * r;
                    g = (v - 1) + (2 - v) * g;
                    b = (v - 1) + (2 - v) * b;
                }
            }
            return Color.FromScRgb(A, r, g, b);
        }
    }

    [ContentProperty("Color")]
    [MarkupExtensionReturnType(typeof(SolidColorBrush))]
    public class LionColor : MarkupExtension
    {
        #region Construction

        public LionColor() { }
        public LionColor(Color color)
        {
            this.Color = color;
        }

        #endregion

        [ConstructorArgument("color")]
        public Color Color { get; set; }

        public float ColorV
        {
            get
            {
                float max = 0;
                max = Math.Max(max, Color.ScR);
                max = Math.Max(max, Color.ScG);
                max = Math.Max(max, Color.ScB);
                return max;
            }
        }

        #region V

        public float V
        {
            get
            {
                return v;
            }
            set
            {
                if (value > 2) value = 2;
                v = value; Invalidate();
            }
        } private float v = 1.0f;

        #endregion

        #region A

        public float A
        {
            get
            {
                return a;
            }
            set
            {
                a = value;
                Invalidate();
            }
        } private float a = 1.0f;

        #endregion

        private Color EffectiveColor
        {
            get
            {
                return Color.Adjust(v, this.A);

                //float r = Color.ScR;
                //float g = Color.ScG;
                //float b = Color.ScB;

                //if (v != 1.0f)
                //{
                //    if (v < 1.0f)
                //    {
                //        r *= v;
                //        g *= v;
                //        b *= v;
                //    }
                //    else
                //    {
                //        r = (v - 1) + (2 - v) * r;
                //        g = (v - 1) + (2 - v) * g;
                //        b = (v - 1) + (2 - v) * b;
                //    }
                //}
                //return Color.FromScRgb(this.A, r, g, b);
            }
        }

        private void Invalidate()
        {
            brush = null;
        }

        public SolidColorBrush Brush
        {
            get
            {
                if (brush == null)
                {
                    brush = new SolidColorBrush(EffectiveColor);
                }
                return brush;
            }
        }

        private SolidColorBrush brush;

        #region Provide Value

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            //IProvideValueTarget pvt = serviceProvider as IProvideValueTarget;
            //if (pvt.TargetObject is DependencyObject)
            //{
            //    return Brush;
            //    //return "ProvideValueString:" + DateTime.Now.ToString();
            //}
            //else
            //{
            //    return this;
            //}

            return Brush;
        }

        #endregion

    }
}
