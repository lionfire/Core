#if NOESIS
using Noesis;
#else
#define Windowing
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LionFire.UI.Wpf;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using LionFire;
using LionFire.Shell;
using System.Threading;
using LionFire.Assets;
using LionFire.ObjectBus;
using LionFire.Alerting;
using System.Collections.Concurrent;
using LionFire.Structures;
using System.ComponentModel;
using LionFire.Persistence;
using Microsoft.Extensions.Logging;
using System.Reflection;
using LionFire.Ontology;
using LionFire.UI;
using System.Diagnostics;
using LionFire.Shell.Wpf;
using Microsoft.Extensions.Options;
using LionFire.UI.Windowing;
using LionFire.Dependencies;
//using LionFire.Vos.VosApp;
using Microsoft.Extensions.DependencyInjection;
using LionFire.ExtensionMethods;

namespace LionFire.ExtensionMethods
{
    public static class NoesisExtensions
    {
        public static T GetCustomAttribute2<T>(this MemberInfo type)
            where T : Attribute
            => System.Reflection.CustomAttributeExtensions.GetCustomAttribute<T>(type);

#if NOESIS

#endif
    }
}


namespace LionFire.Shell
{

    /// <summary>
    /// Intelligent content host for the main content area of the LionFire Shell.
    /// RENAME to WindowPresenter
    /// </summary>
    public partial class ShellContentPresenter : UserControl, /*IControllable, UNUSED*/ IShellContentPresenter, INotifyPropertyChanged
#if Windowing
    , IWpfWindowedPresenter
#endif
    {

        #region Window Parameters

        //public bool TransparentWindowedWindow = false;
        //public bool TransparentFullscreenWindow = false; // TODO

        #endregion

        #region Dependencies

        public IShellPresenter ShellPresenter { get; }
        //public IOptionsMonitor<WindowSettings> WindowSettingsOptions { get; }
        public WindowSettings WindowSettings => ShellPresenter.WindowSettings;

        //WindowSettingsOptions?.CurrentValue;

#if Windowing
        // TODO: Bind this to the current desktop profile
        // TODO: Change detection on profile change
        public WindowLayout WindowLayout
            => WindowSettings?.GetCurrentProfile().GetWindow(Name ?? WindowSettings.DefaultWindowName);
#endif

#if WPF
        IWpfShellContentPresenter MainPresenter => (IWpfShellContentPresenter)ShellPresenter.MainPresenter;
#else
        IShellContentPresenter MainPresenter => ShellPresenter.MainPresenter;
#endif

        #endregion


#if NOESIS

        Panel topControl;
        TabControl ShellTabControl;
        TabControl BackgroundShellTabControl;
        DockPanel ContentDock;
        ContentControl ModalControl;

        private void InitializeComponent()
        {

            Noesis.GUI.LoadComponent(this, "ShellContentPresenter.xaml");
        }
#endif

        public ShellContentPresenter(IShellPresenter shell, string name = null)
        {
            if (name != null) { Name = name; }

            //ApartmentState ap = Thread.CurrentThread.GetApartmentState();
            InitializeComponent();
            ShellPresenter = shell;

            //if (LionFireShellApp.Instance != null && !LionFireShellApp.Instance.GetType().IsAbstract)
            //{
            //    try
            //    {
            //        MainContentTab.Content = LionFireShellApp.Instance.StartupElement;
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception("Exception thrown when getting default content from LionFireShellApp.Instance.StartupElement", ex);
            //    }
            //}
        }

#region Properties

#region ShowMenuButton

        public bool ShowMenuButton
        {
            get => showMenuButton;
            set
            {
                if (showMenuButton == value) return;
                showMenuButton = value;
                OnPropertyChanged(nameof(ShowMenuButton));
            }
        }
        private bool showMenuButton;

#endregion

#endregion

        object IShellContentPresenter.TopControl => topControl;
        public Panel TopControl => topControl;

        //public object MainContent
        //{
        //    get { return MainContentTab.Content; }
        //    set { MainContentTab.Content = value; }
        //}

#region IControllable Members

        public object masterController;
        public object MasterController
        {
            get
            {
                return masterController;
            }
            set
            {
                if (masterController == value) return;

                if (value != null)
                {
                    if (masterController != null) throw new InvalidOperationException("MasterController must be set to null before assigning another master controller.");
                }
                masterController = value;
            }
        }

#endregion

#if NOESIS
        //public IWindow UnityWindow { get; set; } // FUTURE - stub/support for Unity main window (and potentially secondary windows.)
        public bool IsActive => true; // STUB

#else
#region Window Management

#region IsTopmost

        public bool Topmost
        {
            get => topmost;
            set
            {
                if (topmost == value) return;
                topmost = value;
                OnTopmostChanged();
                TopmostChanged?.Invoke(topmost);
            }
        }
        private bool topmost = false;

        public event Action<bool> TopmostChanged;

        protected virtual void OnTopmostChanged()
        {
            if (!Dispatcher.CheckAccess()) Dispatcher.BeginInvoke(new Action(() => OnTopmostChanged()));
            else
            {
#if Windowing
                if (MainPresenter is IWpfWindowedPresenter wp)
                {
                    //this.MainWindow.Topmost = IsTopmost;
                    if (wp.HasFullScreenShellWindow)
                    {
                        wp.FullScreenShellWindow.Topmost = this.topmost;
                    }
                    if (wp.HasShellWindow)
                    {
                        wp.ShellWindow.Topmost = this.topmost;
                    }
                }
#endif
            }
        }

#endregion

        public void BringToFront()
        {
            if (!Dispatcher.CheckAccess()) Dispatcher.BeginInvoke(new Action(() => BringToFront()));
            else
            {
                if (HasFullScreenShellWindow)
                {
                    FullScreenShellWindow.BringIntoView();
                }
                if (HasShellWindow)
                {
                    Window Window = ShellWindow;
                    if (!Window.IsVisible) { Window.Show(); }

                    if (Window.WindowState == WindowState.Minimized) { Window.WindowState = WindowState.Normal; }

                    Window.Activate();
                    Window.Topmost = true;  // important
                    Window.Topmost = false; // important
                    Window.Focus();         // important

                    MoveToForeground.DoOnProcess(Process.GetCurrentProcess().ProcessName);

                    ShellWindow.WindowState = WindowState.Normal;
                    ShellWindow.BringIntoView();
                }
            }
        }


#region Owning Window

        public bool IsFullScreen { get; internal set; }

#if Windowing

        private ShellWindow shellWindow;
        public ShellWindow ShellWindow
        {
            get
            {
                if (shellWindow == null && !isClosing)
                {
                    shellWindow = new ShellWindow(this)
                    {
                        Topmost = MainPresenter?.Topmost ?? false,
                        WindowLayout = WindowLayout,
                    };
                    shellWindow.Closed += new EventHandler(shellWindow_Closed);
                }
                return shellWindow;
            }
        }

        public bool HasShellWindow { get { return shellWindow != null; } }

        private FullScreenShellWindow fullScreenShellWindow;
        public FullScreenShellWindow FullScreenShellWindow
        {
            get
            {
                if (fullScreenShellWindow == null && !isClosing)
                {
                    fullScreenShellWindow = new FullScreenShellWindow(this)
                    {
                        Topmost = MainPresenter?.Topmost ?? false
                    };
                    fullScreenShellWindow.Closed += new EventHandler(fullScreenShellWindow_Closed);
                }
                return fullScreenShellWindow;
            }
        }
        public bool HasFullScreenShellWindow { get { return fullScreenShellWindow != null; } }

        void shellWindow_Closed(object sender, EventArgs e)
        {
            shellWindow = null;
            this.Close();
        }

        void fullScreenShellWindow_Closed(object sender, EventArgs e)
        {
            fullScreenShellWindow = null;
            this.Close();
        }

#endif

        //public System.Windows.Threading.Dispatcher CurrentDispatcher => CurrentWindow?.Dispatcher;

        object IWindowedPresenter.CurrentWindow => CurrentWindow;
        public Window CurrentWindow => IsFullScreen ? fullScreenShellWindow : (Window)shellWindow;

#endregion

#region Show / Hide

        public bool IsPresenterEnabled = true;

        public void Show()
        {
            if (!IsPresenterEnabled) return;
            if (IsFullScreen)
            {
                FullScreenShellWindow.Maximize();
            }
            else
            {
                ShellWindow.Restore();
            }
        }
        public void Hide()
        {
            var w = CurrentWindow;
            if (w != null)
            {
                w.Hide();
            }
        }

#endregion

        public bool IsActive
        {
            get
            {
                if (HasFullScreenShellWindow && FullScreenShellWindow.IsActive) return true;
                if (HasShellWindow && ShellWindow.IsActive) return true;
                return false;
            }
        }


#endregion
#endif

#region Close

        public event Action<ShellContentPresenter> Closed;
        //public event Action<ShellContentPresenter> Closing;

        private void Close_NotifyTabs()
        {
            var nc = this.CurrentDocumentTab as INotifyClosing;
            if (nc != null) nc.OnClosing();

            nc = this.CurrentTabContents as INotifyClosing;
            if (nc != null) nc.OnClosing();

            foreach (var tab in this.NavStack.Reverse().OfType<INotifyClosing>())
            {
                tab.OnClosing();
            }
        }

        public void Close()
        {
            //{ var ev = Closing; if (ev != null) ev(this); }

#if Windowing
            if (fullScreenShellWindow != null) fullScreenShellWindow.DoClose();
            if (shellWindow != null) shellWindow.DoClose();
#endif

            Closed?.Invoke(this);
            //if (IsFullScreen)
            //{
            //    FullScreenShellWindow.Close();
            //}
            //else
            //{
            //    ShellWindow.Close();
            //}
        }

        private bool isClosing = false;

#if Windowing
        internal void CloseWindows(Window windowIsClosing = null)
        {
            if (isClosing) return;


            isClosing = true;

            Close_NotifyTabs();

#if TOPORT
            LionFire.Avalon.WindowsSettings.Save(); // FUTURE - move to shellContentPresenter closing event?
#endif

#if Windowing
            if (this.shellWindow != null)
            {
                try
                {
                    var sw = shellWindow;
                    if (sw != null)
                    {
                        shellWindow = null;
                        if (sw != windowIsClosing)
                        {
                            sw.Close();
                        }
                    }

                }
                catch { }
            }
            if (this.fullScreenShellWindow != null)
            {
                try
                {
                    var fsw = fullScreenShellWindow;

                    if (fsw != null)
                    {
                        fullScreenShellWindow = null;
                        if (fsw != windowIsClosing)
                        {
                            fsw.Close();
                        }

                    }

                }
                catch { }
            }
#endif
        }
#endif

#endregion

#region Tab Management

#region CurrentTabName

        public string CurrentTabName
        {
            get => currentTabName;
            private set
            {
                if (currentTabName == value) return;
                currentTabName = value;

                CurrentTabNameChanged?.Invoke();
            }
        }
        private string currentTabName;

        public event Action CurrentTabNameChanged;

        public object CurrentTabContents
        {
            get
            {
                if (ShellTabControl == null) return null;
                var tabItem = ShellTabControl.SelectedItem as TabItem;
                if (tabItem != null) { return tabItem.Content; }
                else { return ShellTabControl.SelectedItem; }
            }
        }
        public IDocumentTab CurrentDocumentTab => CurrentTabContents as IDocumentTab;

#endregion

        public bool Contains(string name) => GetTabItem(name) != null;

        public TabItem GetTabItem(string name)
        {
            foreach (TabItem ti in ShellTabControl.Items.OfType<TabItem>())
            {
                if (ti.Tag != null && ti.Tag.Equals(name))
                {
                    return ti;
                }
            }
            return null;
        }

        public TabItem AddTabItem(string name, FrameworkElement control)
        {
            //control.Width = control.Height = Double.NaN;

            if (GetTabItem(name) != null) throw new ArgumentException("TabItem with the given name already exists.");

            TabItem ti = new TabItem();
            ti.Tag = name;
            ti.Header = name;

            ti.Content = control;

            ShellTabControl.Items.Add(ti);
            return ti;
        }

        void IShellContentPresenter.ShowTab(string tabKey)
        {
            ShowTab(tabKey);
        }

        public FrameworkElement ShowTab(string tabKey)
        {
            this.ShellTabControl.SelectedItem = GetTabItem(tabKey);

            this.CurrentTabName = tabKey;

            // MEMOPTIMIZE - Delete deselected tab, if marked transient
            var result = this.ShellTabControl.SelectedItem as FrameworkElement;

            this.CurrentTabViewName = TryGetTabViewName(result);

            return result;
        }


#region Current ViewName

        public string CurrentTabViewName
        {
            get;
            private set;
        }

        private string TryGetTabViewName(FrameworkElement result)
        {
            if (result == null) return null;

            var ti = result as TabItem;
            if (ti == null) return null;
            if (ti.Content == null) return null;

#if NOESIS
            // TODO: Use another extension method that avoids ambiguity with Noesis?
            var attr = System.Reflection.CustomAttributeExtensions.GetCustomAttribute<UIEntityAttribute>(ti.Content.GetType());
#else
            var attr = ti.Content.GetType().GetCustomAttribute<UIEntityAttribute>();
#endif
            if (attr == null) return null;
            return attr.Key;
        }

#endregion


#endregion

#region Background Tab Methods

        public bool ContainsBackground(string tabName)
        {
            return GetBackgroundTabItem(tabName) != null;
        }

        public TabItem GetBackgroundTabItem(string tabName)
        {
            foreach (TabItem ti in BackgroundShellTabControl.Items.OfType<TabItem>())
            {
                if (ti.Tag != null && ti.Tag.Equals(tabName))
                {
                    return ti;
                }
            }
            return null;
        }

        public TabItem AddBackgroundTabItem(string name, object control)
        {
            if (GetBackgroundTabItem(name) != null) throw new ArgumentException("TabItem with the given name already exists.");

            TabItem ti = new TabItem
            {
                Tag = name,
                Header = name,
                Content = control
            };

            BackgroundShellTabControl.Items.Add(ti);

            return ti;
        }

        public void ShowBackgroundTab(string tabKey)
        {
            var tabItem = GetBackgroundTabItem(tabKey);
            if (tabItem != this.BackgroundShellTabControl.SelectedItem)
            {
                this.BackgroundShellTabControl.SelectedItem = tabItem;
            }

            // MEMOPTIMIZE - Delete deselected tab, if marked transient
        }

#endregion

#region Routed Events

        private void OnBack(object sender, RoutedEventArgs args)
        {
            l.Trace("SCP.OnBack (routed)");
            CloseTab();
        }


        DocumentManager DocumentManager;

        private void OnSave(object sender, RoutedEventArgs args)
        {
            l.Trace("SCP.OnSave (routed)");
            DocumentManager?.Save();
        }

#endregion


        public void SetDock(FrameworkElement fe, bool isVisible = true)
        {

            if (fe == null) throw new ArgumentNullException("fe");

            Dock dock = DockPanel.GetDock(fe);

            if (!isVisible)
            {
                if (fe.Parent == ContentDock)
                {
                    ContentDock.Children.Remove(fe);
                }
                else if (fe.Parent != null)
                {
                    throw new ArgumentException("Element is already parented by another control");
                }
                else
                {

                }
            }
            else
            {
                if (fe.Parent == ContentDock)
                {

                }
                else if (fe.Parent != null)
                {
                    throw new ArgumentException("Element is already parented");
                }
                else
                {
                    ContentDock.Children.Insert(0, fe);
                }
            }
        }


#region ShowInTaskbar

        public bool ShowInTaskbar
        {
            get { return showInTaskbar; }
            set { showInTaskbar = value; }
        }
        private bool showInTaskbar = true;

#endregion

        public string StackTopTabName { get; set; }

#region Navigation Stack

        public bool HasTabs { get { return NavStack.Count > 0; } }


        private Stack<string> NavStack = new Stack<string>();

        //public void AddCloseStack<T>(T obj)
        //    where T : class
        //{
        //    string tabName = TabManager.GetTabNameFromType(obj);
        //    AddCloseStack(tabName);
        //}

        //public void AddCloseStack(string name)
        //{
        //    NavStack.Push(name);
        //}

        //public bool CloseTab<T>()
        //    where testc : FrameworkElement
        //{
        //    if()
        //    {
        //    }
        //}
        public bool CloseTab()
        {

            var docTab = this.CurrentDocumentTab;
            if (docTab != null)
            {
                var hasOptions = docTab as IHasDocumentTabOptions;

                DocumentTabOptions options = null;
                if (hasOptions != null)
                {
                    options = hasOptions.DocumentTabOptions;
                }
                if (options == null)
                {
                    options = DocumentTabOptions.Default;
                }

                if (options.SaveOnClose)
                {
                    DocumentManager?.Save();
                }
            }

            var closing = CurrentTabContents as INotifyClosing;
            if (closing != null)
            {
                if (!closing.OnClosing())
                {
                    l.Info("Closing of tab canceled: " + currentTabName);
                    return false;
                }
            }

            return DoCloseTab();

        }

        public bool DoCloseTab()
        {
            if (NavStack.Count > 0)
            {
                this.ShowTab(NavStack.Pop());
                return true;
            }
            else
            {
                l.Warn("PopControl invoked when NavStack is empty");
            }
            return false;
        }

        public T PushTab<T>(string tabName = null)
            where T : class
        {
            NavStack.Push(CurrentTabName);
            return GetControl<T>(tabName, showControl: true);
        }
        public object PushTab(Type type, string tabName = null)
        {
            NavStack.Push(CurrentTabName);
            return GetControl(type, tabName, showControl: true);
        }

#endregion

#region Tab Management

        public T ShowControl<T>(string tabName = null)
            where T : class
        {
            var control = GetControl<T>(tabName, showControl: true);
            return control; // REVIEW
        }

        public object GetControl(Type type, string tabName = null, bool showControl = false)
        {
            GetControlGenericMethodInfo ??= this.GetType().GetMethods().Where(mi => mi.Name == nameof(GetControl) && mi.ContainsGenericParameters).First();
            return GetControlGenericMethodInfo.MakeGenericMethod(type).Invoke(this, new object[] { tabName, showControl });
        }
        private static MethodInfo GetControlGenericMethodInfo;

        public T GetControl<T>(string tabName = null
            //, T frameworkElement = null
            , bool showControl = false
            //, bool pushToNavStackOnShow = true FUTURE?
            ) where T : class
        {
            l.Debug("ShowControl: " + typeof(T).Name + " showControl = " + showControl);

#if !NOESIS // FIXME?
            if (!this.Dispatcher.CheckAccess()) return (T)this.Dispatcher.Invoke(new Func<T>(() => GetControl<T>(tabName
                //, frameworkElement
                , showControl
                )));
            else
#endif
            {
                if (tabName == null)
                {
                    tabName = TabManager.GetTabNameFromType<T>(
                        //frameworkElement
                        );
                }

                FrameworkElement frameworkElement;

                if (!this.Contains(tabName))
                {
                    frameworkElement = (FrameworkElement)(object)ActivatorUtilities.CreateInstance<T>(ShellPresenter.ServiceProvider);
                    this.AddTabItem(tabName, frameworkElement);

                    var parented = frameworkElement as IParented<IShellContentPresenter>;
                    if (parented != null)
                    {
                        parented.Parent = this;
                    }
                }

                TabItem tabItem;
                if (showControl)
                {
                    tabItem = (TabItem)this.ShowTab(tabName);

                    //if (pushToNavStackOnShow)
                    //{

                    //}
                }
                else // Only get the item
                {
                    tabItem = (TabItem)this.GetTabItem(tabName);
                }

                //if (frameworkElement != null && tabItem.Content != frameworkElement) // TODO - remove this? UNNEEDED?
                //{
                //    l.Info("UNTESTED - replace TabItem.Content with explicit frameworkElement.");
                //    tabItem.Content = frameworkElement;
                //}
                return (T)tabItem.Content;
            }
        }

        //public void AddControl<T>(T controlInstance, string tabName = null) where T : FrameworkElement // UNUSED
        //{
        //    if (tabName == null)
        //    {
        //        var attr = typeof(T).GetCustomAttribute<ShellPresenterAttribute>();
        //        if (attr != null && !String.IsNullOrEmpty(attr.DefaultTabName))
        //        {
        //            tabName = attr.DefaultTabName;
        //        }
        //        else
        //        {
        //            tabName = typeof(T).Name;
        //        }
        //    }

        //    if (this.Contains(tabName))
        //    {
        //        throw new AlreadyException();
        //    }

        //    TabItem ti = this.GetTabItem(tabName);
        //    if (ti != null && ti.Content != controlInstance)
        //    {
        //        throw new AlreadyException("Unexpected: background tab name already exists: " + tabName);
        //    }

        //    this.AddTabItem(tabName, controlInstance);
        //}


        public bool HideModalControl(string viewName)
        {
            var control = this.ModalControl.Content;
            if (control == null) return true;

            var attr = control.GetType().GetCustomAttribute2<UIEntityAttribute>();
            if (attr.Key == viewName)
            {
                this.ModalControl.Visibility = Visibility.Collapsed;
                var fe = this.ModalControl.Content as FrameworkElement;
                if (fe != null) fe.DataContext = null;
                this.ModalControl.Content = null;
                return true;
            }

            return false;
        }


        public void HideModalControl<T>() where T : class
        {
            this.ModalControl.Visibility = Visibility.Collapsed;
            var fe = this.ModalControl.Content as FrameworkElement;
            if (fe != null) fe.DataContext = null;
            this.ModalControl.Content = null;
        }

        //ConcurrentQueue<KeyValuePair<Type, string>> modalQueue = new ConcurrentQueue<KeyValuePair<Type, string>>();

        //private void TryPopModalQueue()
        //{
        //    KeyValuePair<Type, string> kvp;
        //    if (modalQueue.TryDequeue(out kvp))
        //    {
        //        Alerter.Alert("TODO: Modal queue" );
        //        ShowModalControl
        //    }
        //}

        // TODO: What happens 

        public T ShowModalControl<T>(string tabName = null)
            where T : class
        {
            tabName = tabName ?? TabManager.GetTabNameFromType<T>();

            if (ModalControl.Content != null)
            {
                if (ModalControl.Content.GetType() == typeof(T))
                {
                    return this.ModalControl.Content as T;
                }

                Alerter.Alert("TODO: Handle multiple modal messages");
                //modalQueue.Enqueue(new KeyValuePair<Type, string>(typeof(T), tabName));
                return null;
            }

            T controlInstance = null;

            //if (!this.ContainsModal(tabName))
            {
                controlInstance = Activator.CreateInstance<T>();
                //this.AddModalTabItem(tabName, controlInstance);
            }
            //else
            //{
            //    TabItem ti = this.GetModalTabItem(tabName);
            //    if (ti != null)
            //    {
            //        controlInstance = ti.Content as T;
            //    }
            //    else
            //    {
            //        // TODO Assert failure
            //        l.Warn("TODO Assert failure in ShowModalControl");
            //    }
            //}
            this.ModalControl.Content = controlInstance;
            this.ModalControl.Visibility = Visibility.Visible;

            //this.ShowModalTab(tabName);

            return controlInstance;
        }

        public T ShowBackgroundControl<T>(string tabName = null) where T : class
        {
#if !NOESIS
            if (!Dispatcher.CheckAccess()) { return Dispatcher.Invoke(new Func<T>(() => ShowBackgroundControl<T>(tabName))); }
#endif

            tabName = tabName ?? TabManager.GetTabNameFromType<T>();

            T controlInstance = null;

            if (!this.ContainsBackground(tabName))
            {
                controlInstance = Activator.CreateInstance<T>();
                this.AddBackgroundTabItem(tabName, controlInstance);
            }
            else
            {
                TabItem ti = this.GetBackgroundTabItem(tabName);
                if (ti != null)
                {
                    controlInstance = ti.Content as T;
                }
                else
                {
                    // TODO Assert failure
                    l.Warn("TODO Assert failure in ShowBackgroundControl");
                }
            }
            this.ShowBackgroundTab(tabName);

            return controlInstance;
        }

        public void AddBackgroundControl<T>(T controlInstance, string tabName = null) where T : FrameworkElement
        {
            tabName = tabName ?? TabManager.GetTabNameFromType<T>();

            if (this.ContainsBackground(tabName)) { throw new AlreadyException(); }

            TabItem ti = this.GetBackgroundTabItem(tabName);
            if (ti != null && ti.Content != controlInstance)
            {
                throw new AlreadyException("Unexpected: background tab name already exists: " + tabName);
            }

            this.AddBackgroundTabItem(tabName, controlInstance);
        }

#region OLD

        //public void ShowPresenterType(string presenterType)
        //{
        //    if (Presenters.ContainsKey(presenterType))
        //    {

        //        return;
        //    }
        //    if (PresenterTypes.ContainsKey(presenterType))
        //    {
        //        var presenter = Activator.CreateInstance(PresenterTypes[presenterType]);

        //        //Presenters.Add(presenterType, presenter);

        //    }
        //}

#endregion

#endregion

#region Misc

        private static readonly ILogger l = Log.Get();

#region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

#endregion

#endregion
    }


}
