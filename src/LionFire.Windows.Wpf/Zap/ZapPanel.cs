#if TOPORT
using Microsoft.Extensions.Logging;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LionFire.Avalon
{
    public class ZapPanel : Panel
    {
        public ZapPanel()
        {
            LayoutUpdated += new EventHandler(ZapPanel_LayoutUpdated);
            //Background = new SolidColorBrush(Color.FromArgb(0x75, 0xff, 0x20,0x20));
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return null;
        }

        private static readonly ILogger l = Log.Get();
		
        protected override Size MeasureOverride(Size availableSize)
        {
            //if the parent isn't a FrameworkElement, then measure all the children to infinity
            //and return a size that would result if all of them were as big as the biggest

            //otherwise, measure all at the size of the parent and return the size accordingly

            Size max = new Size();

            HorizontalAlignment ha = this.HorizontalAlignment;
            VerticalAlignment va = this.VerticalAlignment;


            UIElement child;
            for (int i = 0; i < this.InternalChildren.Count; i++)
            {
                child = this.InternalChildren[i];
                try
                {
                    child.Measure(availableSize);
                }
                catch (Exception ex)
                {
                    l.Error("Child measure threw exception: " + ex);
                }
                max.Width = Math.Max(max.Width, child.DesiredSize.Width);
                max.Height = Math.Max(max.Height, child.DesiredSize.Height);

                FrameworkElement fe = child as FrameworkElement;
                if (fe != null)
                {
                    if (fe.HorizontalAlignment == System.Windows.HorizontalAlignment.Stretch)
                    {
                        ha = System.Windows.HorizontalAlignment.Stretch;
                    }
                    if (fe.VerticalAlignment == System.Windows.VerticalAlignment.Stretch)
                    {
                        va = System.Windows.VerticalAlignment.Stretch;
                    }
                }
            }

            if (LionZapScroller != null)
            {
                if (LionZapScroller.Name != this.Tag as string) l.Warn("Tag mismatch: " + LionZapScroller.Name + " " + this.Tag);
                var parent = LionZapScroller;
                {
                    if (parent.HorizontalAlignment == System.Windows.HorizontalAlignment.Stretch)
                    {
                        ha = System.Windows.HorizontalAlignment.Stretch;
                    }
                    if (parent.VerticalAlignment == System.Windows.VerticalAlignment.Stretch)
                    {
                        va = System.Windows.VerticalAlignment.Stretch;
                    }
                }
            }
            //else
            //{
            //    l.Warn("No parent LionZapScroller");
            //}

            if (ha == System.Windows.HorizontalAlignment.Stretch && availableSize.Width != double.PositiveInfinity)
            {
                max.Width = availableSize.Width;
            }
            if (va == System.Windows.VerticalAlignment.Stretch && availableSize.Height != double.PositiveInfinity)
            {
                max.Height = availableSize.Height;
            }

            Size returnSize;
            int count = System.Math.Max(this.InternalChildren.Count, 1);

            //if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
            {
                if (Orientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    returnSize = new Size(max.Width * count, max.Height);
                }
                else
                {
                    returnSize = new Size(max.Width, max.Height * count);
                }
            }
            //else
            //{
            //    if (HorizontalAlignment == System.Windows.HorizontalAlignment.Stretch)
            //    {
            //        rwidth = availableSize.Width;
            //    }
            //    else
            //    {
            //        if (Orientation == System.Windows.Controls.Orientation.Horizontal)
            //        {
            //            rwidth = availableSize.Width * this.InternalChildren.Count;
            //        }
            //        else
            //        {
            //            rwidth = availableSize.Width;
            //        }
            //    }

            //    if (VerticalAlignment == System.Windows.VerticalAlignment.Stretch)
            //    {
            //        rheight = availableSize.Height;
            //    }
            //    else
            //    {
            //        if (Orientation == System.Windows.Controls.Orientation.Horizontal)
            //        {
            //            rheight = System.Math.Min(availableSize.Height, max.Height);
            //        }
            //        else
            //        {
            //            rheight =  System.Math.Min(availableSize.Height * this.InternalChildren.Count, max.Height);
            //        }
            //    }
            //}

            return returnSize;
        }

        public LionZapScroller LionZapScroller
        {
            get
            {
                //if (lionZapScroller == null)
                //{
                //    lionZapScroller = BaseWPFHelpers.Helpers.FindElementOfTypeUp(_visualParent, typeof(LionZapScroller)) as LionZapScroller;
                //}
                return lionZapScroller;
            }
            set
            {
                if (lionZapScroller == value) return;
                if (lionZapScroller != null) l.Warn("Changing LionZapScroller!");
                lionZapScroller = value;

            }
        } private LionZapScroller lionZapScroller;

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_visualParent != null
                && _visualParent.RenderSize.Width != 0 // Jared added
                )
            {
                _lastVisualParentSize = _visualParent.RenderSize;
            }
            else
            {
                _lastVisualParentSize = finalSize;
            }

            Thickness padding;

            if (LionZapScroller != null)
            {
                padding = LionZapScroller.ContentPadding;
            }
            else
            {
                padding = new Thickness(0);
            }

            Size innerContentSize = new Size(
                System.Math.Max(0,
                _lastVisualParentSize.Width - padding.Left - padding.Right),
                System.Math.Max(0,
                _lastVisualParentSize.Height - padding.Top - padding.Bottom)
                );

            UIElement child;
            for (int i = 0; i < this.InternalChildren.Count; i++)
            {
                child = this.InternalChildren[i];
                Rect arrangeRect;

                if (Orientation == Orientation.Horizontal)
                {
                    arrangeRect = new Rect(
                        new Point((_lastVisualParentSize.Width * i) + padding.Left, 0 + padding.Top),
                        innerContentSize);
                }
                else
                {
                    arrangeRect = new Rect(
                        new Point(0 + padding.Left, (_lastVisualParentSize.Height * i) + padding.Top),
                        innerContentSize);
                }
                child.Arrange(arrangeRect);
            }

            int count = System.Math.Max(1, InternalChildren.Count);
            if (Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                return new Size(_lastVisualParentSize.Width * count, _lastVisualParentSize.Height);
            }
            else
            {
                return new Size(_lastVisualParentSize.Width, _lastVisualParentSize.Height * count);
            }
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            try
            {
                //l.Trace("ZapPanel VisualParent: " + this.VisualParent.GetType().Name + " " + this.VisualParent.ToString() + ((FrameworkElement)VisualParent).Tag );
            }
            catch { }
            _visualParent = this.VisualParent as FrameworkElement;
            ZapPanel_LayoutUpdated(this, new EventArgs());
        }

        #region implementation

        private void ZapPanel_LayoutUpdated(object sender, EventArgs e)
        {
            if (_visualParent != null)
            {
                if (_visualParent.RenderSize != _lastVisualParentSize)
                {
                    //InvalidateMeasure(); // Jared added
                    InvalidateArrange();
                }
            }
        }

        private Size _lastVisualParentSize;
        private FrameworkElement _visualParent;

        #endregion

        #region Orientation

        /// <summary>
        /// Orientation Dependency Property
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ZapPanel),
                new FrameworkPropertyMetadata((Orientation)Orientation.Horizontal,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the Orientation property. This dependency property 
        /// indicates ....
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion



    } //** class ZapPanel
}

#endif