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
using System.Collections;
using LionFire.Collections;
using System.Windows.Markup;
using LionFire.Extensions.Linq;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace LionFire.Avalon
{
    /// <summary>
    /// Interaction logic for LionZapScroller.xaml
    /// </summary>
    [ContentProperty("ItemsSource")]
    public partial class LionZapScroller : UserControl, INotifyPropertyChanged
    {
        private static readonly ILogger l = Log.Get();

        public LionZapScroller()
        {
            InitializeComponent();

            //ItemsSource = new MultiBindableCollection<UIElement>();

            OnSelectorDockChanged();
            LionListBox.SelectedIndex = -1;

            HeaderTemplate = HeaderTemplate; // Initializes Header

            this.SizeChanged += new SizeChangedEventHandler(LionZapScroller_SizeChanged);
            //this.Loaded += new RoutedEventHandler(LionZapScroller_Loaded);
            ZapScroller.ZapDecoratorChanged += new Action(ZapScroller_m_zapDecoratorChanged);

            this.Loaded += LionZapScroller_Loaded;
        }

        #region Event handling

        void LionZapScroller_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateListBoxSelectedItem();
        }

        void ZapScroller_m_zapDecoratorChanged()
        {

            if (ZapScroller.ZapDecorator != null)
            {
                ZapScroller.ZapDecorator.ZapPanelChanged += new Action(ZapDecorator_ZapPanelChanged);
                ZapScroller.ZapDecoratorChanged -= new Action(ZapScroller_m_zapDecoratorChanged);

                ZapScroller.ZapDecorator.Tag = this.Name;
            }
#if SanityChecks
            //else
            //{
            //    l.Warn("LionZapScroller.ZapScroller.ZapDecorator changed to null");
            //}
#endif
        }

        void ZapDecorator_ZapPanelChanged()
        {
            if (ZapScroller.ZapDecorator.ZapPanel != null)
            {
                ZapScroller.ZapDecorator.ZapPanel.LionZapScroller = this;
                ZapScroller.ZapDecorator.ZapPanelChanged -= new Action(ZapDecorator_ZapPanelChanged);

                ZapScroller.ZapDecorator.ZapPanel.Tag = this.Name;
            }
#if SanityChecks
            //else
            //{
            //    l.Warn("LionZapScroller.ZapScroller.ZapDecorator.ZapPanel changed to null");
            //}
#endif
        }

        void LionZapScroller_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //l.Fatal((this.Name ?? this.ToString()) + " size: " + e.NewSize);
        }


        //bool firstLoad = true;
        //void LionZapScroller_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (firstLoad)
        //    {
        //        firstLoad = false;
        //        LionListBox.SelectedIndex = 0;
        //    }
        //}

        private void LionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int selectedIndex = this.LionListBox.SelectedIndex;
                if (selectedIndex == -1) return;

                var command = this.ZapScroller.GetCommand(selectedIndex);
                if (command != null && command.CanExecute)
                {
                    command.MakeCurrent();
                }
            }
            catch (Exception ex)
            {
                l.Error(ex.ToString());
            }
        }

        #endregion

        #region ContentPadding

        /// <summary>
        /// ContentPadding Dependency Property
        /// </summary>
        public static readonly DependencyProperty ContentPaddingProperty =
            DependencyProperty.Register("ContentPadding", typeof(Thickness), typeof(LionZapScroller),
                new FrameworkPropertyMetadata(new Thickness(8)));

        /// <summary>
        /// Gets or sets the ContentPadding property. This dependency property 
        /// indicates ....
        /// </summary>
        public Thickness ContentPadding
        {
            get { return (Thickness)GetValue(ContentPaddingProperty); }
            set { SetValue(ContentPaddingProperty, value); }
        }

        #endregion

        #region SelectorDock

        /// <summary>
        /// SelectorDock Dependency Property
        /// </summary>
        public static readonly DependencyProperty SelectorDockProperty =
            DependencyProperty.Register("SelectorDock", typeof(Dock), typeof(LionZapScroller),
                new FrameworkPropertyMetadata((Dock)Dock.Top,
                    FrameworkPropertyMetadataOptions.AffectsMeasure
                    , _OnSelectorDockChanged
                        ));

        /// <summary>
        /// Gets or sets the SelectorDock property. This dependency property 
        /// indicates the position of the selector.
        /// </summary>
        public Dock SelectorDock
        {
            get { return (Dock)GetValue(SelectorDockProperty); }
            set { SetValue(SelectorDockProperty, value); }
        }

        private static void _OnSelectorDockChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            ((LionZapScroller)depObj).OnSelectorDockChanged();
        }

        private void OnSelectorDockChanged()
        {
            this.ZapScroller.Orientation = this.LionListBox.Orientation = (SelectorDock == Dock.Bottom || SelectorDock == Dock.Top) ? Orientation.Horizontal : Orientation.Vertical;

        }

        #endregion

        #region HeaderTemplate

        public DataTemplate HeaderTemplate
        {
            get
            {
                return LionListBox.ItemTemplate;
            }
            set
            {
                if (value != null)
                {
                    LionListBox.DisplayMemberPath = null;
                }
                LionListBox.ItemTemplate = value;
                UpdateListBoxDisplayMemberPath();
                if (value == null)
                {
                    LionListBox.DisplayMemberPath = HeaderDisplayMemberPath;
                }
            }
        }

        private void UpdateListBoxDisplayMemberPath()
        {
            if (LionListBox.ItemTemplate == null)
            {
                LionListBox.DisplayMemberPath = HeaderDisplayMemberPath;
            }
        }

        #endregion

        #region HeaderDisplayMemberPath

        /// <summary>
        /// HeaderDisplayMemberPath Dependency Property
        /// </summary>
        public static readonly DependencyProperty HeaderDisplayMemberPathProperty =
            DependencyProperty.Register("HeaderDisplayMemberPath", typeof(string), typeof(LionZapScroller),
                new FrameworkPropertyMetadata((string)"Header",
                    FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the HeaderDisplayMemberPath property. This dependency property 
        /// indicates the path to the header content for each item.
        /// </summary>
        public string HeaderDisplayMemberPath
        {
            get { return (string)GetValue(HeaderDisplayMemberPathProperty); }
            set { SetValue(HeaderDisplayMemberPathProperty, value); }
        }

        #endregion
        
        #region ItemTemplate

        public DataTemplate ItemTemplate
        {
            get
            {
                return ZapScroller.ItemTemplate;
            }
            set
            {
                //if (value != null)
                //{
                //    LionListBox.DisplayMemberPath = null;
                //}
                ZapScroller.ItemTemplate = value;
                //if (value == null)
                //{
                //    LionListBox.DisplayMemberPath = DefaultDisplayMemberPath;
                //}
            }
        }

        #endregion

        //#region ItemsSource

        ///// <summary>
        ///// ItemsSource Dependency Property
        ///// </summary>
        //public static readonly DependencyProperty ItemsSourceProperty =
        //    DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(LionZapScroller),
        //        new FrameworkPropertyMetadata((IEnumerable)null));

        ///// <summary>
        ///// Gets or sets the ItemsSource property. This dependency property 
        ///// indicates ....
        ///// </summary>
        //public IEnumerable ItemsSource
        //{
        //    get { return (IEnumerable)GetValue(ItemsSourceProperty); }
        //    set { SetValue(ItemsSourceProperty, value); }
        //}

        //#endregion

        #region ItemsSource

        /// <summary>
        /// ItemsSource Dependency Property
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(LionZapScroller),
                new FrameworkPropertyMetadata((IEnumerable)null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange,
                    new PropertyChangedCallback(OnItemsSourceChanged)));

        /// <summary>
        /// Gets or sets the ItemsSource property. This dependency property 
        /// indicates ....
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ItemsSource property.
        /// </summary>
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LionZapScroller target = (LionZapScroller)d;
            IEnumerable oldItemsSource = (IEnumerable)e.OldValue;
            IEnumerable newItemsSource = target.ItemsSource;
            target.OnItemsSourceChanged(oldItemsSource, newItemsSource);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ItemsSource property.
        /// </summary>
        protected virtual void OnItemsSourceChanged(IEnumerable oldItemsSource, IEnumerable newItemsSource)
        {
            {
                INotifyingCollection<UIElement> nc = oldItemsSource as INotifyingCollection<UIElement>;
                if (nc != null)
                {
                    nc.CollectionChanged -= new NotifyCollectionChangedHandler<UIElement>(OnItemsCollectionChanged);
                }
            }
            {
                INotifyingCollection<UIElement> nc = newItemsSource as INotifyingCollection<UIElement>;
                if (nc != null)
                {
                    nc.CollectionChanged += new NotifyCollectionChangedHandler<UIElement>(OnItemsCollectionChanged);
                }
                else if(nc != null && !nc.GetType().IsArray)
                {
                    l.Warn("LionZapScroller: No notification support for: " + newItemsSource.GetType().FullName);
                }
            }

            //l.Trace(this.Name + " new ItemsSource: " + oldItemsSource + " => " + newItemsSource);

            OnPropertyChanged("ItemsSource"); // Populates ZapScroller

            OnItemsCollectionChanged(new NotifyCollectionChangedEventArgs<UIElement>(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }

        void OnItemsCollectionChanged(NotifyCollectionChangedEventArgs<UIElement> e)
        {
            // REVIEW - would this help?
            //OnPropertyChanged("ItemsSource"); // Do this first to inform ZapScroller

            UpdateListBoxVisibility();
        }

        #endregion

        private void UpdateListBoxVisibility()
        {
            LionListBox.Visibility = ItemsSource.HasAtLeast(2) ? Visibility.Visible : System.Windows.Visibility.Collapsed;
            if (LionListBox.Visibility == System.Windows.Visibility.Visible)
            {
                UpdateListBoxSelectedItem();
            }
        }

        //#region Children

        ///// <summary>
        ///// Children Dependency Property
        ///// </summary>
        //public static readonly DependencyProperty ChildrenProperty =
        //    DependencyProperty.Register("Children", typeof(UIElementCollection), typeof(LionZapScroller),
        //        new FrameworkPropertyMetadata((UIElementCollection)null,
        //            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange,
        //            new PropertyChangedCallback(OnChildrenChanged)));

        ///// <summary>
        ///// Gets or sets the Children property. This dependency property 
        ///// indicates the items in the scroller.
        ///// </summary>
        //public UIElementCollection Children
        //{
        //    get { return (UIElementCollection)GetValue(ChildrenProperty); }
        //    set { SetValue(ChildrenProperty, value); }
        //}

        ///// <summary>
        ///// Handles changes to the Children property.
        ///// </summary>
        //private static void OnChildrenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    LionZapScroller target = (LionZapScroller)d;
        //    UIElementCollection oldChildren = (UIElementCollection)e.OldValue;
        //    UIElementCollection newChildren = target.Children;
        //    target.OnChildrenChanged(oldChildren, newChildren);
        //}

        ///// <summary>
        ///// Provides derived classes an opportunity to handle changes to the Children property.
        ///// </summary>
        //protected virtual void OnChildrenChanged(UIElementCollection oldChildren, UIElementCollection newChildren)
        //{
        //    var listener = DependencyPropertyExtensions.FromDependencyPropertyChanged(this.ItemsSource,
        //           collection => collection.Count).
        //           Subscribe(x => System.Diagnostics.Debug.WriteLine("Count changed"));

        //}

        //#endregion

        #region SelectorBackgroundTemplate

        /// <summary>
        /// SelectorBackgroundTemplate Dependency Property
        /// TODO - this should be a ControlTemplate, not a FrameworkElement
        /// </summary>
        public static readonly DependencyProperty SelectorBackgroundTemplateProperty =
            DependencyProperty.Register("SelectorBackgroundTemplate", typeof(ControlTemplate), typeof(LionZapScroller),
                new FrameworkPropertyMetadata((ControlTemplate)null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the SelectorBackgroundTemplate property. This dependency property 
        /// indicates ....
        /// </summary>
        public ControlTemplate SelectorBackgroundTemplate
        {
            get { return (ControlTemplate)GetValue(SelectorBackgroundTemplateProperty); }
            set { SetValue(SelectorBackgroundTemplateProperty, value); }
        }

        #endregion

        public bool AlwaysHandleEvents = true;

        #region MouseWheelScroll

        /// <summary>
        /// MouseWheelScroll Dependency Property
        /// </summary>
        public static readonly DependencyProperty MouseWheelScrollProperty =
            DependencyProperty.Register("MouseWheelScroll", typeof(bool), typeof(LionZapScroller),
                new FrameworkPropertyMetadata((bool)true));

        /// <summary>
        /// Gets or sets the MouseWheelScroll property. This dependency property 
        /// indicates whether mouse wheel can select tabs.
        /// </summary>
        public bool MouseWheelScroll
        {
            get { return (bool)GetValue(MouseWheelScrollProperty); }
            set { SetValue(MouseWheelScrollProperty, value); }
        }

        #endregion



        private void ContentControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (MouseWheelScroll)
            {
                try
                {
                    int selectedIndex = this.LionListBox.SelectedIndex;
                    if (selectedIndex == -1)
                    {
                        selectedIndex = 0;
                    }
                    else
                    {
                        selectedIndex = (e.Delta < 0) ? selectedIndex + 1 : selectedIndex - 1;
                    }

                    var command = this.ZapScroller.GetCommand(selectedIndex);
                    if (command != null && command.CanExecute)
                    {
                        this.LionListBox.SelectedIndex = selectedIndex;
                        //command.MakeCurrent();
                        e.Handled = true;
                    }
                    else
                    {
                        l.Fatal("ContentControl_MouseWheel: No current command or !CanExecute");
                    }
                    if (AlwaysHandleEvents) e.Handled = true;
                }
                catch (Exception ex)
                {
                    l.Error(ex.ToString());
                }
            }
        }

        private void ZapScroller_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // TODO: Optional scrolling
            ContentControl_MouseWheel(sender, e);
        }



        public void SelectItem(int selectedIndex)
        {
            var command = this.ZapScroller.GetCommand(selectedIndex);
            if (command != null
                //&& command.CanExecute // This is false if already set to the specified index
                )
            {
                this.LionListBox.SelectedIndex = selectedIndex;
            }
            //else
            //{
            //    l.Fatal("SelectItem: No current command or !CanExecute");
            //}
            //this.LionListBox.InvalidateHighlight(); - unneeded?
        }

        public int SelectedIndex
        {
            get
            {
                
                return LionListBox.SelectedIndex;
            }
            set
            {
                if (value == LionListBox.SelectedIndex) return;

                if (value >= LionListBox.Items.Count) value = LionListBox.Items.Count - 1;
                //if (value  < 0) value = 0;

                LionListBox.SelectedIndex = value;
            }
        }

        private void UpdateListBoxSelectedItem()
        {
            if (LionListBox.SelectedIndex < 0)
            {
                SelectItem(0);
            }
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var ev = PropertyChanged;
            if (ev != null) ev(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
