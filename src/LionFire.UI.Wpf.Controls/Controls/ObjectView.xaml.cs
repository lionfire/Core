using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.WpfPropertyGrid;

namespace LionFire.Avalon
{
    /// <summary>
    /// Interaction logic for LocalProcessMonitorPanel.xaml
    /// </summary>
    public partial class ObjectView : UserControl
    {
        public static Dictionary<Type, Type> ViewTypes = new Dictionary<Type, Type>();

        public Dictionary<Type, FrameworkElement> loadedViews = new Dictionary<Type, FrameworkElement>();
        
        public static Type DefaultViewType = typeof(PropertyGrid);

        public ObjectView()
        {
            InitializeComponent();

            this.DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);
            UpdateView();
        }

        private void UpdateView()
        {
            if (DataContext == null)
            {
                //ContentPresenter.Visibility = System.Windows.Visibility.Hidden;
                //ContentPresenter.Content = null;
                return;
            }
            ContentPresenter.Visibility = System.Windows.Visibility.Visible;


            Type dataType = DataContext.GetType();
            Type viewType = ViewTypes.TryGetValue(dataType) ?? DefaultViewType;
            FrameworkElement fe = loadedViews.TryGetValue(viewType);
            if (fe == null)
            {
                fe = (FrameworkElement)Activator.CreateInstance(viewType);

                InitPropertyGrid(fe);
                loadedViews.Add(viewType, fe);
            }

            fe.DataContext = this.DataContext;
            ContentPresenter.Content = fe;
        }

        private static void InitPropertyGrid(FrameworkElement fe)
        {
            PropertyGrid pg = fe as PropertyGrid;
            if (pg != null)
            {
                pg.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto); // FAIL
                //pg.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);

                //pg.PropertyDisplayMode = PropertyDisplayMode.
                pg.PropertyFilterVisibility = Visibility.Collapsed;
            }
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext == null)
            {
                ContentPresenter.Content = null;
                return;
            }

            Type dataType = DataContext.GetType();
            Type viewType = ViewTypes.TryGetValue(dataType) ?? DefaultViewType;
            FrameworkElement fe = loadedViews.TryGetValue(viewType);

            if (fe == null)
            {
                fe = (FrameworkElement) Activator.CreateInstance(viewType);
                InitPropertyGrid(fe);
                loadedViews.Add(viewType, fe);
            }

            if (fe as PropertyGrid != null)
            {
                PropertyGrid wpg = (PropertyGrid)fe;
                wpg.SelectedObject = this.DataContext;
            }
            else
            {
                fe.DataContext = this.DataContext;
            }


            ContentPresenter.Content = fe;
        }


    }
}
