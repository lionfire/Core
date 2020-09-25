#if !NOESIS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
#if NOESIS
using Noesis;
#else
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
#endif
using System.Windows.Input;
//using LionFire.Processes;
//using LionFire.Services;
using LionFire.Collections;
using System.Collections;
using System.Collections.Specialized;

namespace LionFire.Avalon
{
    /// <summary>
    /// Interaction logic for LocalProcessMonitorPanel.xaml
    /// </summary>
    public partial class CollectionPropertiesView : UserControl
    {
        public Func<object, object> GetDetailObject = null;
        public Func<object, object> GetDisplayName = null;

#region DetailObject

        /// <summary>
        /// DetailObject Dependency Property
        /// </summary>
        public static readonly DependencyProperty DetailObjectProperty =
            DependencyProperty.Register("DetailObject", typeof(object), typeof(CollectionPropertiesView),
                new FrameworkPropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the DetailObject property. This dependency property 
        /// indicates ....
        /// </summary>
        public object DetailObject
        {
            get { return (object)GetValue(DetailObjectProperty); }
            set { SetValue(DetailObjectProperty, value); }
        }

#endregion

#region ListLocation

        /// <summary>
        /// ListLocation Dependency Property
        /// </summary>
        public static readonly DependencyProperty ListLocationProperty =
            DependencyProperty.Register("ListLocation", typeof(Dock), typeof(CollectionPropertiesView),
                new FrameworkPropertyMetadata((Dock)Dock.Left));

        /// <summary>
        /// Gets or sets the ListLocation property. This dependency property 
        /// indicates ....
        /// </summary>
        public Dock ListLocation
        {
            get { return (Dock)GetValue(ListLocationProperty); }
            set { SetValue(ListLocationProperty, value); }
        }

#endregion

        public CollectionPropertiesView()
        {
            InitializeComponent();

            itemsColumnWidth = ItemsColumn.Width;
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(CollectionPropertiesView_DataContextChanged);
            ListView.SelectionChanged += new SelectionChangedEventHandler(ListView_SelectionChanged);
        }

        void CollectionPropertiesView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.ListView.ItemsSource = this.DataContext as IEnumerable;

            INotifyCollectionChanged ncc = this.ListView.ItemsSource as INotifyCollectionChanged;
            if(ncc != null)
            {
                ncc.CollectionChanged += new NotifyCollectionChangedEventHandler(ncc_CollectionChanged);
            }
            OnItemsChanged();

        }

        private void OnItemsChanged()
        {
            ItemsColumn.Width = this.ListView.ItemsSource.OfType<object>().Count() > 1 ? itemsColumnWidth : new GridLength(0);
            if (ListView.SelectedItem == null || !this.ListView.ItemsSource.OfType<object>().Contains(ListView.SelectedItem))
            {
                ListView.SelectedItem = this.ListView.ItemsSource.OfType<object>().FirstOrDefault();
            }
        }

        GridLength itemsColumnWidth;
        void ncc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
            OnItemsChanged();
        }

        void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this.PropertyGrid.SelectedObject
            if (GetDetailObject != null)
            {                
                DetailObject = GetDetailObject(ListView.SelectedItem);
            }
            else
            {
                DetailObject = ListView.SelectedItem;
            }

            this.ObjectView.DataContext = DetailObject;
        }


        //#region ListItemDisplayConverter

        ///// <summary>
        ///// ListItemDisplayConverter Dependency Property
        ///// </summary>
        //public static readonly DependencyProperty ListItemDisplayConverterProperty =
        //    DependencyProperty.Register("ListItemDisplayConverter", typeof(IValueConverter), typeof(CollectionPropertiesView),
        //        new FrameworkPropertyMetadata((IValueConverter)null));

        ///// <summary>
        ///// Gets or sets the ListItemDisplayConverter property. This dependency property 
        ///// indicates ....
        ///// </summary>
        //public IValueConverter ListItemDisplayConverter
        //{
        //    get { return (IValueConverter)GetValue(ListItemDisplayConverterProperty); }
        //    set { SetValue(ListItemDisplayConverterProperty, value); }
        //}

        //#endregion


    }
}
#endif