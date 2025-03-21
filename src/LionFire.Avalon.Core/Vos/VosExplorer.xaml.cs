using Blacklight.Controls;
using LionFire.Applications;
using LionFire.Assemblies;
using LionFire.ObjectBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LionFire.Avalon
{
    public class VobListViewItem : ListViewItem
    {
    }


}
namespace LionFire.Avalon.Vos
{

    /// <summary>
    /// Interaction logic for VosExplorer.xaml
    /// </summary>
    [MetaAttribute("LaunchName", "Vos")]
    public partial class VosExplorer : UserControl, ILauncher, INotifyPropertyChanged
    {
        #region Launcher

        void ILauncher.Launch(string[] args)
        {
            VosApp.InitVosApp();
            LionFire.Shell.WpfLionFireShell.Run<VosExplorer>();
        }

        #endregion

        #region Construction and Init

        DataTemplate lviTemplate;
        public VosExplorer()
        {
            InitializeComponent();

            this.KeyDown += VosExplorer_KeyDown;

            Con1.ContentFactory = () =>
                {
                    var listView = new VobListView();
                    return listView;
                };

            InitPanes();

            lviTemplate = (DataTemplate)FindResource("VobListViewItem");

            foreach (var view in ListViews)
            {
                view.SelectionMode = SelectionMode.Single;
                view.SelectedItem = null;
                view.SelectionChanged += view_SelectionChanged;
                view.GotFocus += view_GotFocus;
            }

            this.Loaded += VosExplorer_Loaded;

            BreadcrumbBar.PathChanged += BreadcrumbBar_PathChanged;
            BreadcrumbBar.PathConversion += BreadcrumbBar_PathConversion;
            BreadcrumbBar.PopulateItems += BreadcrumbBar_PopulateItems;
            BreadcrumbBar.PopulateItems += BreadcrumbBar_PopulateItems;
            BreadcrumbBar.SelectedBreadcrumbChanged += BreadcrumbBar_SelectedBreadcrumbChanged;
        }

        void BreadcrumbBar_SelectedBreadcrumbChanged(object sender, RoutedPropertyChangedEventArgs<Odyssey.Controls.BreadcrumbItem> e)
        {
            l.Warn("TODO: BreadcrumbBar_SelectedBreadcrumbChanged " + e.NewValue);
        }

        void BreadcrumbBar_PopulateItems(object sender, Odyssey.Controls.BreadcrumbItemEventArgs e)
        {
            //l.Trace("TODO: BreadcrumbBar_PopulateItems " + e.Item.Tag);

            var item = e.Item;

            if (item.Items.Count == 0 && item.ItemsSource == null)
            {
                Vob vob = e.Item.DataContext as Vob;
                item.ItemsSource = vob.GetChildren();
                e.Handled = true;
            }
        }

        void BreadcrumbBar_PathConversion(object sender, Odyssey.Controls.PathConversionEventArgs e)
        {
            //l.Trace("TODO: BreadcrumbBar_PathConversion " + e.EditPath + " " + e.DisplayPath);

        }

        void BreadcrumbBar_PathChanged(object sender, RoutedPropertyChangedEventArgs<string> e)
        {
            //l.Trace("TODO: BreadcrumbBar_PathChanged " + e.NewValue);
        }

        void view_GotFocus(object sender, RoutedEventArgs e)
        {
            OnSelectedItemChanged();
        }
        void lvi_GotFocus(object sender, RoutedEventArgs e)
        {
            OnSelectedItemChanged(sender);
        }

        private void InitPanes()
        {
            if (Left == null)
            {
                Left = ListView1;
                Center = ListView2;
                Right = ListView3;
                Unused = ListView4;

                LeftV = View1;
                CenterV = View2;
                RightV = View3;
                UnusedV = View4;

                IsC5Visible = false;
                IsC1Visible = false;
                IsC2Visible = true;
                IsC3Visible = true;
                IsC4Visible = true;
            }
            SetPaneColumns();
        }

