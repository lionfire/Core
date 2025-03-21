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
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Extensions.Logging;

namespace LionFire.Avalon
{
    /// <summary>
    /// Interaction logic for LionListBox.xaml
    /// </summary>
    public partial class LionListBox : ListBox
    {
        private FrameworkElement highlightControl;
        private FrameworkElement hoverControl;
        private Canvas canvas;

        static LionListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LionListBox), new FrameworkPropertyMetadata(typeof(LionListBox)));
        }

        public LionListBox()
        {
            this.SelectCommand = new DelegateCommand(() =>
            {
                //l.Fatal("SelectCommand fired");
                RaiseSelected();
            }, () => true);

            InitializeComponent();
            
            
            this.SelectionChanged += new SelectionChangedEventHandler(LionListBox_SelectionChanged);
            //this.Loaded += new RoutedEventHandler(LionListBox_Loaded);
            this.MouseWheel += new MouseWheelEventHandler(LionListBox_MouseWheel);
            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(LionListBox_IsVisibleChanged);
            this.MouseMove += new MouseEventHandler(LionListBox_MouseMove);
            this.MouseLeave += new MouseEventHandler(LionListBox_MouseLeave);

            this.Loaded += LionListBox_Loaded;
        }

        bool isLoaded = false;
        void LionListBox_Loaded(object sender, RoutedEventArgs e)
        {
            //LionListBox_SelectionChanged(sender, null);
            isLoaded = true;
            InvalidateHighlight();
        }

        //protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    base.OnPreviewMouseLeftButtonDown(e);
        //    //l.Trace("OnPreviewMouseLeftButtonDown");
        //}
        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    base.OnMouseLeftButtonDown(e);
        //    e.Handled = false;
        //    //l.Trace("OnMouseLeftButtonDown");
        //}

        void LionListBox_MouseLeave(object sender, MouseEventArgs e)
        {
            InvalidateHoverHighlight();

        }

        void LionListBox_MouseMove(object sender, MouseEventArgs e)
        {
            InvalidateHoverHighlight();
        }

        //protected void OnSelectedDoSelectItem(
        //    //MouseButtonEventArgs e
        //    )
        //{
        //    //base.OnMouseLeftButtonDown(e);
        //    ((ListBoxItem)VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this)))).IsSelected = true;
        //}

        private void RaiseSelected()
        {
            var mouseOver = MouseOverItem;

            try
            {
                mouseOver.IsSelected = true;
            }
            catch
            {
                l.Error("Failed to select mouseOver element: " + mouseOver);
            }

            var dc = mouseOver.DataContext;
            this.SelectedItem = dc;

            l.Trace("RaiseSelected: " + dc);
            { var ev = Selected; if (ev != null) ev(this.SelectedItem); }
            { var ev = SelectedX; if (ev != null) ev(this.SelectedItem, null); }
        }
        public event Action<object> Selected;
        public event RoutedEventHandler SelectedX;

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            InvalidateHighlight();
        }

        void LionListBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            InvalidateHighlight();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!Focusable) return;
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (!Focusable) return;
            base.OnKeyUp(e);
        }

        #region SelectCommand

        /// <summary>
        /// SelectCommand Dependency Property
        /// </summary>
        public static readonly DependencyProperty SelectCommandProperty =
            DependencyProperty.Register("SelectCommand", typeof(ICommand), typeof(LionListBox),
                new FrameworkPropertyMetadata((ICommand)null));

        /// <summary>
        /// Gets or sets the SelectCommand property. This dependency property 
        /// indicates ....
        /// </summary>
        public ICommand SelectCommand
        {
            get { return (ICommand)GetValue(SelectCommandProperty); }
            set { SetValue(SelectCommandProperty, value); }
        }

        #endregion



        //protected override void OnMouseDown(MouseButtonEventArgs e)
        //{
        //    if (ClickSelects)
        //    {
        //        base.OnMouseDown(e);
        //        if (!ClicksHandled) e.Handled = false;
        //    }
        //}

        //protected override void OnMouseUp(MouseButtonEventArgs e)
        //{
        //    if (ClickSelects)
        //    {
        //        base.OnMouseUp(e);
        //        if (!ClicksHandled) e.Handled = false;
        //    }
        //}

        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    if (ClickSelects)
        //    {
        //        base.OnMouseLeftButtonDown(e);
        //        if (!ClicksHandled) e.Handled = false;
        //    }
        //}

        //protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        //{
        //    if (ClickSelects)
        //    {
        //        base.OnMouseUp(e);
        //        if (!ClicksHandled) e.Handled = false;
        //    }
        //}

        #region MouseWheelSelects

        /// <summary>
        /// MouseWheelSelects Dependency Property
        /// </summary>
        public static readonly DependencyProperty MouseWheelSelectsProperty =
            DependencyProperty.Register("MouseWheelSelects", typeof(bool), typeof(LionListBox),
                new FrameworkPropertyMetadata((bool)true,
                    FrameworkPropertyMetadataOptions.None));

        /// <summary>
        /// Gets or sets the MouseWheelSelects property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool MouseWheelSelects
        {
            get { return (bool)GetValue(MouseWheelSelectsProperty); }
            set { SetValue(MouseWheelSelectsProperty, value); }
        }

        #endregion

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Size size = base.ArrangeOverride(arrangeBounds);

            //InvalidateHighlight();

            return size;
        }

        public bool AlwaysHandleEvents = false; // REVIEW - why ever set this to true?  This messes up Orbat designer

        void LionListBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (MouseWheelSelects)
            {
                if (e.Delta < 0)
                {
                    if (this.SelectedIndex < this.Items.Count - 1)
                    {
                        this.SelectedIndex++;
                        e.Handled = true;
                    }
                }
                else
                {
                    if (this.SelectedIndex > 0)
                    {
                        this.SelectedIndex--;
                        e.Handled = true;
                    }
                }
            }
            if (AlwaysHandleEvents) e.Handled = true;
        }

        //void LionListBox_Loaded(object sender, RoutedEventArgs e)
        //{
        //    InvalidateHighlight();
        //}

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<Button>())
                {
                    //item.Click += new RoutedEventHandler((o, args) => SelectedItem = item);
                    item.Click += new RoutedEventHandler(OnButtonClicked);
                }
            }
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<Button>())
                {
                    item.Click -= new RoutedEventHandler(OnButtonClicked);
                }
            }
            InvalidateHighlight();

        }

        //protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        //{
        //    base.OnItemsSourceChanged(oldValue, newValue);
        //    InvalidateHighlight();
        //}

        #region CanSelectNone

        /// <summary>
        /// CanSelectNone Dependency Property
        /// </summary>
        public static readonly DependencyProperty CanSelectNoneProperty =
            DependencyProperty.Register("CanSelectNone", typeof(bool), typeof(LionListBox),
                new FrameworkPropertyMetadata((bool)false));

        /// <summary>
        /// Gets or sets the CanSelectNone property. This dependency property 
        /// indicates whether it is possible to have no item selected.
        /// This has no effect when SelectionMode is not set to Single.
        /// </summary>
        public bool CanSelectNone
        {
            get { return (bool)GetValue(CanSelectNoneProperty); }
            set { SetValue(CanSelectNoneProperty, value); }
        }

        #endregion

        #region ClickToToggle

        /// <summary>
        /// ClickToToggle Dependency Property
        /// </summary>
        public static readonly DependencyProperty ClickToToggleProperty =
            DependencyProperty.Register("ClickToToggle", typeof(bool), typeof(LionListBox),
                new FrameworkPropertyMetadata((bool)false));

        /// <summary>
        /// Gets or sets the ClickToToggle property. This dependency property 
        /// indicates whether re-clicking a selected item will deselect it.
        /// </summary>
        public bool ClickToToggle
        {
            get { return (bool)GetValue(ClickToToggleProperty); }
            set { SetValue(ClickToToggleProperty, value); }
        }

        #endregion

       
        private void OnButtonClicked(object sender, RoutedEventArgs args)
        {
            bool isToggle = ClickToToggle || (Keyboard.Modifiers | ModifierKeys.Control) != ModifierKeys.None;

            var fe = args.Source as FrameworkElement;
            if (fe != null)
            {
                if (SelectionMode != System.Windows.Controls.SelectionMode.Single)
                {
                    if (isToggle)
                    {
                        // Toggle 
                        bool isSelected = this.SelectedItems.Contains(fe);
                        if (isSelected)
                        {
                            this.SelectedItems.Remove(fe);
                        }
                        else
                        {
                            this.SelectedItems.Add(fe);
                        }
                        // NOTE: Does not respect CanSelectNone
                        return;
                    }
                }
                else // Single
                {
                    if (isToggle && SelectedItem == fe)
                    {
                        SelectedItem = null;
                    }
                    else
                    {
                        SelectedItem = fe;
                    }
                }
            }
        }

        private void _GetTemplateControls()
        {
            highlightControl = this.GetTemplateChild("HighlightControl") as FrameworkElement;
            hoverControl = this.GetTemplateChild("HoverControl") as FrameworkElement;
            canvas = this.GetTemplateChild("Canvas") as Canvas;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _GetTemplateControls();

            InvalidateHighlight();
            
        }

        private FrameworkElement FindByName(string name, FrameworkElement root)
        {
            Stack<FrameworkElement> tree = new Stack<FrameworkElement>();
            tree.Push(root);

            while (tree.Count > 0)
            {
                FrameworkElement current = tree.Pop();
                if (current.Name == name)
                    return current;

                int count = VisualTreeHelper.GetChildrenCount(current);
                for (int i = 0; i < count; ++i)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(current, i);
                    if (child is FrameworkElement)
                        tree.Push((FrameworkElement)child);
                }
            }

            return null;
        }

        //private void myList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (myList.SelectedItem != null)
        //    {
        //        object o = myList.SelectedItem;
        //        ListViewItem lvi = (ListViewItem)myList.ItemContainerGenerator.ContainerFromItem(o);
        //        TextBox tb = FindByName("myBox", lvi) as TextBox;

        //        if (tb != null)
        //            tb.Dispatcher.BeginInvoke(new Func<bool>(tb.Focus));
        //    }
        //}

        #region AnimationDuration

        /// <summary>
        /// AnimationDuration Dependency Property
        /// </summary>
        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(Duration), typeof(LionListBox),
                new FrameworkPropertyMetadata((Duration)TimeSpan.FromMilliseconds(600)));

        /// <summary>
        /// Gets or sets the AnimationDuration property. This dependency property 
        /// indicates ....
        /// </summary>
        public Duration AnimationDuration
        {
            get { return (Duration)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        #endregion

        //Duration animDuration = new Duration(TimeSpan.FromMilliseconds(600));
        private static readonly ILogger l = Log.Get();

        void LionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //l.Trace("LionListBox_SelectionChanged - " + SelectedIndex);

            if (SelectedIndex < 0 && Items != null && Items.Count > 0)
            {
                
                if (!CanSelectNone)
                {
                    // StackOverflow here?
                    l.Debug("SelectedIndex < 0 && Items.Count > 0 " + Environment.StackTrace);
                    SelectedIndex = 0;
                    //InvalidateHighlight(); - occurs after changing selectedindex?
                    return;
                }
            }
            InvalidateHighlight();
        }

        public class HitResult
        {
            public DependencyObject FE;
        }


        public ListBoxItem MouseOverItem
        {
            get
            {
                HitResult hr = new HitResult();
                //FrameworkElement hoverItem = null;

                VisualTreeHelper.HitTest(this, new HitTestFilterCallback(dob =>
                {
                    //    l.Trace("HitTestFilter: " + dob);
                    if (dob as ListBoxItem != null)
                    {
                        hr.FE = dob;
                        return HitTestFilterBehavior.Stop;
                    }
                    return HitTestFilterBehavior.Continue;
                }),
                    new HitTestResultCallback(dob =>
                    {

                        hr.FE = dob.VisualHit as ListBoxItem;
                        return HitTestResultBehavior.Continue;
                    }), new PointHitTestParameters(Mouse.GetPosition(this)));


                ListBoxItem fe = hr.FE as ListBoxItem;

                return fe;
            }
        }

        public void InvalidateHoverHighlight()
        {
            try
            {
                if (this.hoverControl == null)
                {
                    _GetTemplateControls();

                }
                if (this.hoverControl == null || canvas == null)
                {
                    //l.Fatal("no highlightcontrol or canvas");
                    return;
                }

                FrameworkElement fe = MouseOverItem;

                //  l.Fatal("HoverTemplate: " + this.HoverTemplate);
                if (fe == null)
                {
                    hoverControl.Width = 0;
                    hoverControl.Height = 0;
                    //l.Trace("No mouseover item: " + fe);
                    hoverControl.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    hoverControl.Width = fe.ActualWidth;
                    hoverControl.Height = fe.ActualHeight;
                    hoverControl.Opacity = 1.0;

                    //l.Fatal("Got mouseover item: " + fe);

                    //Point point = Mouse.GetPosition(this.canvas);
                    //l.Trace("fe pts" + fe.PointToScreen(new Point(0, 0)));
                    //l.Trace("can pts" + canvas.PointToScreen(new Point(0, 0)));

                    if (VisualTreeHelper.GetParent(canvas) != null && this.IsVisible
                        && PresentationSource.FromVisual(canvas) != null // see http://stackoverflow.com/questions/2154211/in-wpf-under-what-circumstances-does-visual-pointfromscreen-throw-invalidoperat
                        )
                    {
                        Point point;
                        if (fe.Visibility == System.Windows.Visibility.Visible)
                        {
                            point = canvas.PointFromScreen(new Point(0, 0));
                        }
                        else
                        {
                            var feScreenPoint = fe.PointToScreen(new Point(0, 0));
                            point = canvas.PointFromScreen(feScreenPoint);
                        }
                            //l.Trace("point" + point);

                        Canvas.SetLeft(hoverControl, point.X);
                        Canvas.SetTop(hoverControl, point.Y);
                        hoverControl.Visibility = System.Windows.Visibility.Visible;
                    }
                }
                //    ListBoxItem hoverItem = null;
                //foreach (UIElement element in this.VisualCh)
                //{
                //    if (element is ListBoxItem)
                //    {
                //        hoverIndex = myListBox.Items.IndexOf(element);
                //        break;
                //    }
                //}

                //l.Trace( "Currently hovering over item index " + hoverIndex.ToString());

            }
            catch (Exception ex)
            {
                l.Warn("InvalidateHoverHighlight exception: " + ex.ToString());
            }
        }

        public void InvalidateHighlight()
        {
            if (!isLoaded) return;
            //l.Trace("LionListBox.InvalidateHighlight() " + Environment.StackTrace);
            //InvalidateHoverHighlight();

            if (this.highlightControl == null)
            {
                _GetTemplateControls();

            }
            if (this.highlightControl == null || canvas == null)
            {
                //l.Fatal("no highlightcontrol or canvas");
                return;
            }

            object si = this.SelectedItem;
            ListBoxItem selectedItem = (ListBoxItem)this.ItemContainerGenerator.ContainerFromItem(si);

            double newHeight;
            double newWidth;
            double newOpacity;

            IEasingFunction easingFunction = new ElasticEase() { EasingMode = EasingMode.EaseOut, Springiness = 18, Oscillations = 1 };

            Duration animDuration = this.AnimationDuration;

            if (selectedItem != null)
            {
                newWidth = selectedItem.ActualWidth;
                newHeight = selectedItem.ActualHeight;
                newOpacity = 1;
                //highlightControl.Visibility = Visibility.Visible; // Never set back to hidden

                Point point = selectedItem.TranslatePoint(new Point(0, 0), this);

                //if (Orientation == System.Windows.Controls.Orientation.Vertical)
                //{
                //    point.X = 0;
                //    newWidth = this.ActualWidth;
                //}
                //else
                //{
                //    point.Y = 0;
                //    newHeight = this.ActualHeight;
                //}

                if (highlightControl.Width == 0)
                {
                    highlightControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    //highlightControl.MinWidth = newWidth;
                    //highlightControl.MaxWidth = newWidth;
                    highlightControl.Width = newWidth;
                    highlightControl.BeginAnimation(FrameworkElement.WidthProperty, new DoubleAnimation(newWidth, animDuration) { EasingFunction = easingFunction });
                }
                else
                {
                    highlightControl.BeginAnimation(FrameworkElement.WidthProperty, new DoubleAnimation(newWidth, animDuration) { EasingFunction = easingFunction });
                }

                if (highlightControl.Height == 0)
                {
                    highlightControl.VerticalAlignment = System.Windows.VerticalAlignment.Center;

                    //highlightControl.MinHeight = newHeight;
                    //highlightControl.MaxHeight = newHeight;
                    highlightControl.Height = newHeight;
                    highlightControl.BeginAnimation(FrameworkElement.HeightProperty, new DoubleAnimation(newHeight, animDuration) { EasingFunction = easingFunction });

                }
                else
                {
                    highlightControl.BeginAnimation(FrameworkElement.HeightProperty, new DoubleAnimation(newHeight, animDuration) { EasingFunction = easingFunction });
                }

                if (highlightControl.Opacity == 0)
                {
                    highlightControl.SetValue(Canvas.TopProperty, point.Y);
                    highlightControl.SetValue(Canvas.LeftProperty, point.X);
                }
                else
                {
                    highlightControl.BeginAnimation(Canvas.TopProperty, new DoubleAnimation(point.Y, animDuration) { EasingFunction = easingFunction });
                    highlightControl.BeginAnimation(Canvas.LeftProperty, new DoubleAnimation(point.X, animDuration) { EasingFunction = easingFunction });
                }

                
            }
            else
            {
                newWidth = 0;
                newHeight = 0;
                newOpacity = 0;

                //highlightControl.Height = 0;
                //highlightControl.Width = 0;
            }

            if (highlightControl.Opacity != newOpacity)
            {
                if (AnimateOpacity)
                {
                    highlightControl.BeginAnimation(FrameworkElement.OpacityProperty, new DoubleAnimation(newOpacity, animDuration) { EasingFunction = easingFunction });
                }
                else
                {
                    //highlightControl.Visibility = Visibility.Hidden;
                    highlightControl.Opacity = newOpacity;
                }
            }
            //l.Trace("highlightControl.Opacity = " + highlightControl.Opacity + " - " + highlightControl.Width + "x" + highlightControl.Height + " @ " +  highlightControl.GetValue(Canvas.LeftProperty) + ", " + highlightControl.GetValue(Canvas.TopProperty));
            
        }

        public bool AnimateOpacity = true;

        #region HighlightTemplate

        /// <summary>
        /// HighlightTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty HighlightTemplateProperty =
            DependencyProperty.Register("HighlightTemplate", typeof(ControlTemplate), typeof(LionListBox),
                new FrameworkPropertyMetadata((ControlTemplate)null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the HighlightTemplate property. This dependency property 
        /// indicates the template of the highlighting object that will hover over the selected item.
        /// </summary>
        public ControlTemplate HighlightTemplate
        {
            get { return (ControlTemplate)GetValue(HighlightTemplateProperty); }
            set { SetValue(HighlightTemplateProperty, value); }
        }

        #endregion

        #region HoverTemplate

        /// <summary>
        /// HoverTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty HoverTemplateProperty =
            DependencyProperty.Register("HoverTemplate", typeof(ControlTemplate), typeof(LionListBox),
                new FrameworkPropertyMetadata((ControlTemplate)null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the HoverTemplate property. This dependency property 
        /// indicates the template of the highlighting object that will hover over the selected item.
        /// </summary>
        public ControlTemplate HoverTemplate
        {
            get { return (ControlTemplate)GetValue(HoverTemplateProperty); }
            set { SetValue(HoverTemplateProperty, value); }
        }

        #endregion

        #region Orientation

        /// <summary>
        /// Orientation Dependency Property
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(LionListBox),
                new FrameworkPropertyMetadata((Orientation)Orientation.Vertical,
                    FrameworkPropertyMetadataOptions.AffectsArrange));

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

        //public bool ClicksHandled = true;
        //public bool ClickSelects = true;

    }
}
