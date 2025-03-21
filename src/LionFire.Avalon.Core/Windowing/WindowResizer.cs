using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using LionFire.Avalon.WinForms;
using Microsoft.Extensions.Logging;

namespace LionFire.Avalon
{

    public class WindowResizer
    {
        #region Misc

        private static readonly ILogger l = Log.Get();
		
        #endregion

        public WindowResizer()
        {
            FramesPerSecond = 60.0;
        }

        #region (Public) Parameters

        public Window Window;
        public Size TargetSize;
        public double Duration = 1.0;
        public double FramesPerSecond
        {
            get { return 1000.0 / IntervalMilliseconds; }
            set { IntervalMilliseconds = (int)(1000.0 / value); }
        }
        public int IntervalMilliseconds;
        public IEasingFunction EasingFunction = null;

        public HorizontalAlignment HorizontalAlignment = HorizontalAlignment.Center;
        public VerticalAlignment VerticalAlignment = VerticalAlignment.Center;

        #endregion

        #region State

        double startHeight;
        double startWidth;
        double startLeft;
        double startTop;

        private double deltaW;
        private double deltaL;
        private double deltaH;
        private double deltaT;

        public Action Finished;

        double elapsed;
        double EffectiveProgress { get { if (EasingFunction == null) return progress; else return EasingFunction.Ease(progress); } }
        double progress { get { return elapsed / Duration; } }

        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public int frames
        {
            get { return (int)(FramesPerSecond * Duration); }
        }

        #endregion

        public void Cancel()
        {
            if (timer != null)
            {
                timer.Stop();
            }
            timer = null;
        }
        #region (Private) Event Handlers

        private void timer_Tick(Object myObject, EventArgs myEventArgs)
        {
            if (timer == null) return;

            using (var d = Window.Dispatcher.DisableProcessing())
            {
                Window.Height = startHeight + (deltaH * EffectiveProgress);
                Window.Width = startWidth + (deltaW * EffectiveProgress);

                if (deltaL != 0.0) Window.Left = startLeft + (deltaL * EffectiveProgress);
                if (deltaT != 0.0) Window.Top = startTop + (deltaT * EffectiveProgress);

                elapsed += 1.0 / FramesPerSecond;

                if (elapsed >= Duration)
                {
                    timer.Stop();
                    timer.Enabled = false;
                    timer.Dispose();

                    Window.Height = TargetSize.Height;
                    Window.Width = TargetSize.Width;
                    Window.Left = startLeft + deltaL;
                    Window.Top = startTop + deltaT;

                    if (snapsToDevicePixels) Window.SnapsToDevicePixels = this.snapsToDevicePixels;

                    if (Finished != null) Finished();
                }
            }
        }

        #endregion

        public void Resize(double maxSizePercentage = 0.85)
        {
            //startHeight = Window.ActualHeight;
            //startWidth = Window.ActualWidth;
            startHeight = Window.Height;
            startWidth = Window.Width;
            elapsed = 0;

            deltaH = (TargetSize.Height - startHeight);
            deltaW = (TargetSize.Width - startWidth);
            startLeft = Window.Left;
            startTop = Window.Top;

            l.Warn("startLeft: " + startLeft + ", startTop: " + startTop + ", deltaTop: " + deltaT + ", startHeight: " + startHeight);

            WpfScreen screen = WpfScreen.GetScreenFrom(Window);
           
            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    deltaL = -deltaW * 0.5;
                    break;
                case HorizontalAlignment.Left:
                    deltaL = 0;
                    break;
                case HorizontalAlignment.Right:
                    deltaL = -deltaW;
                    break;
                case HorizontalAlignment.Stretch:
                default:
                    throw new ArgumentException("Unsupported alignment: " + HorizontalAlignment);
                    //break;
            }

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    deltaT = -deltaH;
                    break;
                case VerticalAlignment.Center:
                    deltaT = -deltaH * 0.5;
                    break;
                case VerticalAlignment.Stretch:
                    break;
                case VerticalAlignment.Top:
                    deltaT = 0;
                    break;
                default:
                    break;
            }

            //double targetLeft = startLeft + deltaL;
            //double targetTop = startTop + deltaT;

            Rect targetRect = new Rect(startLeft + deltaL, startTop + deltaT, TargetSize.Width, TargetSize.Height);

            if(!double.IsNaN(maxSizePercentage))
            {
                targetRect = WindowUtils.ConstrainRectToRect(targetRect, screen.DeviceBounds, 0.9);
                TargetSize = targetRect.Size;
                deltaL = targetRect.Left - startLeft;
                deltaT = targetRect.Top - startTop;
            }

            l.Warn("startLeft: " + startLeft + ", startTop: " + startTop + ", deltaTop: " + deltaT + ", startHeight: " + startHeight);
            l.Warn("targetRect: " + targetRect);

            timer.Tick += new EventHandler(timer_Tick);
            //_Timer.Interval = (int)((Duration * 1000) / frames);
            timer.Interval = (int)(1000 / FramesPerSecond);

            Window.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            Window.VerticalContentAlignment = VerticalAlignment.Stretch;

            //_Height = this.TargetSize.Height;
            //_Width = this.TargetSize.Width;

            //_RatioHeight = ((Window.Height - TargetSize.Height) / frames) * -1;
            //_RatioWidth = ((Window.Width - TargetSize.Width) / frames) * -1;

            snapsToDevicePixels = Window.SnapsToDevicePixels;
            if (snapsToDevicePixels) Window.SnapsToDevicePixels = false;

            //_Timer.Enabled = true;
            timer.Start();
        }
        bool snapsToDevicePixels;
    }

}
