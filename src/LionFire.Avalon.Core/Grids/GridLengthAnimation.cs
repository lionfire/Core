// Retrieved from http://www.codeproject.com/Articles/18379/WPF-Tutorial-Part-2-Writing-a-custom-animation-cla
// on March 25, 2013 under CPOL

// Jared added easing function
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace LionFire.Avalon
{
    public class GridLengthAnimation : AnimationTimeline
    {
        static GridLengthAnimation()
        {
            FromProperty = DependencyProperty.Register("From", typeof(GridLength),
                typeof(GridLengthAnimation));

            ToProperty = DependencyProperty.Register("To", typeof(GridLength), 
                typeof(GridLengthAnimation));

        }

        public override Type TargetPropertyType
        {
            get 
            {
                return typeof(GridLength);
            }
        }

        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new GridLengthAnimation();
        }

        public static readonly DependencyProperty FromProperty;
        public GridLength From
        {
            get
            {
                return (GridLength)GetValue(GridLengthAnimation.FromProperty);
            }
            set
            {
                SetValue(GridLengthAnimation.FromProperty, value);
            }
        }

        public static readonly DependencyProperty ToProperty;
        public GridLength To
        {
            get
            {
                return (GridLength)GetValue(GridLengthAnimation.ToProperty);
            }
            set
            {
                SetValue(GridLengthAnimation.ToProperty, value);
            }
        }

        public override object GetCurrentValue(object defaultOriginValue, 
            object defaultDestinationValue, AnimationClock animationClock)
        {
            double fromVal = ((GridLength)GetValue(GridLengthAnimation.FromProperty)).Value;
            double toVal = ((GridLength)GetValue(GridLengthAnimation.ToProperty)).Value;

            if (EasingFunction == null)
            {
                l.Warn("No easing function. setting one.");
                // TEMP
                EasingFunction = new System.Windows.Media.Animation.CubicEase()
                {
                    EasingMode = EasingMode.EaseOut,
                };
            }
            
            if (fromVal > toVal)
            {
                double progress = animationClock.CurrentProgress.Value;
                if (EasingFunction != null)
                {
                    progress = EasingFunction.Ease(progress);
                }
                return new GridLength((1-progress) * (fromVal - toVal) + toVal, GridUnitType.Pixel);
            }
            else
            {
                double progress = animationClock.CurrentProgress.Value;
                if (EasingFunction != null)
                {
                    progress = EasingFunction.Ease(progress);
                }
                return new GridLength(progress * (toVal - fromVal) + fromVal, GridUnitType.Pixel);
            }
        }

        //public IEasingFunction EasingFunction { get; set; }

        #region EasingFunction

        public IEasingFunction EasingFunction
        {
            get { return easingFunction; }
            set { easingFunction = value; }
        } private IEasingFunction easingFunction;

        #endregion



        private static readonly ILogger l = Log.Get();
		
    }
}
