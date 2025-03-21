using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LionFire.Avalon
{
    public partial class AnimatedPanes : ContentControl
    {

        #region PaneContentTemplate

        /// <summary>
        /// PaneContentTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty PaneContentTemplateProperty =
            DependencyProperty.Register("PaneContentTemplate", typeof(DataTemplate), typeof(AnimatedPanes),
                new FrameworkPropertyMetadata((DataTemplate)null,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    new PropertyChangedCallback(OnPaneContentTemplateChanged)));

        /// <summary>
        /// Gets or sets the PaneContentTemplate property. This dependency property 
        /// indicates ....
        /// </summary>
        public DataTemplate PaneContentTemplate
        {
            get { return (DataTemplate)GetValue(PaneContentTemplateProperty); }
            set { SetValue(PaneContentTemplateProperty, value); }
        }

        /// <summary>
        /// Handles changes to the PaneContentTemplate property.
        /// </summary>
        private static void OnPaneContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatedPanes target = (AnimatedPanes)d;
            DataTemplate oldPaneContentTemplate = (DataTemplate)e.OldValue;
            DataTemplate newPaneContentTemplate = target.PaneContentTemplate;
            target.OnPaneContentTemplateChanged(oldPaneContentTemplate, newPaneContentTemplate);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the PaneContentTemplate property.
        /// </summary>
        protected virtual void OnPaneContentTemplateChanged(DataTemplate oldPaneContentTemplate, DataTemplate newPaneContentTemplate)
        {
            if (PaneA != null) PaneA.ContentTemplate = newPaneContentTemplate;
            if (PaneB != null) PaneB.ContentTemplate = newPaneContentTemplate;

            foreach (var paneItem in this.panes)
            {
                paneItem.Container.ContentTemplate = newPaneContentTemplate;
            }
        }

        #endregion

        #region Transforms

        private TransformGroup AGroup;
        private TranslateTransform ATranslate;
        private ScaleTransform AScale;

        private TransformGroup BGroup;
        private TranslateTransform BTranslate;
        private ScaleTransform BScale;

        #endregion

        #region Parameters

        public IEasingFunction EasingFunction { get; set; }
        public double AccelerationRatio = 0.0;
        public double DecelerationRatio = 0.0;
        public Duration Duration = new Duration(TimeSpan.FromMilliseconds(400));

        private readonly bool UseGroup = false;

        static int a = 1;
        public Func<FrameworkElement> ContentFactory = () => 
            new Button()
            {
                Margin = new Thickness(5),
                Content = "Pane " + a,
            };
            

        #region Columns

        public int Columns
        {
            get { return columns; }
            set
            {
                if (columns == value) return;
                columns = value;
                ApplyTemplate();
            }
        } private int columns = 2;

        #endregion

        #endregion

        #region Construction

        public AnimatedPanes()
        {
            //InitializeComponent();

            this.ClipToBounds = true;

            // Good ones:
            //EasingFunction = new BackEase()
            //{
            //    Amplitude = 0.2,
            //    EasingMode = EasingMode.EaseOut,
            //};
            EasingFunction = new ExponentialEase()
            {
                Exponent = 12,
                EasingMode = EasingMode.EaseOut,
            };


            //

            //EasingFunction = new PowerEase()
            //{
            //    Power = 2,
            //    EasingMode = EasingMode.EaseOut,
            //};
            //EasingFunction = new CubicEase()
            //{
            //    EasingMode = EasingMode.EaseOut,
            //};
            //EasingFunction = new BounceEase()
            //{
            //    Bounces = 1,
            //    Bounciness = 10,
            //    EasingMode = EasingMode.EaseOut,
            //};
            //EasingFunction = new ElasticEase()
            //{
            //    Oscillations = 1,
            //     //Springiness = 
            //    EasingMode = EasingMode.EaseOut,
            //};

            this.Loaded += AnimatedABControl_Loaded;
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.AnimateToColumns(false);
        }

        void AnimatedABControl_Loaded(object sender, RoutedEventArgs e)
        {
            AnimateToColumns(false);
        }

        Grid grid;
        private void BuildOneColumn()
        {
            PaneA = new ContentControl();
            PaneB = new ContentControl();

            // REVIEW: Move to default template?
            PaneB.Visibility = System.Windows.Visibility.Hidden;

            grid.Children.Add(PaneA);
            grid.Children.Add(PaneB);

            ATranslate = new TranslateTransform(0, 0);
            BTranslate = new TranslateTransform(0, 0);

            if (UseGroup)
            {
                AScale = new ScaleTransform(0, 0);
                BScale = new ScaleTransform(0, 0);

                AGroup = new TransformGroup();
                AGroup.Children.Add(ATranslate);
                AGroup.Children.Add(AScale);
                PaneA.RenderTransform = AGroup;

                BGroup = new TransformGroup();
                BGroup.Children.Add(BTranslate);
                BGroup.Children.Add(BScale);
                PaneB.RenderTransform = BGroup;
            }
            else
            {
                PaneA.RenderTransform = ATranslate;
                PaneB.RenderTransform = BTranslate;
            }

            PaneA.Content = ContentA = ContentFactory();
            PaneB.Content = ContentB = ContentFactory();
        }


        class PaneItem
        {
            static int id = 0;
            public int Id;
            public PaneItem()
            {
                Id = id++;
            }
            public ContentControl Container;
            public FrameworkElement Content;

            public DoubleAnimation AnimX = new DoubleAnimation();
            public DoubleAnimation AnimY = new DoubleAnimation();
            public TransformGroup TransformGroup;
            public ScaleTransform ScaleTransform;
            public TranslateTransform TranslateTransform;
            //public ColumnDefinition ColumnDefinition;

            public override string ToString()
            {
                
                if (Content != null)
                {
                    if (Content.DataContext != null)
                    {
                        return Content.DataContext.ToString() + "-" + Id;
                    }
                }
                return base.ToString() + "-" + Id;
            }
        }

        List<PaneItem> items = new List<PaneItem>();
        List<PaneItem> panes = new List<PaneItem>();

        private PaneItem extra;

        private PaneItem CreatePaneItem()
        {
            var pi = new PaneItem()
            {
                Container = new ContentControl(),
                //ScaleTransform = new ScaleTransform(0, 0),
                TransformGroup = new TransformGroup(),
                TranslateTransform = new TranslateTransform(),
                Content = ContentFactory(),
                //ColumnDefinition = colDef,
            };
            pi.Container.ContentTemplate = this.PaneContentTemplate;
            pi.Container.Content = pi.Content;
            //pi.TransformGroup.Children.Add(pi.TranslateTransform);
            //pi.TransformGroup.Children.Add(pi.ScaleTransform);
            pi.Container.RenderTransform = pi.TranslateTransform;

            Storyboard.SetTarget(pi.AnimX, pi.Container);
            Storyboard.SetTarget(pi.AnimY, pi.Container);

            Storyboard.SetTargetProperty(pi.AnimX, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            Storyboard.SetTargetProperty(pi.AnimY, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            sb.Children.Add(pi.AnimX);
            sb.Children.Add(pi.AnimY);

            return pi;
        }

        private void BuildMultiColumn()
        {
            sb = new Storyboard();
            sb.FillBehavior = FillBehavior.HoldEnd;
            sb.AccelerationRatio = AccelerationRatio;
            sb.DecelerationRatio = DecelerationRatio;
            sb.Completed += sbMulti_Completed;
            
            items.Clear();
            panes.Clear();

            grid.ColumnDefinitions.Clear();

            for (int i = 0; i < Columns; i++)
            {
                var colDef = new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star),
                };
                grid.ColumnDefinitions.Add(colDef);

                var pi = CreatePaneItem();
                items.Add(pi);
                panes.Add(pi);

                grid.Children.Add(pi.Container);
            }

            extra = CreatePaneItem();
            grid.Children.Add(extra.Container);

            AnimateToColumns(false);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            grid = new Grid();
            this.Content = grid;

            if (this.Columns == 1)
            {
                BuildOneColumn();
            }
            else
            {
                BuildMultiColumn();
            }
        }

        #endregion

        #region Template Parts

        public FrameworkElement Top { get { return this; } }

        public ContentControl PaneA { get; private set; }
        public ContentControl PaneB { get; private set; }
        public FrameworkElement ContentA { get; private set; }
        public FrameworkElement ContentB { get; private set; }

        ///// <summary>
        ///// Gets the UI elements out of the template.
        ///// </summary>
        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();

        //    this.border = this.GetTemplateChild("PART_Border") as Border;
        //    this.topLeftContentControl = this.GetTemplateChild("PART_TopLeftContentControl") as ContentControl;
        //    this.topRightContentControl = this.GetTemplateChild("PART_TopRightContentControl") as ContentControl;
        //    this.bottomRightContentControl = this.GetTemplateChild("PART_BottomRightContentControl") as ContentControl;
        //    this.bottomLeftContentControl = this.GetTemplateChild("PART_BottomLeftContentControl") as ContentControl;

        //    if (this.topLeftContentControl != null)
        //    {
        //        this.topLeftContentControl.SizeChanged += new SizeChangedEventHandler(this.ContentControl_SizeChanged);
        //    }

        //    this.topLeftClip = this.GetTemplateChild("PART_TopLeftClip") as RectangleGeometry;
        //    this.topRightClip = this.GetTemplateChild("PART_TopRightClip") as RectangleGeometry;
        //    this.bottomRightClip = this.GetTemplateChild("PART_BottomRightClip") as RectangleGeometry;
        //    this.bottomLeftClip = this.GetTemplateChild("PART_BottomLeftClip") as RectangleGeometry;

        //    this.UpdateClipContent(this.ClipContent);

        //    this.UpdateCornerRadius(this.CornerRadius);
        //}

        #endregion

        #region A / B State

        private bool isA;

        public FrameworkElement CurrentPane
        {
            get { return isA ? PaneA : PaneB; }
        }
        public FrameworkElement OtherPane
        {
            get { return isA ? PaneB : PaneA; }
        }
        public FrameworkElement CurrentView
        {
            get { return isA ? ContentA : ContentB; }
        }
        public FrameworkElement OtherView
        {
            get { return isA ? ContentB : ContentA; }
        }

        #endregion

        #region Fields

        Storyboard sb;

        #endregion
		
        public double GetColumnX(int column)
        {
            double x = 0;
            for (int i = 0; i < column; i++)
            {
                x += grid.ColumnDefinitions[i].ActualWidth;
            }
            //l.Trace("Width for column " + column + ": " + x);
            return x;
        }
        
        public FrameworkElement BeginSwitchMulti(ExpandDirection direction = ExpandDirection.Down, int column = -1)
        {
            FrameworkElement result;

            PaneItem newPane = extra;

            //DoubleAnimation daAX, daAY;
            switch (direction)
            {
                case ExpandDirection.Left:
                    extra = panes.First();
                    panes.RemoveAt(0);
                    panes.Add(newPane);
                    newPane.TranslateTransform = new TranslateTransform(grid.ActualWidth, 0);

                    //daAX = extra.AnimX;
                    extra.AnimX.To = -GetColumnX(1);
                    extra.AnimX.From = 0;

                    extra.AnimY.To = 0;
                    extra.AnimY.From = 0;

                    //daAX =new DoubleAnimation()
                    //{
                    //    To = -GetColumnX(1),
                    //    //From = extra.TranslateTransform.X,
                    //    From = 0,
                    //};
                    //daAY = new DoubleAnimation()
                    //{
                    //    To = 0,
                    //    //From = extra.TranslateTransform.Y,
                    //    From = 0,
                    //};
                    
                    break;
                case ExpandDirection.Right:
                    extra = panes.Last();
                    panes.RemoveAt(panes.Count - 1);
                    panes.Insert(0, newPane);
                    newPane.TranslateTransform = new TranslateTransform(-grid.ColumnDefinitions[0].ActualWidth, 0);

                    extra.AnimX.To = grid.ActualWidth;
                    extra.AnimX.From = extra.TranslateTransform.X;
                    extra.AnimY.To = 0;
                    extra.AnimY.From = 0;

                    //daAX = new DoubleAnimation()
                    //{
                    //    To = grid.ActualWidth,
                    //    //From = GetColumnX(Columns - 1),
                    //    From = extra.TranslateTransform.X,
                    //};
                    //daAY = new DoubleAnimation()
                    //{
                    //    To = 0,
                    //    //From = extra.TranslateTransform.Y,
                    //    From = 0,
                    //};

                    break;
                case ExpandDirection.Down:
                    if (column < 0 || column >= items.Count) throw new ArgumentOutOfRangeException("invalid column for direction");
                    extra = panes.ElementAt(column);
                    panes.RemoveAt(column);
                    panes.Insert(column, newPane);

                    newPane.TranslateTransform = new TranslateTransform(GetColumnX(column), -grid.ActualHeight);

                    extra.AnimX.To = GetColumnX(column);
                    extra.AnimX.From = extra.TranslateTransform.X;
                    extra.AnimY.To = grid.ActualHeight;
                    extra.AnimY.From = 0;

                    //daAX = new DoubleAnimation()
                    //{
                    //    To = GetColumnX(column),
                    //    //From = extra.TranslateTransform.X,
                    //};
                    //daAY = new DoubleAnimation()
                    //{
                    //    To = grid.ActualHeight,
                    //    //From = extra.TranslateTransform.Y,
                    //    From = 0,
                    //};

                    break;
                case ExpandDirection.Up:
                    if (column < 0 || column >= items.Count) throw new ArgumentOutOfRangeException("invalid column for direction");
                    extra = panes.ElementAt(column);
                    panes.RemoveAt(column);
                    panes.Insert(column, newPane);

                    newPane.TranslateTransform = new TranslateTransform(GetColumnX(column), grid.ActualHeight);

                    extra.AnimX.To = GetColumnX(column);
                    extra.AnimX.From = extra.TranslateTransform.X;
                    extra.AnimY.To = -grid.ActualHeight;
                    extra.AnimY.From = 0;

                    //daAX = new DoubleAnimation()
                    //{
                    //    To = GetColumnX(column),
                    //    //From = extra.TranslateTransform.X,
                    //};
                    //daAY = new DoubleAnimation()
                    //{
                    //    To = -grid.ActualHeight,
                    //    //From = extra.TranslateTransform.Y,
                    //    From = 0,
                    //};

                    break;
                default:
                    throw new ArgumentOutOfRangeException("invalid direction");
            }

            l.Debug("Extra pane " + extra + " from " + extra.AnimX.From + "," + extra.AnimY.From + " to " + extra.AnimX.To + "," + extra.AnimY.To);

            //Storyboard.SetTarget(daAX, extra.Container);
            //Storyboard.SetTarget(daAY, extra.Container);

            //Storyboard.SetTargetProperty(daAX, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            //Storyboard.SetTargetProperty(daAY, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            //sb.Children.Add(daAX);
            //sb.Children.Add(daAY);

            result = newPane.Content;

            AnimateToColumns(true);

            return result;
        }

        private void AnimateToColumns(bool animate = true)
        {
            int i = 0;

            //double x = 0;

            //if (sb == null)
            {
            //    sb = new Storyboard();
            }
            sb.Duration = animate ? Duration : new Duration(TimeSpan.FromMilliseconds(0));
            

            foreach (var pane in panes
                //.Concat(new PaneItem[] { extra })
                )
            {
                var daAX = pane.AnimX;
                var daAY = pane.AnimY;

                pane.Container.Visibility = System.Windows.Visibility.Visible;

                //var from = pane.TranslateTransform.
                //var from = grid.TranslatePoint(new System.Windows.Point(0, 0), pane.Container);
                
                //daAX.IsAdditive = false;
                //daAY.IsAdditive = false;

                //daAX.From = from.X;
                daAX.From = pane.TranslateTransform.X;
                daAX.To = GetColumnX(i);

                //daAY.From = from.Y;
                daAY.From = pane.TranslateTransform.Y;
                daAY.To = 0;

                l.Debug("Pane " + pane + " from " + daAX.From + "," + daAY.From + " to " + daAX.To + ", " + daAY.To);

                //Storyboard.SetTarget(daAX, pane.Container);
                //Storyboard.SetTarget(daAY, pane.Container);

                //Storyboard.SetTargetProperty(daAX, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
                //Storyboard.SetTargetProperty(daAY, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

                //sb.Children.Add(daAX);
                //sb.Children.Add(daAY);

                //pane.Container.Width = this.grid.ActualWidth / Columns;

                i++;
            }
            if (animate)
            {
                //var daAX = new DoubleAnimation();
                //var daAY = new DoubleAnimation();

                var pane = extra;
                pane.Container.Visibility = System.Windows.Visibility.Visible;

                ////var from = pane.TranslateTransform.
                //var from = grid.TranslatePoint(new System.Windows.Point(0, 0), pane.Container);

                ////daAX.From = from.X;
                //daAX.From = pane.TranslateTransform.X;
                //daAX.To = GetColumnX(i);

                ////daAY.From = from.Y;
                //daAY.From = pane.TranslateTransform.Y;
                //daAY.To = 0;

                //Storyboard.SetTarget(daAX, pane.Container);
                //Storyboard.SetTarget(daAY, pane.Container);

                //Storyboard.SetTargetProperty(daAX, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
                //Storyboard.SetTargetProperty(daAY, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

                //sb.Children.Add(daAX);
                //sb.Children.Add(daAY);

                ////pane.Container.Width = this.grid.ActualWidth / Columns;
                //sb.FillBehavior = FillBehavior.HoldEnd;
            }
            else
            {
                //extra.Container.Visibility = System.Windows.Visibility.Collapsed;
                sbMulti_Completed(null, null);
            }

            foreach (var child in sb.Children.OfType<DoubleAnimation>())
            {
                child.Duration = Duration;
                child.EasingFunction = EasingFunction;
                child.AccelerationRatio = AccelerationRatio;
                child.DecelerationRatio = DecelerationRatio;
            }

            sb.Begin();
            
            
        }
        void sbMulti_Completed(object sender, EventArgs e)
        {
            extra.Container.Visibility = System.Windows.Visibility.Collapsed;

            int i = 0;

            foreach (var pane in panes)
            {
                pane.TranslateTransform.X = GetColumnX(i);
                pane.TranslateTransform.Y = 0;
                i++;
            }
            
        }

        public FrameworkElement BeginSwitch(ExpandDirection direction = ExpandDirection.Down, int column = -1)
        {
            if (Columns != 1)
            {
                return BeginSwitchMulti(direction, column);
            }
            isA ^= true;

            var daAX = new DoubleAnimation();
            var daAY = new DoubleAnimation();
            var daBX = new DoubleAnimation();
            var daBY = new DoubleAnimation();

            PaneA.Visibility = PaneB.Visibility = System.Windows.Visibility.Visible;

            bool inProgress = sb != null;
            sb = new Storyboard();

            switch (direction)
            {
                case ExpandDirection.Down:
                    daAX.From = 0;
                    daAX.To = 0;
                    daAY.From = -Top.ActualHeight;
                    daAY.To = 0;

                    daBX.To = 0;
                    daBX.From = 0;
                    daBY.To = Top.ActualHeight;
                    daBY.From = 0;
                    break;

                case ExpandDirection.Left:
                    daAX.From = Top.ActualWidth;
                    daAX.To = 0;
                    daAY.From = 0;
                    daAY.To = 0;

                    daBX.To = -Top.ActualWidth;
                    daBX.From = 0;
                    daBY.To = 0;
                    daBY.From = 0;
                    break;

                case ExpandDirection.Right:
                    daAX.From = -Top.ActualWidth;
                    daAX.To = 0;
                    daAY.From = 0;
                    daAY.To = 0;

                    daBX.To = Top.ActualWidth;
                    daBX.From = 0;
                    daBY.To = 0;
                    daBY.From = 0;
                    break;

                case ExpandDirection.Up:
                    daAX.From = 0;
                    daAX.To = 0;
                    daAY.From = Top.ActualHeight;
                    daAY.To = 0;

                    daBX.To = 0;
                    daBX.From = 0;
                    daBY.To = -Top.ActualHeight;
                    daBY.From = 0;
                    break;

                default:
                    break;
            }

            //daAX.Duration = daAY.Duration 
            // = daBX.Duration = daBY.Duration = this.Duration;

            //daAX.EasingFunction =
            //    daAY.EasingFunction =
            //    daBX.EasingFunction =
            //    daBY.EasingFunction = this.EasingFunction;

            sb.AccelerationRatio = AccelerationRatio;
            sb.DecelerationRatio = DecelerationRatio;
            sb.Duration = Duration;

            Storyboard.SetTarget(daAX, CurrentPane);
            Storyboard.SetTarget(daAY, CurrentPane);
            Storyboard.SetTarget(daBX, OtherPane);
            Storyboard.SetTarget(daBY, OtherPane);

            if (UseGroup)
            {
                Storyboard.SetTargetProperty(daAX, new PropertyPath("(UIElement.RenderTransform)[0].(TranslateTransform.X)"));
                Storyboard.SetTargetProperty(daAY, new PropertyPath("(UIElement.RenderTransform)[0].(TranslateTransform.Y)"));
                Storyboard.SetTargetProperty(daBX, new PropertyPath("(UIElement.RenderTransform)[0].(TranslateTransform.X)"));
                Storyboard.SetTargetProperty(daBY, new PropertyPath("(UIElement.RenderTransform)[0].(TranslateTransform.Y)"));
            }
            else
            {
                Storyboard.SetTargetProperty(daAX, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
                Storyboard.SetTargetProperty(daAY, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
                Storyboard.SetTargetProperty(daBX, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
                Storyboard.SetTargetProperty(daBY, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            }

            //Storyboard.SetTargetProperty(daAX, new PropertyPath(TranslateTransform.X)"));
            //Storyboard.SetTargetProperty(daAX, new PropertyPath(TranslateTransform.XProperty));
            //Storyboard.SetTargetProperty(daAX, new PropertyPath("X"));
            //Storyboard.SetTargetProperty(daAX, new PropertyPath("(RenderTransform.X)"));

            //Storyboard.SetTargetProperty(daAX, new PropertyPath("(RenderTransform.Children[0].X)"));

            sb.Children.Add(daAX);
            sb.Children.Add(daAY);
            sb.Children.Add(daBX);
            sb.Children.Add(daBY);

            //ATranslate.BeginAnimation(TranslateTransform.XProperty, daAX);
            //ATranslate.BeginAnimation(TranslateTransform.YProperty, daAY);

            sb.Completed += sb_Completed;
            sb.Begin();

            return CurrentView;
        }

        void sb_Completed(object sender, EventArgs e)
        {
            sb = null;
            OtherPane.Visibility = Visibility.Hidden;
        }


        #region Misc

        private static readonly ILogger l = Log.Get();

        #endregion

    }

}
