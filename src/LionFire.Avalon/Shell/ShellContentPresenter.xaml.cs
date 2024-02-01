﻿using System;
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
using LionFire;
using LionFire.Shell;
//using LionFire.Valor.Gui.Accounts;
using LionFire.Valor.Gui.Shell;
using System.Threading;
using LionFire.Assets;
using LionFire.ObjectBus;
using LionFire.Alerting;
using System.Collections.Concurrent;
using LionFire.Structures;
using System.ComponentModel;
using LionFire.Persistence;
#if NEW
using LionFire.Persistence.Handles;
#endif
using Microsoft.Extensions.Logging;
using System.Reflection;
using LionFire.Ontology;

namespace LionFire.Shell
{

    /// <summary>
    /// Intelligent content host for the main content area of the LionFire Shell.
    /// </summary>
    public partial class ShellContentPresenter : UserControl, IControllable, IShellContentPresenter, INotifyPropertyChanged
    {

#region Window Parameters

        //public bool TransparentWindowedWindow = false;
        //public bool TransparentFullscreenWindow = false; // TODO

#endregion

        public ShellContentPresenter()
        {
            //ApartmentState ap = Thread.CurrentThread.GetApartmentState();
            InitializeComponent();

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

        public bool ShowMenuButton {
            get { return showMenuButton; }
            set {
                if (showMenuButton == value) return;
                showMenuButton = value;
                OnPropertyChanged(nameof(ShowMenuButton));
            }
        }
        private bool showMenuButton;

#endregion

        

#endregion

        public Panel TopControl
        {
            get { return topControl; }
        }

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

#region Window Management

#region Owning Window

        public bool IsFullScreen { get; internal set; }

        private ShellWindow shellWindow;
        public ShellWindow ShellWindow
        {
            get
            {
                if (shellWindow == null && !isClosing)
                {
                    shellWindow = new ShellWindow(this);
                    shellWindow.Topmost = WpfLionFireShell.Instance.Topmost;
                    shellWindow.Closed += new EventHandler(shellWindow_Closed);
                }
                return shellWindow;
            }
        } public bool HasShellWindow { get { return shellWindow != null; } }

        private FullScreenShellWindow fullScreenShellWindow;
        public FullScreenShellWindow FullScreenShellWindow
        {
            get
            {
                if (fullScreenShellWindow == null && !isClosing)
                {
                    fullScreenShellWindow = new FullScreenShellWindow(this);
                    fullScreenShellWindow.Topmost = WpfLionFireShell.Instance.Topmost;
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

        public System.Windows.Threading.Dispatcher CurrentDispatcher
        {
            get
            {
                var w = CurrentWindow;
                return w == null ? null : w.Dispatcher;
            }
        }
        public Window CurrentWindow
        {
            get
            {
                return IsFullScreen ? fullScreenShellWindow : (Window)shellWindow;
            }
        }

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

            if (fullScreenShellWindow != null) fullScreenShellWindow.DoClose();
            if (shellWindow != null) shellWindow.DoClose();

            { var ev = Closed; if (ev != null) ev(this); }
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
        internal void CloseWindows(Window windowIsClosing = null)
        {
            if (isClosing) return;


            isClosing = true;

            Close_NotifyTabs();

            LionFire.Avalon.WindowsSettings.Save(); // FUTURE - move to shellContentPresenter closing event?

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

#region Tab Management

#region CurrentTabName

        public string CurrentTabName
        {
            get { return currentTabName; }
            private set
            {
                if (currentTabName == value) return;
                currentTabName = value;

                var ev = CurrentTabNameChanged;
                if (ev != null) ev();
            }
        } private string currentTabName;

        public event Action CurrentTabNameChanged;

        public object CurrentTabContents
        {
            get
            {
                if (ShellTabControl == null) return null;
                var tabItem = ShellTabControl.SelectedItem as TabItem;
                if (tabItem != null)
                {
                    return tabItem.Content;
                }
                else
                {
                    return ShellTabControl.SelectedItem;
                }
            }
        }
        public IDocumentTab CurrentDocumentTab
        {
            get
            {
                return CurrentTabContents as IDocumentTab;
            }
        }

#if NEW
        Use H<object>
#endif
        public IHandle CurrentDocumentHandle
        {
            get
            {
                var fe = CurrentDocumentTab as FrameworkElement;
                if (fe == null) return null;

                var handle = fe.DataContext as IHandle;
                if (handle != null) return handle;

                var hasHandle = fe.DataContext as IHasHandle;
                if (hasHandle != null) return hasHandle.Handle;

                return null;
            }
        }

#endregion

        public bool Contains(string name)
        {
            return GetTabItem(name) != null;
        }
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

            this.CurrentViewName = TryGetTabViewName(result);

            return result;
        }


#region Current ViewName

        public string CurrentViewName
        {
            get;
            private set;
        }

        public string CurrentTabViewName => "";

        private string TryGetTabViewName(FrameworkElement result)
        {
            if (result == null) return null;

            var ti = result as TabItem;
            if (ti == null) return null;
            if (ti.Content == null) return null;

            var attr = ti.Content.GetType().GetCustomAttribute<UIEntityAttribute>();
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

        public TabItem AddBackgroundTabItem(string name, FrameworkElement control)
        {
            if (GetBackgroundTabItem(name) != null) throw new ArgumentException("TabItem with the given name already exists.");

            TabItem ti = new TabItem();
            ti.Tag = name;
            ti.Header = name;

            ti.Content = control;

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

        private void OnSave(object sender, RoutedEventArgs args)
        {
            l.Trace("SCP.OnSave (routed)");
            Save();
        }

        public bool Save(FailAction failAction = FailAction.AlertUser)
        {
            var doc = this.CurrentDocumentHandle;

            try
            {
                if (doc == null)
                {
                    //throw new Exception("No document found in CurrentDocumentHandle");
                    return true;
                }

#if NEW
                doc.Commit();
#else
                doc.Save();
#endif
                l.Info("Saved: " + doc.Reference);
                return true;
            }
            catch (Exception ex)
            {
                l.Warn("Save exception:  " + ex);
                if (failAction == FailAction.AlertUser)
                {
                    Alerter.Alert("Failed to save", ex);
                }
                else if (failAction == FailAction.ThrowException)
                {
                    throw;
                }
                return false;
            }

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
        } private bool showInTaskbar = true;

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
                if(hasOptions != null)
                {
                    options = hasOptions.DocumentTabOptions;
                }
                if(options == null)
                {
                    options = DocumentTabOptions.Default; 
                }

                if (options.SaveOnClose)
                {
                    Save();
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
            where T : FrameworkElement
        {
            NavStack.Push(CurrentTabName);
            return GetControl<T>(tabName, showControl: true);
        }

#endregion

        

#region Tab Management

        public T GetControl<T>(string tabName = null
            //, T frameworkElement = null
            , bool showControl = false
            //, bool pushToNavStackOnShow = true FUTURE?
            ) where T : FrameworkElement
        {
            l.Debug("ShowControl: " + typeof(T).Name + " showControl = " + showControl);

            if (!this.Dispatcher.CheckAccess()) return (T)this.Dispatcher.Invoke(new Func<T>(() => GetControl<T>(tabName
                //, frameworkElement
                , showControl
                )));
            else
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
                    this.AddTabItem(tabName,
                        //frameworkElement ?? 
                        frameworkElement = Activator.CreateInstance<T>());

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

            var attr = control.GetType().GetCustomAttribute<UIEntityAttribute>();
            if (attr.Key == viewName)
            {
                this.ModalControl.Visibility = System.Windows.Visibility.Collapsed;
                var fe = this.ModalControl.Content as FrameworkElement;
                if (fe != null) fe.DataContext = null;
                this.ModalControl.Content = null;
                return true;
            }

            return false;
        }
        

        public void HideModalControl<T>() where T : FrameworkElement
        {
            this.ModalControl.Visibility = System.Windows.Visibility.Collapsed;
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
        
        public T ShowModalControl<T>(string tabName = null) where T : FrameworkElement
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
            this.ModalControl.Visibility = System.Windows.Visibility.Visible;

            //this.ShowModalTab(tabName);

            return controlInstance;
        }

        public T ShowBackgroundControl<T>(string tabName = null) where T : FrameworkElement
        {
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
