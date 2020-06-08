//#define NO_ALPHA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Input;
using Microsoft.Extensions.Logging;

namespace LionFire.Avalon
{
    //[TemplatePart(Name = "GlassColor", Type = typeof(Color))]
    //[TemplatePart(Name = "GlassColor2", Type = typeof(Color))]
    [TemplatePart(Name = "GlassGradientStop", Type = typeof(GradientStop))]
    [TemplatePart(Name = "GlassGradientStop2", Type = typeof(GradientStop))]
    public class LionButton : Button
    {
        static LionButton()
        {
            try
            {
                DefaultStyleKeyProperty.OverrideMetadata(typeof(LionButton), new FrameworkPropertyMetadata(typeof(LionButton)));
            }
            catch { }
        }

        public LionButton()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                OverridesDefaultStyle = true;
            }
        }

        public override void OnApplyTemplate()
        {
            try
            {
                base.OnApplyTemplate();
            }
            catch { }

            try
            {
                GradientStop gs = this.Template.FindName("GlassGradientStop", this) as GradientStop;
                if (gs != null)
                {
                    gs.Color = GlassColor;
                }
                GradientStop gs2 = this.Template.FindName("GlassGradientStop2", this) as GradientStop;
                if (gs2 != null)
                {
                    gs2.Color = GlassColor;
                }
            }
            catch { }

            UpdateAltBackgroundOpacity(); 
        }

        #region HandleMouseLeftButton

        /// <summary>
        /// HandleMouseLeftButton Dependency Property
        /// </summary>
        public static readonly DependencyProperty HandleMouseLeftButtonProperty =
            DependencyProperty.Register("HandleMouseLeftButton", typeof(bool), typeof(LionButton),
                new FrameworkPropertyMetadata((bool)true,
                    FrameworkPropertyMetadataOptions.None));

        /// <summary>
        /// Gets or sets the HandleMouseLeftButton property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool HandleMouseLeftButton
        {
            get { return (bool)GetValue(HandleMouseLeftButtonProperty); }
            set { SetValue(HandleMouseLeftButtonProperty, value); }
        }

        #endregion

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            //HandleMouseLeftButton = false; // TEMP TEST
            base.OnMouseLeftButtonUp(e);

            
            e.Handled &= HandleMouseLeftButton;
        }

        //private static readonly ILogger l = Log.Get();
		

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            //HandleMouseLeftButton = false; // TEMP TEST

            base.OnMouseLeftButtonDown(e);

            e.Handled &= HandleMouseLeftButton;
        }

        #region ContentEffect

        /// <summary>
        /// ContentEffect Dependency Property
        /// </summary>
        public static readonly DependencyProperty ContentEffectProperty =
            DependencyProperty.Register("ContentEffect", typeof(Effect), typeof(LionButton),
                new FrameworkPropertyMetadata((Effect)null,
                    new PropertyChangedCallback(OnContentEffectChanged)));

        /// <summary>
        /// Gets or sets the ContentEffect property. This dependency property 
        /// indicates the effect applied to the content of the button.
        /// </summary>
        public Effect ContentEffect
        {
            get { return (Effect)GetValue(ContentEffectProperty); }
            set { SetValue(ContentEffectProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ContentEffect property.
        /// </summary>
        private static void OnContentEffectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LionButton target = (LionButton)d;
            Effect oldContentEffect = (Effect)e.OldValue;
            Effect newContentEffect = target.ContentEffect;
            target.OnContentEffectChanged(oldContentEffect, newContentEffect);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ContentEffect property.
        /// </summary>
        protected virtual void OnContentEffectChanged(Effect oldContentEffect, Effect newContentEffect)
        {
        }

        #endregion

        #region GlassColor

        /// <summary>
        /// GlassColor Dependency Property
        /// </summary>
        public static readonly DependencyProperty GlassColorProperty =
            DependencyProperty.Register("GlassColor", typeof(Color), typeof(LionButton),
                new FrameworkPropertyMetadata((Color)Colors.White, FrameworkPropertyMetadataOptions.AffectsRender,
                    new PropertyChangedCallback(OnGlassColorChanged)));

        /// <summary>
        /// Gets or sets the GlassColor property. This dependency property 
        /// indicates ....
        /// </summary>
        public Color GlassColor
        {
            get { return (Color)GetValue(GlassColorProperty); }
            set { SetValue(GlassColorProperty, value); }
        }

        /// <summary>
        /// Handles changes to the GlassColor property.
        /// </summary>
        private static void OnGlassColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LionButton target = (LionButton)d;
            Color oldGlassColor = (Color)e.OldValue;
            Color newGlassColor = target.GlassColor;
            target.OnGlassColorChanged(oldGlassColor, newGlassColor);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the GlassColor property.
        /// </summary>
        protected virtual void OnGlassColorChanged(Color oldGlassColor, Color newGlassColor)
        {
        }

        #endregion

        #region GlassColor2

        /// <summary>
        /// GlassColor2 Dependency Property
        /// </summary>
        public static readonly DependencyProperty GlassColor2Property =
            DependencyProperty.Register("GlassColor2", typeof(Color), typeof(LionButton),
                new FrameworkPropertyMetadata((Color)Colors.White, FrameworkPropertyMetadataOptions.AffectsRender,
                    new PropertyChangedCallback(OnGlassColor2Changed)));

        /// <summary>
        /// Gets or sets the GlassColor2 property. This dependency property 
        /// indicates ....
        /// </summary>
        public Color GlassColor2
        {
            get { return (Color)GetValue(GlassColor2Property); }
            set { SetValue(GlassColor2Property, value); }
        }

        /// <summary>
        /// Handles changes to the GlassColor2 property.
        /// </summary>
        private static void OnGlassColor2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LionButton target = (LionButton)d;
            Color oldGlassColor2 = (Color)e.OldValue;
            Color newGlassColor2 = target.GlassColor2;
            target.OnGlassColor2Changed(oldGlassColor2, newGlassColor2);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the GlassColor2 property.
        /// </summary>
        protected virtual void OnGlassColor2Changed(Color oldGlassColor2, Color newGlassColor2)
        {
        }

        #endregion

        #region GlassOpacity

        /// <summary>
        /// GlassOpacity Dependency Property
        /// </summary>
        public static readonly DependencyProperty GlassOpacityProperty =
            DependencyProperty.Register("GlassOpacity", typeof(double), typeof(LionButton),
                new UIPropertyMetadata((double)0.7,
                    new PropertyChangedCallback(OnGlassOpacityChanged)));

        /// <summary>
        /// Gets or sets the GlassOpacity property. This dependency property 
        /// indicates the opacity of the glass reflection effect.
        /// </summary>
        public double GlassOpacity
        {
            get { return (double)GetValue(GlassOpacityProperty); }
            set { SetValue(GlassOpacityProperty, value); }
        }

        /// <summary>
        /// Handles changes to the GlassOpacity property.
        /// </summary>
        private static void OnGlassOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LionButton target = (LionButton)d;
            double oldGlassOpacity = (double)e.OldValue;
            double newGlassOpacity = target.GlassOpacity;
            target.OnGlassOpacityChanged(oldGlassOpacity, newGlassOpacity);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the GlassOpacity property.
        /// </summary>
        protected virtual void OnGlassOpacityChanged(double oldGlassOpacity, double newGlassOpacity)
        {
        }

        #endregion

        #region CornerRadius

        /// <summary>
        /// CornerRadius Dependency Property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(LionButton),
                new UIPropertyMetadata(new CornerRadius(8)));

        /// <summary>
        /// Gets or sets the CornerRadius property. This dependency property 
        /// indicates ....
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        #endregion

        #region RestingOpacity

        /// <summary>
        /// RestingOpacity Dependency Property
        /// </summary>
        public static readonly DependencyProperty RestingOpacityProperty =
            DependencyProperty.Register("RestingOpacity", typeof(double), typeof(LionButton),
                new FrameworkPropertyMetadata((double)1.0,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the RestingOpacity property. This dependency property 
        /// indicates the opacity in in the main content area when the mouse is not hovering over the button.
        /// </summary>
        public double RestingOpacity
        {
            get { return (double)GetValue(RestingOpacityProperty); }
            set { SetValue(RestingOpacityProperty, value); }
        }

        #endregion

        //#region HoverBorderBrush

        ///// <summary>
        ///// HoverBorderBrush Dependency Property
        ///// </summary>
        //public static readonly DependencyProperty HoverBorderBrushProperty =
        //    DependencyProperty.Register("HoverBorderBrush", typeof(Brush), typeof(LionButton),
        //        new FrameworkPropertyMetadata((Brush)null,
        //            FrameworkPropertyMetadataOptions.AffectsRender));

        ///// <summary>
        ///// Gets or sets the HoverBorderBrush property. This dependency property 
        ///// indicates ....
        ///// </summary>
        //public Brush HoverBorderBrush
        //{
        //    get { return (Brush)GetValue(HoverBorderBrushProperty); }
        //    set { SetValue(HoverBorderBrushProperty, value); }
        //}

        //#endregion

        //#region CurrentBorderBrush

        ///// <summary>
        ///// CurrentBorderBrush Dependency Property
        ///// </summary>
        //public static readonly DependencyProperty CurrentBorderBrushProperty =
        //    DependencyProperty.Register("CurrentBorderBrush", typeof(Brush), typeof(LionButton),
        //        new FrameworkPropertyMetadata((Brush)null,
        //            FrameworkPropertyMetadataOptions.AffectsRender));

        ///// <summary>
        ///// Gets or sets the CurrentBorderBrush property. This dependency property 
        ///// indicates ....
        ///// </summary>
        //public Brush CurrentBorderBrush
        //{
        //    get { return (Brush)GetValue(CurrentBorderBrushProperty); }
        //    set { SetValue(CurrentBorderBrushProperty, value); }
        //}

        //#endregion



        #region HoverBackground

        /// <summary>
        /// HoverBackground Dependency Property
        /// </summary>
        public static readonly DependencyProperty HoverBackgroundProperty =
            DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(LionButton),
                new FrameworkPropertyMetadata((Brush)new SolidColorBrush(Colors.White),
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the HoverBackground property. This dependency property 
        /// indicates the background color when the mouse is over the button.
        /// </summary>
        public Brush HoverBackground
        {
            get { return (Brush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }

        #endregion

        #region HoverBackgroundOpacity

        /// <summary>
        /// HoverBackgroundOpacity Dependency Property
        /// </summary>
        public static readonly DependencyProperty HoverBackgroundOpacityProperty =
            DependencyProperty.Register("HoverBackgroundOpacity", typeof(double), typeof(LionButton),
                new FrameworkPropertyMetadata(0.08,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the HoverBackgroundOpacity property. This dependency property 
        /// indicates the background color when the mouse is over the button.
        /// </summary>
        public double HoverBackgroundOpacity
        {
            get { return (double)GetValue(HoverBackgroundOpacityProperty); }
            set { SetValue(HoverBackgroundOpacityProperty, value); }
        }

        #endregion

        #region HoverBackgroundCurrentOpacity

        /// <summary>
        /// HoverBackgroundCurrentOpacity Dependency Property
        /// </summary>
        public static readonly DependencyProperty HoverBackgroundCurrentOpacityProperty =
            DependencyProperty.Register("HoverBackgroundCurrentOpacity", typeof(double), typeof(LionButton),
                new FrameworkPropertyMetadata(0.00,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the HoverBackgroundCurrentOpacity property. This dependency property 
        /// indicates the background color when the mouse is over the button.
        /// </summary>
        public double HoverBackgroundCurrentOpacity
        {
            get { return (double)GetValue(HoverBackgroundCurrentOpacityProperty); }
            set { SetValue(HoverBackgroundCurrentOpacityProperty, value); }
        }

        #endregion

        #region MouseDownHoverBackgroundOpacity

        /// <summary>
        /// MouseDownHoverBackgroundOpacity Dependency Property
        /// </summary>
        public static readonly DependencyProperty MouseDownHoverBackgroundOpacityProperty =
            DependencyProperty.Register("MouseDownHoverBackgroundOpacity", typeof(double), typeof(LionButton),
                new FrameworkPropertyMetadata(0.4,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the MouseDownHoverBackgroundOpacity property. This dependency property 
        /// indicates the background color when the mouse is over the button.
        /// </summary>
        public double MouseDownHoverBackgroundOpacity
        {
            get { return (double)GetValue(MouseDownHoverBackgroundOpacityProperty); }
            set { SetValue(MouseDownHoverBackgroundOpacityProperty, value); }
        }

        #endregion

        #region AltBackground

        /// <summary>
        /// AltBackground Dependency Property
        /// </summary>
        public static readonly DependencyProperty AltBackgroundProperty =
            DependencyProperty.Register("AltBackground", typeof(Brush), typeof(LionButton),
                new FrameworkPropertyMetadata((Brush)null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the AltBackground property. This dependency property 
        /// indicates ....
        /// </summary>
        public Brush AltBackground
        {
            get { return (Brush)GetValue(AltBackgroundProperty); }
            set { SetValue(AltBackgroundProperty, value); }
        }

        #endregion

        #region AltBackgroundCurrentOpacity

        /// <summary>
        /// AltBackgroundCurrentOpacity Dependency Property
        /// Set by LionButton.
        /// </summary>
        public static readonly DependencyProperty AltBackgroundCurrentOpacityProperty =
            DependencyProperty.Register("AltBackgroundCurrentOpacity", typeof(double), typeof(LionButton),
                new FrameworkPropertyMetadata((double)0,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the AltBackgroundCurrentOpacity property. This dependency property 
        /// indicates ....
        /// </summary>
        public double AltBackgroundCurrentOpacity
        {
            get { return (double)GetValue(AltBackgroundCurrentOpacityProperty); }
            set { SetValue(AltBackgroundCurrentOpacityProperty, value); } // REMOVE this setter?
        }

        #endregion

        #region IsAltMode

        /// <summary>
        /// IsAltMode Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsAltModeProperty =
            DependencyProperty.Register("IsAltMode", typeof(bool), typeof(LionButton),
                new FrameworkPropertyMetadata((bool)false,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    new PropertyChangedCallback(OnIsAltModeChanged)));

        /// <summary>
        /// Gets or sets the IsAltMode property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsAltMode
        {
            get { return (bool)GetValue(IsAltModeProperty); }
            set { SetValue(IsAltModeProperty, value); }
        }

        /// <summary>
        /// Handles changes to the IsAltMode property.
        /// </summary>
        private static void OnIsAltModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LionButton target = (LionButton)d;
            bool oldIsAltMode = (bool)e.OldValue;
            bool newIsAltMode = target.IsAltMode;
            target.OnIsAltModeChanged(oldIsAltMode, newIsAltMode);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the IsAltMode property.
        /// </summary>
        protected virtual void OnIsAltModeChanged(bool oldIsAltMode, bool newIsAltMode)
        {
            UpdateAltBackgroundOpacity();
        }
        private void UpdateAltBackgroundOpacity()
        {
            AltBackgroundCurrentOpacity = IsAltMode ? 1 : 0;
        }

        #endregion

        #region CanAltMode

        /// <summary>
        /// CanAltMode Dependency Property
        /// </summary>
        public static readonly DependencyProperty CanAltModeProperty =
            DependencyProperty.Register("CanAltMode", typeof(bool), typeof(LionButton),
                new FrameworkPropertyMetadata((bool)false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback(OnCanAltModeChanged)));

        /// <summary>
        /// Gets or sets the CanAltMode property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool CanAltMode
        {
            get { return (bool)GetValue(CanAltModeProperty); }
            set { SetValue(CanAltModeProperty, value); }
        }

        /// <summary>
        /// Handles changes to the CanAltMode property.
        /// </summary>
        private static void OnCanAltModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LionButton target = (LionButton)d;
            bool oldCanAltMode = (bool)e.OldValue;
            bool newCanAltMode = target.CanAltMode;
            target.OnCanAltModeChanged(oldCanAltMode, newCanAltMode);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the CanAltMode property.
        /// </summary>
        protected virtual void OnCanAltModeChanged(bool oldCanAltMode, bool newCanAltMode)
        {
            if (CanAltMode)
            {
                this.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(RightClickButton_MouseRightButtonDown);
                this.MouseRightButtonUp += new System.Windows.Input.MouseButtonEventHandler(RightClickButton_MouseRightButtonUp);
            }
            else
            {
                this.MouseRightButtonDown -= new System.Windows.Input.MouseButtonEventHandler(RightClickButton_MouseRightButtonDown);
                this.MouseRightButtonUp -= new System.Windows.Input.MouseButtonEventHandler(RightClickButton_MouseRightButtonUp);
            }
        }

        #endregion

        #region Right Click Handling

        public event RoutedEventHandler RightClick;

        private bool _clicked = false;

        void RightClickButton_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.IsPressed = true;
            CaptureMouse();
            _clicked = true;
        }

        void RightClickButton_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();

            if (this.IsMouseOver && _clicked)
            {
                if (RightClick != null)
                {
                    RightClick.Invoke(this, e);
                }
                if (CanAltMode)
                {
                    IsAltMode ^= true;
                }
            }

            _clicked = false;
            this.IsPressed = false;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.IsMouseCaptured)
            {
                bool isInside = false;

                VisualTreeHelper.HitTest(
                    this,
                    d =>
                    {
                        if (d == this)
                        {
                            isInside = true;
                        }

                        return HitTestFilterBehavior.Stop;
                    },
                    ht => HitTestResultBehavior.Stop,
                    new PointHitTestParameters(e.GetPosition(this)));

                if (isInside)
                    this.IsPressed = true;
                else
                    this.IsPressed = false;
            }
        }

        #endregion


    }
}
