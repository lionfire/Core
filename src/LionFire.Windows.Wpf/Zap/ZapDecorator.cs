#if TOPORT
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Extensions.Logging;
using PixelLab.Common;

namespace LionFire.Avalon
{
    public class ZapDecorator : Decorator
    {
        public ZapDecorator()
        {
            m_listener.Rendering += m_listener_rendering;
            m_listener.WireParentLoadedUnloaded(this);
        }

        #region Orientation

        /// <summary>
        /// Orientation Dependency Property
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ZapDecorator),
                new FrameworkPropertyMetadata((Orientation)Orientation.Horizontal,
                    FrameworkPropertyMetadataOptions.AffectsArrange,
                    new PropertyChangedCallback(OnOrientationChanged)));

        /// <summary>
        /// Gets or sets the Orientation property. This dependency property 
        /// indicates ....
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Orientation property.
        /// </summary>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            ZapDecorator target = (ZapDecorator)d;
            Orientation oldOrientation = (Orientation)e.OldValue;
            Orientation newOrientation = target.Orientation;
            target.OnOrientationChanged(oldOrientation, newOrientation);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Orientation property.
        /// </summary>
        protected virtual void OnOrientationChanged(Orientation oldOrientation, Orientation newOrientation)
        {
        }

        #endregion


        //#region Padding

        ///// <summary>
        ///// Padding Dependency Property
        ///// </summary>
        //public static readonly DependencyProperty PaddingProperty =
        //    DependencyProperty.Register("Padding", typeof(Thickness), typeof(ZapDecorator),
        //        new FrameworkPropertyMetadata((Thickness)0,
        //            FrameworkPropertyMetadataOptions.AffectsArrange,
        //            new PropertyChangedCallback(OnPaddingChanged)));

        ///// <summary>
        ///// Gets or sets the Padding property. This dependency property 
        ///// indicates ....
        ///// </summary>
        //public Thickness Padding
        //{
        //    get { return (Thickness)GetValue(PaddingProperty); }
        //    set { SetValue(PaddingProperty, value); }
        //}

        ///// <summary>
        ///// Handles changes to the Padding property.
        ///// </summary>
        //private static void OnPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    ZapDecorator target = (ZapDecorator)d;
        //    Thickness oldPadding = (Thickness)e.OldValue;
        //    Thickness newPadding = target.Padding;
        //    target.OnPaddingChanged(oldPadding, newPadding);
        //}

        ///// <summary>
        ///// Provides derived classes an opportunity to handle changes to the Padding property.
        ///// </summary>
        //protected virtual void OnPaddingChanged(Thickness oldPadding, Thickness newPadding)
        //{
        //}

        //#endregion

        public static readonly DependencyProperty TargetIndexProperty =
            DependencyProperty.Register("TargetIndex", typeof(int), typeof(ZapDecorator),
            new FrameworkPropertyMetadata(0, new PropertyChangedCallback(targetIndexChanged)));

        public int TargetIndex
        {
            get { return (int)GetValue(TargetIndexProperty); }
            set { SetValue(TargetIndexProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            UIElement child = this.Child;
            Size size;
            if (child != null)
            {
                m_listener.StartListening();
                child.Measure(availableSize);
                size = child.DesiredSize;
            }
            else
            {
                size = new Size();
            }
            return size;
        }

        #region Implementation

        private void m_listener_rendering(object sender, EventArgs e)
        {
            if (this.Child != ZapPanel)
            {
                var itemsControl = this.Child as ItemsControl;
                if (itemsControl != null)
                {
                    ZapPanel = itemsControl.VisualDescendentsOfType<ZapPanel>().First();
                    ZapPanel.RenderTransform = m_traslateTransform;
                }
                else
                {
                    ZapPanel = (ZapPanel)this.Child;
                    ZapPanel.RenderTransform = m_traslateTransform;
                }
            }
            if (ZapPanel != null)
            {
                int actualTargetIndex = Math.Max(0, Math.Min(ZapPanel.Children.Count - 1, TargetIndex));

                double targetPercentOffset = -actualTargetIndex / (double)ZapPanel.Children.Count;
                targetPercentOffset = double.IsNaN(targetPercentOffset) ? 0 : targetPercentOffset;
                
                // TODO: Turn off elasic, use cubic or something instead
                bool stopListening = !GeoHelper.Animate(
                    m_percentOffset, m_velocity, targetPercentOffset,
                    .11, // attraction
                    .4,  // dampening
                    .6, // terminal velocity
                    //.02, // terminal velocity
                    c_diff, c_diff,
                    out m_percentOffset, out m_velocity);

                if (Orientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    double targetPixelOffset = m_percentOffset * (this.RenderSize.Width * ZapPanel.Children.Count);
                    m_traslateTransform.X = targetPixelOffset;
                }
                else
                {
                    double targetPixelOffset = m_percentOffset * (this.RenderSize.Height * ZapPanel.Children.Count);
                    m_traslateTransform.Y = targetPixelOffset;
                }

                if (stopListening)
                {
                    m_listener.StopListening();
                }
            }
        }

        private static void targetIndexChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            ((ZapDecorator)element).m_listener.StartListening();
        }

        private double m_velocity;
        private double m_percentOffset;

        private static readonly ILogger l = Log.Get();
		
        #region ZapPanel

        public ZapPanel ZapPanel
        {
            get { return zapPanel; }
            set
            {
                if (zapPanel == value) return;
                if (zapPanel != null) l.Warn("non-null zapPanel being changed"); // TEMP
                zapPanel = value;

                var ev = ZapPanelChanged;
                if (ev != null) ev();
            }
        } private ZapPanel zapPanel;

        public event Action ZapPanelChanged;

        #endregion


        //private ZapPanel m_zapPanel;
        private readonly TranslateTransform m_traslateTransform = new TranslateTransform();
        private readonly CompositionTargetRenderingListener m_listener = new CompositionTargetRenderingListener();

        private const double c_diff = .00001;

        #endregion
    }
}
#endif