        void VosExplorer_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext as Vob != null)
            {
                CurrentVob = (Vob)DataContext;
            }
            else
            {
                if (V.Root != null)
                {
                    CurrentVob = V.Root;

                    //#if TEMP
                    var testVob = V.Root["Test123/456/789/0"];
                    testVob = V.Root["Test123/4/7/1"];
                    CurrentVob = testVob;
                    //#endif
                }
            }
            UpdateContents();
        }

        #endregion

        #region Factories

        private IEnumerable<ListViewItem> GetItems(IEnumerable<Vob> vobs)
        {


            foreach (var vob in vobs)
            {
                //var lvi = new VobListViewItem()
                var lvi = new ListViewItem()
                {

                    Content = vob,
                    Tag = vob,
                    DataContext = vob,
                    ContentTemplate = lviTemplate,
                };
                lvi.GotFocus += lvi_GotFocus;
                lvi.MouseEnter += lvi_MouseEnter;
                lvi.MouseUp += lvi_MouseUp;
                yield return lvi;
            }
        }

        #endregion

        #region ListViews

        public IEnumerable<ListView> ListViews
        {
            get
            {
                yield return ListView1;
                yield return ListView2;
                yield return ListView3;
                yield return ListView4;
            }
        }
        public IEnumerable<FrameworkElement> Views
        {
            get
            {
                yield return View1;
                yield return View2;
                yield return View3;
                yield return View4;
            }
        }

        #endregion

        #region Event Handler

        void view_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSelectedItemChanged();

            //if (sender == Center)
            //{
            //    l.Trace("view_SelectionChanged (Center)");
            //    if (Center.SelectedItems.Count == 1)
            //    {
            //        //CurrentVob = ((ListViewItem)Center.SelectedItem).Tag as Vob;
            //        //Back();
            //    }
            //}


            //foreach (var addedItem in e.AddedItems)
            //{
            //    l.Debug("TODO: view_SelectionChanged added: " + ((ListView)sender).SelectedItem);
            //}
            //foreach (var addedItem in e.RemovedItems)
            //{
            //    l.Debug("TODO: view_SelectionChanged removed: " + ((ListView)sender).SelectedItem);
            //}

            //l.Debug("TODO: view_SelectionChanged " +( (ListView)sender).SelectedItem );
        }

        #endregion

        #region Update Methods

        private void OnSelectedItemChanged(object sender = null)
        {
            FrameworkElement fe = sender as FrameworkElement;

            if (fe != null && fe.IsFocused)
            {
                ShowDetail(fe);
                //Details1.DataContext = fe;
                return;
            }
            // if (Center.IsFocused)
            //{
            //    if (Center.SelectedItems.Count == 1)
            //    {
            //        ShowDetail(Center.SelectedItem);
            //        //var se = Center.SelectedItem as FrameworkElement;
            //        //Details1.DataContext = se.Tag;
            //    }
            //}
            //else // if (Right.IsFocused)
            //{
            //    //l.Trace("view_SelectionChanged (Right)");
            //    if (Right.SelectedItems.Count == 1)
            //    {
            //        //CurrentVob = ((ListViewItem)Right.SelectedItem).Tag as Vob;

            //        //var se = Right.SelectedItem as FrameworkElement;

            //        ShowDetail(Right.SelectedItem);

            //        //Forward(CurrentVob);
            //    }
            //}
        }

        private void ShowDetail(object obj)
        {

            var fe = obj as FrameworkElement;
            if (fe != null)
            {
                obj = fe.Tag;
            }

            Vob vob = obj as Vob;
            if (vob != null)
            {
                l.Trace("ShowDetail: " + vob);
                Details1.DataContext = vob;
            }
            else
            {
                l.Trace("??? ShowDetail: " + obj);

            }
        }

        private void SetPaneColumns()
        {
            C1.Width = new GridLength(0);
            C4.Width = new GridLength(0);
            C2.Width = new GridLength(1, GridUnitType.Star);
            C5.Width = new GridLength(1, GridUnitType.Star);

            Grid.SetColumn(LeftV, 0);
            Grid.SetColumn(CenterV, 1);
            Grid.SetColumn(RightV, 2);
            Grid.SetColumn(UnusedV, 3);
            Grid.SetColumn(Detail1, 4);
            //Grid.SetColumn(Detail2, 4);

            //Unused.Visibility = System.Windows.Visibility.Hidden;
            //Left.Visibility = System.Windows.Visibility.Hidden;
            //Right.Visibility = System.Windows.Visibility.Visible;
            //Center.Visibility = System.Windows.Visibility.Visible;

            UpdateCanBack();
        }

        private void UpdateCanBack()
        {
            //BackButton1.Visibility =
            //    BackButton2.Visibility =
            //    //Center.Visibility =
            //    CanBack ? Visibility.Visible : Visibility.Hidden;
        }

        private void UpdateContents()
        {
            if (CurrentVob == null)
            {
                Right.ItemsSource = null;
                Center.ItemsSource = null;
                BreadcrumbBar.Path = null;
            }
            else
            {
                if (BreadcrumbBar.Root == null)
                {
                    //BreadcrumbBar.Root = new Odyssey.Controls.BreadcrumbItem()
                    //{
                    //    DataContext = V.Root,
                    //    Tag = V.Root,
                    //    Header = "/",
                    //};
                    BreadcrumbBar.Root = V.Root;

                }
                BreadcrumbBar.Path = CurrentVob.Path;

                Right.ItemsSource = GetItems(CurrentVob.GetChildren());
                Right.SelectedItem = null;

                var parent = CurrentVob.Parent;
                if (parent == CurrentVob) parent = null;
                Center.ItemsSource = parent == null ? null : GetItems(parent.GetChildren());
                if (Center.ItemsSource != null)
                {
                    Center.SelectedItem = Center.ItemsSource.OfType<ListViewItem>().Where(lvi => lvi.Tag == CurrentVob).FirstOrDefault();
                }

                if (parent != null)
                {
                    var parent2 = parent.Parent;
                    if (parent2 == parent) parent2 = null;
                    Left.ItemsSource = parent2 == null ? null : GetItems(parent2.GetChildren());
                    if (Left.ItemsSource != null)
                    {
                        Left.SelectedItem = Left.ItemsSource.OfType<ListViewItem>().Where(lvi => lvi.Tag == parent).FirstOrDefault();
                    }
                }
            }
            Left.ItemsSource = null;
            Unused.ItemsSource = null;
        }

        #endregion





        void lvi_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ListViewItem lvi = (ListViewItem)sender;
            var parent = lvi.Parent;

            ListView lv = null;
            for (DependencyObject lParent = lvi; (lv = lParent as ListView) == null; lParent = VisualTreeHelper.GetParent(lParent))
            {
                if (lParent == null)
                {
                    string msg = "ListView parent not found for ListViewItem";
                    l.Error(msg);
                    return;
                    //throw new Exception(msg);
                }
            }

            l.Trace("MouseUp:  " + lv + " " + ((ListViewItem)sender).Tag);
            if (lv == Right)
            {
                Forward(lvi.Tag as Vob);
                //CurrentVob = lvi.Tag as Vob;
                //OnCurrentVobChanged();
            }
            else if (lv == Center)
            {
                var oldVob = CurrentVob;
                string oldName = CurrentVob.Name;
                CurrentVob = lvi.Tag as Vob;

                if (oldVob != CurrentVob)
                {
                    var listView = Con1.BeginSwitchMulti(CurrentVob.Name.CompareTo(oldName) < 0 ? ExpandDirection.Down : ExpandDirection.Up, 1) as VobListView;
                    listView.ListView.ItemsSource = CurrentVob.GetChildren();

                    UpdateContents();
                }
            }
        }

        //private void OnCurrentVobChanged()
        //{
        //    UpdateContents();
        //    UpdateCanBack();
        //}

        void lvi_MouseEnter(object sender, MouseEventArgs e)
        {
            ListViewItem lvi = (ListViewItem)sender;
            var parent = lvi.Parent;
            var lParent = LogicalTreeHelper.GetParent(lvi);
            if (lParent == Right)
            {
                Right.SelectedItem = lvi;
            }
            //l.Trace("MouseEnter:  '" + parent + "' " + ((ListViewItem)sender).Tag);
        }



        private ListView Unused;
        private ListView Left;
        private ListView Right;
        private ListView Center;
        private FrameworkElement UnusedV;
        private FrameworkElement LeftV;
        private FrameworkElement RightV;
        private FrameworkElement CenterV;

        private ListView newU;
        private ListView newL;
        private ListView newR;
        private ListView newC;
        private FrameworkElement newUV;
        private FrameworkElement newLV;
        private FrameworkElement newRV;
        private FrameworkElement newCV;

        void VosExplorer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back
                || e.Key == Key.Left
                )
            {
                Back();
                e.Handled = true;
            }

            //if (e.Key == Key.Enter)
            //{
            //}
        }

        #region Methods

        public bool CanBack
        {
            get
            {
                if (CurrentVob == null
                    || CurrentVob.Parent == null) return false;

                return true;
            }
        }

        public void Back()
        {

            if (!CanBack) return;


            CurrentVob = CurrentVob.Parent;

            newL = Unused;
            newLV = UnusedV;

            newC = Left;
            newCV = LeftV;

            newR = Center;
            newRV = CenterV;

            newU = Right;
            newUV = RightV;

            //Unused.ItemsSource = CurrentVob.GetChildren();
            var parent = CurrentVob.Parent;
            Left.ItemsSource = parent == null ? null : parent.GetChildren();
            //Grid.SetColumn(newCV, 0);

            var listView = Con1.BeginSwitch(ExpandDirection.Right) as VobListView;
            listView.ListView.ItemsSource = parent == null ? null : parent.GetChildren();
            
            DoTransition(false);
        }

        public void Forward(Vob selectedItem)
        {
            CurrentVob = selectedItem as Vob;

            var listView = Con1.BeginSwitch(ExpandDirection.Left) as VobListView;
            listView.ListView.ItemsSource = CurrentVob.GetChildren();

            Unused.ItemsSource = CurrentVob.GetChildren();

            newL = Center;
            newLV = CenterV;

            newC = Right;
            newCV = RightV;

            newR = Unused;
            newRV = UnusedV;

            newU = Left;
            newUV = LeftV;

            DoTransition();
        }

        Storyboard sb;
        TimeSpan ts;
        private void DoTransition(bool forward = true)
        {
            double ms = 250;
            ts = TimeSpan.FromMilliseconds(ms);
            Duration dur = new Duration(ts);

            //IEasingFunction easing = new System.Windows.Media.Animation.PowerEase()
            //{
            //    EasingMode = EasingMode.EaseOut,
            //    Power = 2,
            //};

            IEasingFunction easing = new System.Windows.Media.Animation.CubicEase()
            {
                EasingMode = EasingMode.EaseOut,
            };

#if true

            if (newL == ListView1)
            {
                //AnimatedContentControl z;

                AC1.PositionAnimationDuration = ts;
                AC1.SizeAnimationDuration = ts;

                AC1.AnimatePosition(200, 200);
                //AC1.AnimateSize(50, 50);

            }

            ColumnDefinition grow, shrink;
            if (forward)
            {
                grow = C4;
                Grid.SetColumn(UnusedV, 3);

                shrink = C2;


            }
            else
            {
                grow = C1;
                Grid.SetColumn(UnusedV, 3);

                shrink = C3;
            }


            sb = new Storyboard();
            sb.DecelerationRatio = 0.2;

            //double toWidth = 0.33333;
            double toWidth = C3.ActualWidth;

            // Grow
            {
                var col = grow;
                var anim = new GridLengthAnimation();
                anim.From = new GridLength(col.ActualWidth);
                anim.To = new GridLength(toWidth);
                anim.Duration = dur;
                
                l.Trace("grow from " + col.ActualWidth + " to " + toWidth);

                Storyboard.SetTarget(anim, col);
                Storyboard.SetTargetProperty(anim, new PropertyPath("(ColumnDefinition.Width)"));


                sb.Children.Add(anim);
                anim.EasingFunction = easing;

            }

            // Shrink
            {
                var col = shrink;
                var anim = new GridLengthAnimation();
                anim.From = new GridLength(col.ActualWidth);
                anim.To = new GridLength(0);
                anim.Duration = dur;
                l.Trace("shrink from " + col.ActualWidth + " to 0");

                Storyboard.SetTarget(anim, col);
                Storyboard.SetTargetProperty(anim, new PropertyPath("(ColumnDefinition.Width)"));

                sb.Children.Add(anim);
                anim.EasingFunction = easing;

            }
            sb.Completed += sb_Completed;
            sb.FillBehavior = FillBehavior.Stop;
            sb.Begin();
#else
            if (forward)
            {
                IsC5Visible = true;
                IsC2Visible = false;
            }
            else // back
            {
                Left.Visibility = System.Windows.Visibility.Visible;

                IsC1Visible = true;
                IsC4Visible = false;
            }

            sb = new Storyboard();

            sb.Completed += sb_Completed;
            sb.Begin();
#endif

            Unused = newU;
            UnusedV = newUV;

            Left = newL;
            LeftV = newLV;

            Right = newR;
            RightV = newRV;

            Center = newC;
            CenterV = newCV;
        }

        void sb_Completed(object sender, EventArgs e)
        {
            //Unused.Visibility = System.Windows.Visibility.Hidden;
            //Left.Visibility = System.Windows.Visibility.Hidden;
            //Detail2.Visibility = System.Windows.Visibility.Hidden;

            SetPaneColumns();

            UpdateContents();

            if (newL == ListView1)
            {
                //AnimatedContentControl z;

                AC1.PositionAnimationDuration = ts;
                AC1.SizeAnimationDuration = ts;

                AC1.AnimatePosition(-200, -200);
                //AC1.AnimateSize(50, 50);

            }
        }



        #endregion

        #region Column Visibility Properties

        #region IsC1Visible

        public bool IsC1Visible
        {
            get { return isC1Visible; }
            set
            {
                if (isC1Visible == value) return;
                isC1Visible = value;
                OnPropertyChanged("IsC1Visible");
            }
        } private bool isC1Visible;

        #endregion

        #region IsC2Visible

        public bool IsC2Visible
        {
            get { return isC2Visible; }
            set
            {
                if (isC2Visible == value) return;
                isC2Visible = value;
                OnPropertyChanged("IsC2Visible");
            }
        } private bool isC2Visible;

        #endregion

        #region IsC3Visible

        public bool IsC3Visible
        {
            get { return isC3Visible; }
            set
            {
                if (isC3Visible == value) return;
                isC3Visible = value;
                OnPropertyChanged("IsC3Visible");
            }
        } private bool isC3Visible;

        #endregion

        #region IsC4Visible

        public bool IsC4Visible
        {
            get { return isC4Visible; }
            set
            {
                if (isC4Visible == value) return;
                isC4Visible = value;
                OnPropertyChanged("IsC4Visible");
            }
        } private bool isC4Visible;

        #endregion

        #region IsC5Visible

        public bool IsC5Visible
        {
            get { return isC5Visible; }
            set
            {
                if (isC5Visible == value) return;
                isC5Visible = value;
                OnPropertyChanged("IsC5Visible");
            }
        } private bool isC5Visible;

        #endregion

        #endregion

        #region CurrentVob

        public Vob CurrentVob
        {
            get => currentVob;
            set
            {
                if (currentVob == value) return;
                currentVob = value;

                l.Info("CurrentVob = " + value);

                BreadcrumbBar.Path = CurrentVob == null ? null : CurrentVob.Path;

                UpdateCanBack();

                var ev = CurrentVobChanged;
                if (ev != null) ev();
                OnPropertyChanged("CurrentVob");
            }
        }
        private Vob currentVob;

        public event Action CurrentVobChanged;

        #endregion

        #region Misc

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var ev = PropertyChanged;
            if (ev != null) ev(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private static readonly ILogger l = Log.Get();

        #endregion

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    Back();
        //}

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Back();
        }

        private void TextBlock_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            l.Info("TextBlock_MouseDown_1");
        }

        //private void Button_Click_3(object sender, RoutedEventArgs e)
        //{
        //    Forward(null);
        //}

    }
}
