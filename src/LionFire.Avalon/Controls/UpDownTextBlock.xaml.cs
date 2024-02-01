// REPLACED by behavior MouseWheelIncrementer!
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;

//namespace LionFire.Avalon
//{


//    /// <summary>
//    /// Interaction logic for UpDownTextBlock.xaml
//    /// </summary>
//    public partial class UpDownTextBlock : TextBlock
//    {
//        public UpDownTextBlock()
//        {
//            InitializeComponent();

//            this.MouseWheel += OnMouseWheel;
//        }

//        public int MaxMouseWheelIncrement = 1;
//        public int ShiftMultiplier = 10;
//        public int ControlMultiplier = 10;
//        public int MouseWheelNotchSize = int.MaxValue;

//        public bool Invert = false;

//        #region StringFormat

//        public string StringFormat
//        {
//            get { return stringFormat; }
//            set { stringFormat = value; }
//        } private string stringFormat = "+#;-#;0";

//        #endregion

//        public int Number
//        {
//            get
//            {
//                return Convert.ToInt32(this.Text);
//            }
//            set
//            {
//                if (StringFormat != null)
//                {
//                    this.Text = value.ToString(StringFormat);
//                }
//                else
//                {
//                    this.Text = value.ToString();
//                }
//            }
//        }

//        public void Increment()
//        {
//            int num = Convert.ToInt32(this.Text);
//            Number = ++num;
//        }
//        public void Decrement()
//        {
//            int num = Convert.ToInt32(this.Text);
//            Number = --num;
//        }


//        private void OnMouseWheel(object sender, MouseWheelEventArgs args)
//        {
//            int num = Number;

//            int delta;

//            if (MaxMouseWheelIncrement == 1)
//            {
//                delta = args.Delta > 0 ? 1 : -1;
//            }
//            else
//            {
//                if (args.Delta < MouseWheelNotchSize) MouseWheelNotchSize = args.Delta;

//                delta = args.Delta / MouseWheelNotchSize;

//                if (Math.Abs(delta) > MaxMouseWheelIncrement)
//                {
//                    delta = MaxMouseWheelIncrement * Math.Sign(delta);
//                }
//            }

//            if (Invert) delta *= -1;

//            if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None) delta *= ShiftMultiplier;
//            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) delta *= ControlMultiplier;

//            num += delta;
//            Number = num;
//        }


//    }
//}
