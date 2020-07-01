//#if TOPORT
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using PixelLab.Common;
using System;
using Microsoft.Extensions.Logging;
using DelegateCommand = LionFire.Avalon.LionDelegateCommand;

namespace LionFire.Avalon
{
    public class ZapTabControl : TabControl, IZapScroller 
    {
        static ZapTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZapTabControl), new FrameworkPropertyMetadata(typeof(ZapTabControl)));
            FocusableProperty.OverrideMetadata( typeof(ZapTabControl), new FrameworkPropertyMetadata(false));
        }

        public ZapTabControl()
        {
            m_firstCommand = new DelegateCommand(First, canFirst);
            m_previousCommand = new DelegateCommand(Previous, canPrevious);
            m_nextCommand = new DelegateCommand(Next, canNext);
            m_lastCommand = new DelegateCommand(Last, canLast);

            this.SizeChanged += new SizeChangedEventHandler(ZapTapControl_SizeChanged);
        }


        LionListBox llb;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

             llb = this.GetTemplateChild("llb") as LionListBox;
            if (llb != null)
            {
                llb.SelectionChanged += llb_SelectionChanged;
            }
        }

        void llb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var c = llb.SelectedItem as ICommand;
            c.Execute(null);
        }

        void ZapTapControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(ZapDecorator != null)
            this.ZapDecorator.InvalidateMeasure();
        }

        public ICommand FirstCommand { get { return m_firstCommand; } }

        public ICommand PreviousCommand { get { return m_previousCommand; } }

        public ICommand NextCommand { get { return m_nextCommand; } }

        public ICommand LastCommand { get { return m_lastCommand; } }

        public ReadOnlyObservableCollection<ZapCommandItem> Commands
        {
            get
            {
                return m_commandItems.ReadOnly;
            }
        }

        public static readonly DependencyProperty CommandItemTemplateProperty =
            DependencyProperty.Register("CommandItemTemplate", typeof(DataTemplate), typeof(ZapTabControl));

        public DataTemplate CommandItemTemplate
        {
            get { return (DataTemplate)GetValue(CommandItemTemplateProperty); }
            set { SetValue(CommandItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty CommandItemTemplateSelectorProperty =
            DependencyProperty.Register("CommandItemTemplateSelector", typeof(DataTemplateSelector), typeof(ZapTabControl));

        public DataTemplateSelector CommandItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(CommandItemTemplateSelectorProperty); }
            set { SetValue(CommandItemTemplateSelectorProperty, value); }
        }

        private static readonly DependencyPropertyKey ItemCountPropertyKey =
            DependencyProperty.RegisterReadOnly("ItemCount",
            typeof(int), typeof(ZapTabControl), new PropertyMetadata(0));

        public static readonly DependencyProperty ItemCountProperty = ItemCountPropertyKey.DependencyProperty;

        public int ItemCount
        {
            get { return (int)GetValue(ItemCountProperty); }
        }

        public static readonly DependencyProperty CurrentItemIndexProperty =
            DependencyProperty.Register("CurrentItemIndex", typeof(int), typeof(ZapTabControl),
            new PropertyMetadata(new PropertyChangedCallback(currentItemIndex_changed)));

        public int CurrentItemIndex
        {
            get { return (int)GetValue(CurrentItemIndexProperty); }
            set { SetValue(CurrentItemIndexProperty, value); }
        }

        public void First()
        {
            if (canFirst())
            {
                CurrentItemIndex = 0;
            }
        }

        public void Previous()
        {
            if (canPrevious())
            {
                CurrentItemIndex--;
            }
        }

        public void Next()
        {
            if (canNext())
            {
                CurrentItemIndex++;
            }
        }

        public void Last()
        {
            if (canLast())
            {
                CurrentItemIndex = (ItemCount - 1);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            findZapDecorator();

            return base.MeasureOverride(availableSize);
        }

        public static readonly RoutedEvent CurrentItemIndexChangedEvent =
             EventManager.RegisterRoutedEvent("CurrentItemIndexChanged", RoutingStrategy.Bubble,
             typeof(RoutedPropertyChangedEventHandler<int>), typeof(ZapTabControl));

        public event RoutedPropertyChangedEventHandler<int> CurrentItemChanged
        {
            add { base.AddHandler(CurrentItemIndexChangedEvent, value); }
            remove { base.RemoveHandler(CurrentItemIndexChangedEvent, value); }
        }

        protected virtual void OnCurrentItemIndexChanged(int oldValue, int newValue)
        {
            resetEdgeCommands();
            RoutedPropertyChangedEventArgs<int> args = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue);
            args.RoutedEvent = CurrentItemIndexChangedEvent;
            base.RaiseEvent(args);
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            ItemCollection newItems = this.Items;

            if (newItems != m_internalItemCollection)
            {
                m_internalItemCollection = newItems;

                resetProperties();
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (m_internalItemCollection != Items)
            {
                m_internalItemCollection = Items;
            }

            resetProperties();
        }

        #region Implementation

        private static void currentItemIndex_changed(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            ZapTabControl zapScroller = (ZapTabControl)element;
            zapScroller.OnCurrentItemIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        private void resetEdgeCommands()
        {
            m_firstCommand.RaiseCanExecuteChanged();
            m_lastCommand.RaiseCanExecuteChanged();
            m_nextCommand.RaiseCanExecuteChanged();
            m_previousCommand.RaiseCanExecuteChanged();
        }

        private void resetCommands()
        {
            resetEdgeCommands();

            int parentItemsCount = this.ItemCount;

            if (parentItemsCount != m_commandItems.Count)
            {
                if (parentItemsCount > m_commandItems.Count)
                {
                    for (int i = m_commandItems.Count; i < parentItemsCount; i++)
                    {
                        m_commandItems.Add(new ZapCommandItem(this, i));
                    }
                }
                else
                {
                    Debug.Assert(parentItemsCount < m_commandItems.Count);
                    int delta = m_commandItems.Count - parentItemsCount;
                    for (int i = 0; i < delta; i++)
                    {
                        m_commandItems.RemoveAt(m_commandItems.Count - 1);
                    }
                }
            }

            Debug.Assert(Items.Count == m_commandItems.Count);

            for (int i = 0; i < parentItemsCount; i++)
            {
                m_commandItems[i].Content = Items[i];
            }

#if DEBUG
            for (int i = 0; i < m_commandItems.Count; i++)
            {
                Debug.Assert(((ZapCommandItem)m_commandItems[i]).Index == i);
            }
#endif
        }

        private void findZapDecorator()
        {
            if (this.Template != null)
            {
                ZapDecorator temp = this.Template.FindName(PART_ZapDecorator, this) as ZapDecorator;
                if (ZapDecorator != temp)
                {
                    ZapDecorator = temp;
                    if (ZapDecorator != null)
                    {
                        {
                            Binding binding = new Binding("CurrentItemIndex");
                            binding.Source = this;
                            ZapDecorator.SetBinding(ZapDecorator.TargetIndexProperty, binding);
                        }

                        {
                            Binding binding = new Binding("Orientation");
                            binding.Source = this;
                            ZapDecorator.SetBinding(ZapDecorator.OrientationProperty, binding);
                        }

                    }
                }
                else
                {
                    Debug.WriteLine("No element with name '" + PART_ZapDecorator + "' in the template.");
                }
            }
            else
            {
                Debug.WriteLine("No template defined for ZapTapControl.");
            }
        }

        private void resetProperties()
        {
            if (m_internalItemCollection.Count != ItemCount)
            {
                SetValue(ItemCountPropertyKey, m_internalItemCollection.Count);
            }
            if (CurrentItemIndex >= ItemCount)
            {
                CurrentItemIndex = (ItemCount - 1);
            }
            else if (CurrentItemIndex == -1 && ItemCount > 0)
            {
                CurrentItemIndex = 0;
            }

            resetCommands();

            InvalidateMeasure();
        }

        private bool canFirst()
        {
            return (ItemCount > 1) && (CurrentItemIndex > 0);
        }

        private bool canNext()
        {
            return (CurrentItemIndex >= 0) && CurrentItemIndex < (ItemCount - 1);
        }

        private bool canPrevious()
        {
            return CurrentItemIndex > 0;
        }

        private bool canLast()
        {
            return (ItemCount > 1) && (CurrentItemIndex < (ItemCount - 1));
        }

        private ItemCollection m_internalItemCollection;

        #region ZapDecorator

        private static readonly ILogger l = Log.Get();
		
        public ZapDecorator ZapDecorator
        {
            get { return zapDecorator; }
            set
            {
                if (zapDecorator == value) return;
                if (zapDecorator != null) l.Warn("non-null zapDecorator being changed"); // TEMP
                zapDecorator = value;
                
                var ev = ZapDecoratorChanged;
                if (ev != null) ev();
            }
        } private ZapDecorator zapDecorator;

        public event Action ZapDecoratorChanged;

        #endregion
        
        private readonly LionDelegateCommand m_firstCommand, m_previousCommand, m_nextCommand, m_lastCommand;

        private readonly ObservableCollectionPlus<ZapCommandItem> m_commandItems = new ObservableCollectionPlus<ZapCommandItem>();

        public ZapCommandItem GetCommand(int index)
        {
            foreach (var command in m_commandItems)
            {
                if (command.Index == index)
                {
                    return command;
                }
            }
            return null;
        }
        #endregion

        public const string PART_ZapDecorator = "PART_ZapDecorator";


        #region Orientation

        /// <summary>
        /// Orientation Dependency Property
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ZapTabControl),
                new FrameworkPropertyMetadata((Orientation)Orientation.Horizontal,
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


        #region HeaderDock

        /// <summary>
        /// HeaderDock Dependency Property
        /// </summary>
        public static readonly DependencyProperty HeaderDockProperty =
            DependencyProperty.Register("HeaderDock", typeof(Dock), typeof(ZapTabControl),
                new FrameworkPropertyMetadata((Dock)Dock.Top,
                    FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Gets or sets the HeaderDock property. This dependency property 
        /// indicates the position of the header.
        /// </summary>
        public Dock HeaderDock
        {
            get { return (Dock)GetValue(HeaderDockProperty); }
            set { SetValue(HeaderDockProperty, value); }
        }

        #endregion


        #region ListBoxStyle

        /// <summary>
        /// ListBoxStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty ListBoxStyleProperty =
            DependencyProperty.Register("ListBoxStyle", typeof(Style), typeof(ZapTabControl),
                new FrameworkPropertyMetadata((Style)null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    new PropertyChangedCallback(OnListBoxStyleChanged)));

        /// <summary>
        /// Gets or sets the ListBoxStyle property. This dependency property 
        /// indicates ....
        /// </summary>
        public Style ListBoxStyle
        {
            get { return (Style)GetValue(ListBoxStyleProperty); }
            set { SetValue(ListBoxStyleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ListBoxStyle property.
        /// </summary>
        private static void OnListBoxStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZapTabControl target = (ZapTabControl)d;
            Style oldListBoxStyle = (Style)e.OldValue;
            Style newListBoxStyle = target.ListBoxStyle;
            target.OnListBoxStyleChanged(oldListBoxStyle, newListBoxStyle);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ListBoxStyle property.
        /// </summary>
        protected virtual void OnListBoxStyleChanged(Style oldListBoxStyle, Style newListBoxStyle)
        {
        }

        #endregion

    }
}
//#endif