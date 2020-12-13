using LionFire.Alerting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
//using ColorConverter = System.Windows.Media.ColorConverter;
using Microsoft.Extensions.Logging;
using LionFire.Shell;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace LionFire.Avalon
{
    public class BoolToHorizontalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is bool && (bool)value == true)
            {
                return HorizontalAlignment.Left;
            }
            return HorizontalAlignment.Center;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Interaction logic for AlertAdorner.xaml
    /// </summary>
    public partial class AlertAdorner : UserControl
    {
        public Alert Alert { get { return DataContext as Alert; } }

        public AlertAdorner()
        {
            InitializeComponent();
            this.DataContextChanged += AlertAdorner_DataContextChanged;
            closeButtonStyle = TryFindResource("AlertCloseLionButton") as Style;

            this.Focusable = true;
            this.Loaded += AlertAdorner_Loaded;

        }

        void AlertAdorner_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();

            
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
                e.Handled = true;
            }

            base.OnKeyUp(e);
            if (e.Handled) return;
        }

        Style closeButtonStyle;
        void AlertAdorner_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateButtons();
            UpdateColors();

            if (Alert != null && Alert.ShowTextEntry)
            {
                TextBox.SelectAll();
            }
        }

        private void UpdateButtons()
        {
            ButtonsPanel.Children.Clear();

            if (Alert != null && Alert.Buttons != null)
            {
                CloseButton.Visibility = Visibility.Collapsed;

                foreach (var b in Alert.Buttons)
                {
                    var lb = new LionButton()
                    {
                    };
                    lb.Content = b.Text;
                    if (closeButtonStyle != null)
                    {
                        lb.Style = closeButtonStyle;
                    }
                    lb.Click += (s, args) =>
                    {
                        if (b.OnClicked != null)
                        {
                            b.OnClicked();
                        }
                        Close();
                    };
                    ButtonsPanel.Children.Add(lb);
                }
            }
            else
            {
                ButtonsPanel.Children.Add(CloseButton);
            }
        }

        public Panel Layer { get; set; }

        private void Close()
        {
            if (Alert!=null&&Alert.DialogResult != null)
            {
                Alert.DialogResult.TextEntry = this.TextBox.Text;
            }

            Layer.Children.Remove(this);
            ((IAlerterInternal)Alerter.Instance).CloseAlert();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void UpdateColors()
        {
            var alert = Alert;
            if (alert == null) return;

            var AlertBorder = (SolidColorBrush)FindResource("AlertBorder");
            var AlertFG = (SolidColorBrush)FindResource("AlertFG");
            var AlertBG = (RadialGradientBrush)FindResource("AlertBG");
            var AlertHeaderBG = (LinearGradientBrush)FindResource("AlertHeaderBG");

            switch (alert.LogLevel)
            {
                //case LogLevel.Disabled:
                //break;
                //case LogLevel.Fatal:
                case LogLevel.Critical:
                case LogLevel.Error:
                    {
                        Color color = (Color)ColorConverter.ConvertFromString("#FF88");
                        Color color2 = (Color)ColorConverter.ConvertFromString("#FF22");
                        AlertBorder.Color = color;
                        AlertHeaderBG.GradientStops[0].Color = color;
                        AlertBG.GradientStops[0].Color = color2;
                        break;
                    }
                case LogLevel.Warning:
                    {
                        //case LogLevel.Warn:
                        Color color = (Color)ColorConverter.ConvertFromString("#FFA8");
                        Color color2 = (Color)ColorConverter.ConvertFromString("#FF92");
                        AlertBorder.Color = color;
                        AlertHeaderBG.GradientStops[0].Color = color;
                        AlertBG.GradientStops[0].Color = color2;
                        break;
                    }
                //case LogLevel.MajorMessage:
                //case LogLevel.Message:
                //case LogLevel.MinorMessage:
                //case LogLevel.Info:
                case LogLevel.Information:
                default:
                    {
                        Color color = (Color)ColorConverter.ConvertFromString("#EEE");
                        Color color2 = (Color)ColorConverter.ConvertFromString("#EEE");
                        AlertBorder.Color = color;
                        AlertHeaderBG.GradientStops[0].Color = color;
                        AlertBG.GradientStops[0].Color = color2;
                        break;
                    }
                //case LogLevel.Verbose:
                case LogLevel.Debug:
                case LogLevel.Trace:
                    {
                        Color color = (Color)ColorConverter.ConvertFromString("#555");
                        Color color2 = (Color)ColorConverter.ConvertFromString("#555");
                        AlertBorder.Color = color;
                        AlertHeaderBG.GradientStops[0].Color = color;
                        AlertBG.GradientStops[0].Color = color2;
                        break;
                    }
                //case LogLevel.Default:
                //    break;
                //case LogLevel.Step:
                //    break;
                //case LogLevel.All:
                //    break;
                //case LogLevel.Unspecified:
                //    break;
            }
        }
    }
}
