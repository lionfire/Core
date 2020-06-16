using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Controls.Primitives;

namespace LionFire.Avalon
{
    //[TemplatePart(Name = "GlassColor", Type = typeof(Color))]
    //[TemplatePart(Name = "GlassColor2", Type = typeof(Color))]
    [TemplatePart(Name = "GlassGradientStop", Type = typeof(GradientStop))]
    [TemplatePart(Name = "GlassGradientStop2", Type = typeof(GradientStop))]
    public class LionToggleButton : ToggleButton
    {
        static LionToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LionToggleButton), new FrameworkPropertyMetadata(typeof(LionToggleButton)));
        }

        public LionToggleButton()
        {
            
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

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
        }

        #region HandleMouseLeftButton

        /// <summary>
        /// HandleMouseLeftButton Dependency Property
        /// </summary>
        public static readonly DependencyProperty HandleMouseLeftButtonProperty =
            DependencyProperty.Register("HandleMouseLeftButton", typeof(bool), typeof(LionToggleButton),
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
            HandleMouseLeftButton = false; // TEMP TEST
            base.OnMouseLeftButtonUp(e);
            
            e.Handled &= HandleMouseLeftButton;
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            HandleMouseLeftButton = false; // TEMP TEST

            base.OnMouseLeftButtonDown(e);

            e.Handled &= HandleMouseLeftButton;

        }


        #region ContentEffect

        /// <summary>
        /// ContentEffect Dependency Property
        /// </summary>
        public static readonly DependencyProperty ContentEffectProperty =
            DependencyProperty.Register("ContentEffect", typeof(Effect), typeof(LionToggleButton),
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
            LionToggleButton target = (LionToggleButton)d;
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
            DependencyProperty.Register("GlassColor", typeof(Color), typeof(LionToggleButton),
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
            LionToggleButton target = (LionToggleButton)d;
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
            DependencyProperty.Register("GlassColor2", typeof(Color), typeof(LionToggleButton),
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
            LionToggleButton target = (LionToggleButton)d;
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
            DependencyProperty.Register("GlassOpacity", typeof(double), typeof(LionToggleButton),
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
            LionToggleButton target = (LionToggleButton)d;
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
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(LionToggleButton),
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
            DependencyProperty.Register("RestingOpacity", typeof(double), typeof(LionToggleButton),
                new FrameworkPropertyMetadata((double)0.7,
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

    }
}
