using System;
using System.Diagnostics;
using System.Reflection;
#if NOESIS
using Noesis;
#else
using System.Windows;
#endif
using System.Windows.Input;

namespace LionFire.Avalon
{

    public static class MouseWheelIncrementer
    {
#region Property

        /// <summary>
        /// Property Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty PropertyProperty =
            DependencyProperty.RegisterAttached("Property", typeof(string), typeof(MouseWheelIncrementer),
                new FrameworkPropertyMetadata((string)null,
                    new PropertyChangedCallback(OnPropertyChanged)));

        /// <summary>
        /// Gets the Property property. This dependency property 
        /// indicates ....
        /// </summary>
        public static string GetProperty(DependencyObject d)
        {
            return (string)d.GetValue(PropertyProperty);
        }

        /// <summary>
        /// Sets the Property property. This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetProperty(DependencyObject d, string value)
        {
            d.SetValue(PropertyProperty, value);
        }

        /// <summary>
        /// Handles changes to the Property property.
        /// </summary>
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string oldProperty = (string)e.OldValue;
            string newProperty = (string)d.GetValue(PropertyProperty);

            var fe = d as FrameworkElement;
            if (fe == null) return;

            if (newProperty == null)
            {
                if (oldProperty != null)
                {
                    fe.MouseWheel += fe_MouseWheel;
                }
            }
            else
            {
                if (oldProperty == null)
                {
                    fe.MouseWheel += fe_MouseWheel;
                }
            }
        }

#endregion

#region Target

        /// <summary>
        /// Target Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.RegisterAttached("Target", typeof(DependencyObject), typeof(MouseWheelIncrementer),
                new FrameworkPropertyMetadata((DependencyObject)null));

        /// <summary>
        /// Gets the Target property. This dependency property 
        /// indicates ....
        /// </summary>
        public static DependencyObject GetTarget(DependencyObject d)
        {
            return (DependencyObject)d.GetValue(TargetProperty);
        }

        /// <summary>
        /// Sets the Target property. This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetTarget(DependencyObject d, DependencyObject value)
        {
            d.SetValue(TargetProperty, value);
        }

#endregion

#region MinValue

        /// <summary>
        /// MinValue Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.RegisterAttached("MinValue", typeof(double), typeof(MouseWheelIncrementer),
                new FrameworkPropertyMetadata((double)double.NaN));

        /// <summary>
        /// Gets the MinValue property. This dependency property 
        /// indicates ....
        /// </summary>
        public static double GetMinValue(DependencyObject d)
        {
            return (double)d.GetValue(MinValueProperty);
        }

        /// <summary>
        /// Sets the MinValue property. This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetMinValue(DependencyObject d, double value)
        {
            d.SetValue(MinValueProperty, value);
        }

#endregion

#region Command

        /// <summary>
        /// Command Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(MouseWheelIncrementer),
                new FrameworkPropertyMetadata((ICommand)null,
                    new PropertyChangedCallback(OnCommandChanged)));

        /// <summary>
        /// Gets the Command property. This dependency property 
        /// indicates ....
        /// </summary>
        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }

        /// <summary>
        /// Sets the Command property. This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Handles changes to the Property property.
        /// </summary>
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ICommand oldProperty = (ICommand)e.OldValue;
            ICommand newProperty = (ICommand)d.GetValue(CommandProperty);

            var fe = d as FrameworkElement;
            if (fe == null) return;

            if (newProperty == null)
            {
                if (oldProperty != null)
                {
                    fe.MouseWheel += fe_MouseWheel;
                }
            }
            else
            {
                if (oldProperty == null)
                {
                    fe.MouseWheel += fe_MouseWheel;
                }
            }
        }

#endregion

#region Event Handling

        private static int GetDelta(int mouseWheelDelta)
        {
            int delta;
            if (MaxMouseWheelIncrement == 1)
            {
                delta = mouseWheelDelta > 0 ? 1 : -1;
            }
            else
            {
                if (mouseWheelDelta < MouseWheelNotchSize) MouseWheelNotchSize = mouseWheelDelta;

                delta = mouseWheelDelta / MouseWheelNotchSize;

                if (Math.Abs(delta) > MaxMouseWheelIncrement)
                {
                    delta = MaxMouseWheelIncrement * Math.Sign(delta);
                }
            }

            if (Invert) delta *= -1;
#if !NOESIS // TOPORT
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None) delta *= ShiftMultiplier;
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) delta *= ControlMultiplier;
#endif

            return delta;
        }

        static void fe_MouseWheel(object sender, MouseWheelEventArgs args)
        {
            var dobSender = sender as DependencyObject;
            if (dobSender == null) return;

            int delta = GetDelta(args.Delta);

            var command = GetCommand(dobSender);
            if (command != null)
            {
                if (command.CanExecute(delta))
                {
                    //try
                    //{
                        command.Execute(delta);
                    //}
                    //catch (Exception ex)
                    //{
                    //    Trace.WriteLine("MouseWheelIncrementer command failed: " + ex.ToString());
                    //}
                }
                return;
            }

            var propertyName = GetProperty(dobSender);

            var target = GetTarget(dobSender);
            if (target == null) { target = dobSender; }

            PropertyInfo pi = target.GetType().GetProperty(propertyName);

            if (pi == null)
            {
                Debug.WriteLine(typeof(MouseWheelIncrementer).Name + ": could not get property " + propertyName);
                return;
            }

            int num;

            if (pi.PropertyType == typeof(string))
            {
                string text = pi.GetValue(target, null) as string;
                if (String.IsNullOrWhiteSpace(text)) num = 0;
                else num = Convert.ToInt32(text);

                num += delta;

                string newText;
                if (StringFormat != null)
                {
                    newText = num.ToString(StringFormat);
                }
                else
                {
                    newText = num.ToString();
                }

                pi.SetValue(target, newText, null);
            }
            else
            {
                var minValue = GetMinValue(dobSender);

                if (pi.PropertyType == typeof(int))
                {
                    num = (int)pi.GetValue(target, null);
                    if ((double)num < minValue) { num = (int)minValue; }
                    num += delta;
                    pi.SetValue(target, num, null);
                }
                else if (pi.PropertyType == typeof(double))
                {
                    double val = (double)pi.GetValue(target, null);
                    if (val < minValue) { val = minValue; }
                    val += delta;
                    pi.SetValue(target, (double)val, null);
                    return;
                }
                else
                {
                    Debug.WriteLine(typeof(MouseWheelIncrementer).Name + ": only properties of type string or Int32 supported: " + propertyName);
                    return;
                }
            }
        }

#endregion

#region Options (TODO: Attached Properties)

        public static int MaxMouseWheelIncrement = 1;
        public static int ShiftMultiplier = 10;
        public static int ControlMultiplier = 10;
        public static int MouseWheelNotchSize = int.MaxValue;

        public static bool Invert = false;


#region StringFormat

        public static string StringFormat
        {
            get { return stringFormat; }
            set { stringFormat = value; }
        } 
        //private static string stringFormat = "+#;-#;0";
        private static string stringFormat = null;

#endregion

#endregion

    }

}
