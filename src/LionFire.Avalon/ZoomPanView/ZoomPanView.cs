// Based on Stackoverflow answer:
// http://stackoverflow.com/a/11897316/208304
// Retrieved on Jun 20, 2016

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LionFire.Avalon
{


    /// <summary>
    /// To use:
    ///  - Background Image: Panel.ZIndex="-2"
    ///  - Background Objects: Panel.ZIndex="-1"
    ///  - Set ScrollableZIndex - this is what the user can grab to drag the view.  It can be transparent.  It has a ZIndex of 0
    ///  - Add more items to the content
    /// </summary>
    /// <remarks>
    /// TODO: Smooth zoom
    /// TODO: Pan momentum
    /// </remarks>
    public class ZoomPanView : Canvas
    {
        #region (Private) State

        //private Point startingTranslation;
        private Point startingDragPosition;

        private double CurrentZoom = 1.0;

        List<FrameworkElement> scrolledElements = new List<FrameworkElement>();

        bool isDragging = false;

        #endregion

        #region Attached Properties - Pan and Zoom for Parallax effect

        #region PanMultiplier

        /// <summary>
        /// PanMultiplier Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty PanMultiplierProperty =
            DependencyProperty.RegisterAttached("PanMultiplier", typeof(double), typeof(ZoomPanView),
                new FrameworkPropertyMetadata((double)1.0));

        /// <summary>
        /// Gets the PanMultiplier property. This dependency property 
        /// indicates ....
        /// </summary>
        public static double GetPanMultiplier(DependencyObject d)
        {
            return (double)d.GetValue(PanMultiplierProperty);
        }

        /// <summary>
        /// Sets the PanMultiplier property. This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetPanMultiplier(DependencyObject d, double value)
        {
            d.SetValue(PanMultiplierProperty, value);
        }

        #endregion

        #region ZoomMultiplier

        /// <summary>
        /// ZoomMultiplier Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty ZoomMultiplierProperty =
            DependencyProperty.RegisterAttached("ZoomMultiplier", typeof(double), typeof(ZoomPanView),
                new FrameworkPropertyMetadata((double)1.0));

        /// <summary>
        /// Gets the ZoomMultiplier property. This dependency property 
        /// indicates ....
        /// </summary>
        public static double GetZoomMultiplier(DependencyObject d)
        {
            return (double)d.GetValue(ZoomMultiplierProperty);
        }

        /// <summary>
        /// Sets the ZoomMultiplier property. This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetZoomMultiplier(DependencyObject d, double value)
        {
            d.SetValue(ZoomMultiplierProperty, value);
        }

        #endregion

        #endregion

        #region Parameters

        #region CenterAllChildren

        /// <summary>
        /// If true, CenterAllChildren in the middle of the canvas.  To position items, adjust the RenderTransform (TranslationTransform), wich will be preserved.
        /// </summary>
        public static readonly DependencyProperty CenterAllChildrenProperty =
            DependencyProperty.Register("CenterAllChildren", typeof(bool), typeof(ZoomPanView),
                new FrameworkPropertyMetadata((bool)false,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the CenterAllChildren property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool CenterAllChildren {
            get { return (bool)GetValue(CenterAllChildrenProperty); }
            set { SetValue(CenterAllChildrenProperty, value); }
        }

        #endregion

        #region ZoomAmount

        /// <summary>
        /// ZoomAmount Dependency Property
        /// </summary>
        public static readonly DependencyProperty ZoomAmountProperty =
            DependencyProperty.Register("ZoomAmount", typeof(double), typeof(ZoomPanView),
                new FrameworkPropertyMetadata((double)0.2));

        /// <summary>
        /// Gets or sets the ZoomAmount property. This dependency property 
        /// indicates ....
        /// </summary>
        public double ZoomAmount {
            get { return (double)GetValue(ZoomAmountProperty); }
            set { SetValue(ZoomAmountProperty, value); }
        }

        #endregion

        #region MinZoom

        /// <summary>
        /// MinZoom Dependency Property
        /// </summary>
        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(ZoomPanView),
                new FrameworkPropertyMetadata((double)0.333));

        /// <summary>
        /// Gets or sets the MinZoom property. This dependency property 
        /// indicates ....
        /// </summary>
        public double MinZoom {
            get { return (double)GetValue(MinZoomProperty); }
            set { SetValue(MinZoomProperty, value); }
        }

        #endregion

        #region MaxZoom

        /// <summary>
        /// MaxZoom Dependency Property
        /// </summary>
        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(ZoomPanView),
                new FrameworkPropertyMetadata((double)2.5));

        /// <summary>
        /// Gets or sets the MaxZoom property. This dependency property 
        /// indicates ....
        /// </summary>
        public double MaxZoom {
            get { return (double)GetValue(MaxZoomProperty); }
            set { SetValue(MaxZoomProperty, value); }
        }

        #endregion

        #endregion

        #region Layers

        //private const int NonScrollableZIndex = 30;
        private const int ScrollableZIndex = 0;

        //#region NonScrollableLayer

        ///// <summary>
        ///// NonScrollableLayer Dependency Property.  Place 
        ///// </summary>
        //public static readonly DependencyProperty NonScrollableLayerProperty =
        //    DependencyProperty.Register("NonScrollableLayer", typeof(FrameworkElement), typeof(ZoomPanView),
        //        new FrameworkPropertyMetadata((FrameworkElement)null,
        //            FrameworkPropertyMetadataOptions.AffectsRender,
        //            new PropertyChangedCallback(OnNonScrollableLayerChanged)));

        ///// <summary>
        ///// Gets or sets the NonScrollableLayer property. This dependency property 
        ///// indicates ....
        ///// </summary>
        //public FrameworkElement NonScrollableLayer {
        //    get { return (FrameworkElement)GetValue(NonScrollableLayerProperty); }
        //    set { SetValue(NonScrollableLayerProperty, value); }
        //}

        ///// <summary>
        ///// Handles changes to the NonScrollableLayer property.
        ///// </summary>
        //private static void OnNonScrollableLayerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    ZoomPanView target = (ZoomPanView)d;
        //    FrameworkElement oldNonScrollableLayer = (FrameworkElement)e.OldValue;
        //    if (oldNonScrollableLayer != null)
        //    {
        //        target.Children.Remove(oldNonScrollableLayer);
        //    }
        //    FrameworkElement newNonScrollableLayer = target.NonScrollableLayer;
        //    if (newNonScrollableLayer != null)
        //    {
        //        target.Children.Add(newNonScrollableLayer);
        //        Panel.SetZIndex(newNonScrollableLayer, NonScrollableZIndex);
        //        target.AttachTransforms(newNonScrollableLayer);

        //    }
        //    target.OnNonScrollableLayerChanged(oldNonScrollableLayer, newNonScrollableLayer);
        //}

        ///// <summary>
        ///// Provides derived classes an opportunity to handle changes to the NonScrollableLayer property.
        ///// </summary>
        //protected virtual void OnNonScrollableLayerChanged(FrameworkElement oldNonScrollableLayer, FrameworkElement newNonScrollableLayer)
        //{
        //    if (oldNonScrollableLayer != null)
        //    {
        //        var fe = oldNonScrollableLayer;
        //        if (fe.Name == "ScrollableLayer")
        //        {
        //            fe.Name = null;
        //            if (fe.Name == "NonScrollableLayer")
        //            {
        //                fe.Name = null;
        //            }
        //        }
        //    }

        //    if (newNonScrollableLayer != null)
        //    {
        //        var fe = newNonScrollableLayer;
        //        if (fe.Name == null)
        //        {
        //            fe.Name = "NonScrollableLayer";
        //        }
        //    }
        //}

        //#endregion


        #region ScrollableLayer

        /// <summary>
        /// ScrollableLayer Dependency Property
        /// </summary>
        public static readonly DependencyProperty ScrollableLayerProperty =
            DependencyProperty.Register("ScrollableLayer", typeof(FrameworkElement), typeof(ZoomPanView),
                new FrameworkPropertyMetadata((FrameworkElement)null,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    new PropertyChangedCallback(OnScrollableLayerChanged)));

        /// <summary>
        /// Gets or sets the ScrollableLayer property. This dependency property 
        /// indicates ....
        /// </summary>
        public FrameworkElement ScrollableLayer {
            get { return (FrameworkElement)GetValue(ScrollableLayerProperty); }
            set { SetValue(ScrollableLayerProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ScrollableLayer property.
        /// </summary>
        private static void OnScrollableLayerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZoomPanView target = (ZoomPanView)d;
            FrameworkElement oldScrollableLayer = (FrameworkElement)e.OldValue;
            FrameworkElement newScrollableLayer = target.ScrollableLayer;
            target.OnScrollableLayerChanged(oldScrollableLayer, newScrollableLayer);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ScrollableLayer property.
        /// </summary>
        protected virtual void OnScrollableLayerChanged(FrameworkElement oldScrollableLayer, FrameworkElement newScrollableLayer)
        {
            if (oldScrollableLayer != null)
            {
                var fe = oldScrollableLayer;
                Children.Remove(fe);
                DetachElement(fe);
                fe.MouseLeftButtonDown -= OnMouseLeftButtonDown;
                fe.MouseLeftButtonUp -= OnMouseLeftUp;
                fe.MouseMove -= OnMouseMove;
                if (fe.Name == "ScrollableLayer")
                {
                    fe.Name = null;
                }
            }

            if (newScrollableLayer != null)
            {
                var fe = newScrollableLayer;
                Children.Add(fe);
                SetZIndex(fe, ScrollableZIndex);
                AttachTransforms(fe);
                fe.MouseLeftButtonDown += OnMouseLeftButtonDown;
                fe.MouseLeftButtonUp += OnMouseLeftUp;
                fe.MouseMove += OnMouseMove;
                if (fe.Name == null)
                {
                    fe.Name = "ScrollableLayer";
                }
            }
        }


        #endregion

        #endregion

        #region Construction

        public ZoomPanView()
        {
            Loaded += OnLoaded;

            ClipToBounds = true;

#if true
            // NOTE I use a border as the first child, to which I add the image. I do this so the panned image doesn't partly obscure the control's border.
            // In case you are going to use rounder corner's on this control, you may to update your clipping, as in this example:
            // http://wpfspark.wordpress.com/2011/06/08/clipborder-a-wpf-border-that-clips/
            var border = new Border
            {
                Name = "ImageHolderBorder",
                IsManipulationEnabled = true,
                ClipToBounds = true
                //,  Child = image
            };
            //Child = border;
            var directChild = border;
#else
            var child = image;
#endif
            this.Children.Add(directChild);
            Panel.SetZIndex(directChild, ScrollableZIndex - 6);

            //NOTE I apply the manipulation to the border, and not to the image itself (which caused stability issues when translating)!
            border.ManipulationDelta += (o, e) =>
            {
                foreach (var fe in scrolledElements)
                {

                    // TODO: use scale for zoom/pan
                    var st = (ScaleTransform)((TransformGroup)fe.RenderTransform).Children.First(tr => tr is ScaleTransform);
                    var tt = (TranslateTransform)((TransformGroup)fe.RenderTransform).Children.First(tr => tr is TranslateTransform);

                    st.ScaleX *= e.DeltaManipulation.Scale.X;
                    st.ScaleY *= e.DeltaManipulation.Scale.X;
                    tt.X += e.DeltaManipulation.Translation.X;
                    tt.Y += e.DeltaManipulation.Translation.Y;
                }
                e.Handled = true;
            };

            this.LayoutUpdated += ZoomPanViewControl_LayoutUpdated;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var result = base.ArrangeOverride(arrangeSize);

            foreach (var child in Children.OfType<FrameworkElement>())
            {
                if (GetPanMultiplier(child) > 0 || GetZoomMultiplier(child) > 0)
                {
                    AttachTransforms(child);
                }
            }
            return result;
        }

        private void ZoomPanViewControl_LayoutUpdated(object sender, EventArgs e)
        {
            foreach (var child in Children.OfType<FrameworkElement>())
            {
                if (CenterAllChildren || child.HorizontalAlignment == HorizontalAlignment.Center)
                {
                    SetLeft(child, -((child.ActualWidth - this.ActualWidth) / 2));
                }
                if (CenterAllChildren || child.VerticalAlignment == VerticalAlignment.Center)
                {
                    SetTop(child, -((child.ActualWidth - this.ActualWidth) / 2));
                }
            }
        }

        #endregion

#if false
        private Image image;
        #region ImagePath

        /// <summary>
        ///     ImagePath Dependency Property
        /// </summary>
        public static readonly DependencyProperty ImagePathProperty = DependencyProperty.Register("ImagePath", typeof(string), typeof(ZoomPanView), new FrameworkPropertyMetadata(string.Empty, OnImagePathChanged));

        /// <summary>
        ///     Gets or sets the ImagePath property. This dependency property 
        ///     indicates the path to the image file.
        /// </summary>
        public string ImagePath {
            get { return (string)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the ImagePath property.
        /// </summary>
        private static void OnImagePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ZoomPanView)d;
            var oldImagePath = (string)e.OldValue;
            var newImagePath = target.ImagePath;
            target.OnImagePathChanged(oldImagePath, newImagePath);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to the ImagePath property.
        /// </summary>
        protected virtual void OnImagePathChanged(string oldImagePath, string newImagePath)
        {
            ReloadImage(newImagePath);
        }

        #endregion

                /// <summary>
        /// Load the image (and do not keep a hold on it, so we can delete the image without problems)
        /// </summary>
        /// <see cref="http://blogs.vertigo.com/personal/ralph/Blog/Lists/Posts/Post.aspx?ID=18"/>
        /// <param name="path"></param>
        private void ReloadImage(string path)
        {
            if (image == null)
            {
                image = new Image
                {
                    Name = "TestImage",
                    Width = 10,
                    Height = 100
                    //IsManipulationEnabled = true,
                };
            }
            try
            {
                ResetPanZoom();
                // load the image, specify CacheOption so the file is not locked--
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                bitmapImage.EndInit();
                image.Source = bitmapImage;
            }
            catch (SystemException e)
            {
                Console.WriteLine(e.Message);
            }
        }

#endif

        #region Attach / Detach Child

        private void AttachTransforms(FrameworkElement fe)
        {
            if (scrolledElements.Contains(fe)) return;
            fe.RenderTransformOrigin = new Point(0.5, 0.5);
            if (fe.RenderTransform == null)
            {
                fe.RenderTransform = new TransformGroup
                {
                    Children = new TransformCollection { new ScaleTransform(), new TranslateTransform() }
                };
            }
            else
            {
                var tg = fe.RenderTransform as TransformGroup;
                if (tg == null)
                {
                    tg = new TransformGroup();
                    tg.Children.Add(fe.RenderTransform);
                    fe.RenderTransform = tg;
                }
                if (!tg.Children.Where(t => t is ScaleTransform).Any())
                {
                    tg.Children.Add(new ScaleTransform());
                }
                if (!tg.Children.Where(t => t is TranslateTransform).Any())
                {
                    tg.Children.Add(new TranslateTransform());
                }
            }
            scrolledElements.Add(fe);
            fe.MouseWheel += OnMouseWheel;
        }

        private void DetachElement(FrameworkElement fe)
        {
            scrolledElements.Remove(fe);
            fe.MouseWheel -= OnMouseWheel;
        }

        #endregion

        #region Mouse Events


        private void OnMouseWheel(object s, MouseWheelEventArgs e)
        {

            var zoom = e.Delta > 0 ? ZoomAmount : -ZoomAmount;
            var newZoom = CurrentZoom + zoom;
            if (newZoom > MaxZoom) { newZoom = MaxZoom; }
            if (newZoom < MinZoom) { newZoom = MinZoom; }
            var zoomDelta = newZoom - CurrentZoom;
            CurrentZoom = newZoom;

            foreach (var fe in scrolledElements)
            {
                try
                {
                    var position = e.GetPosition(fe);
                    if (fe.ActualHeight == 0)
                    {
                        Debug.WriteLine("fe.ActualHeight == 0 for " + fe.GetType().Name + " " + fe.Name);
                        continue;
                    }
                    if (double.IsNaN(position.X))
                    {
                        Debug.WriteLine("double.IsNaN(position.X) for " + fe.GetType().Name + " " + fe.Name);
                        continue;
                    }

                    fe.RenderTransformOrigin = new Point(position.X / fe.ActualWidth, position.Y / fe.ActualHeight);
                    var zoomMultiplier = GetZoomMultiplier(fe);
                    var st = (ScaleTransform)((TransformGroup)fe.RenderTransform).Children.First(tr => tr is ScaleTransform);
                    st.ScaleX += zoomDelta * zoomMultiplier;
                    st.ScaleY += zoomDelta * zoomMultiplier;

                }
                catch (Exception ex) { Debug.WriteLine(ex); } // TODO EMPTYCATCH
            }
            e.Handled = true;
        }




        private void OnMouseLeftButtonDown(object s, MouseButtonEventArgs e)
        {
            //if (e.ClickCount == 2)
            //    ResetPanZoom();
            //else
            {
                ScrollableLayer.CaptureMouse();
                var translateTransform = (TranslateTransform)((TransformGroup)ScrollableLayer.RenderTransform).Children.First(tr => tr is TranslateTransform);
                startingDragPosition = e.GetPosition(this);
                //startingTranslation = new Point(translateTransform.X, translateTransform.Y);
                //Debug.WriteLine("Left Down: startingTranslation - " + startingTranslation + ", startingDragPosition - " + startingDragPosition);
                isDragging = true;
            }
            e.Handled = true;

            var dict = new Dictionary<FrameworkElement, Point>();
            foreach (var fe in scrolledElements)
            {
                var translateTransform = (TranslateTransform)((TransformGroup)fe.RenderTransform).Children.First(tr => tr is TranslateTransform);
                //startingTranslation = new Point(translateTransform.X, translateTransform.Y);
                dict.Add(fe, new Point(translateTransform.X, translateTransform.Y));
            }
            fes = dict;
        }
        private void OnMouseLeftUp(object s, MouseButtonEventArgs e)
        {
            isDragging = false;
            ScrollableLayer.ReleaseMouseCapture();
        }
        private void OnMouseMove(object s, MouseEventArgs e)
        {
            if (!isDragging) return;
            if (!ScrollableLayer.IsMouseCaptured) return;

            Vector v;
            v = startingDragPosition - e.GetPosition(this);
            //Debug.WriteLine("v " + v);

            foreach (var kvp in fes)
            {
                var fe = kvp.Key;
                var startingTranslation = kvp.Value;
                var translateTransform = (TranslateTransform)((TransformGroup)fe.RenderTransform).Children.First(tr => tr is TranslateTransform);

                var panMultiplier = GetPanMultiplier(fe);

                translateTransform.X = startingTranslation.X - (v.X * panMultiplier);
                translateTransform.Y = startingTranslation.Y - (v.Y * panMultiplier);
            }
            e.Handled = true;
        }
        Dictionary<FrameworkElement, Point> fes = new Dictionary<FrameworkElement, Point>();

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
        }

        #endregion

        #region (Public) Methods

        public void ResetPanZoom()
        {
            foreach (var fe in scrolledElements)
            {
                var st = (ScaleTransform)((TransformGroup)fe.RenderTransform).Children.First(tr => tr is ScaleTransform);
                var tt = (TranslateTransform)((TransformGroup)fe.RenderTransform).Children.First(tr => tr is TranslateTransform);
                st.ScaleX = st.ScaleY = 1;
                tt.X = tt.Y = 0;
                fe.RenderTransformOrigin = new Point(0.5, 0.5);
            }
        }


        #endregion

    }
}
