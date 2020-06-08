using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Reflection;
using LionFire.Collections;
using System.Windows.Documents;
using LionFire.Avalon;
using LionFire.Applications;
//using AppUpdate;
//using AppUpdate.Common;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Diagnostics;
using Caliburn.Micro;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using LionFire.Dependencies;
using LionFire.UI;
using LionFire.Applications.Splash;

namespace LionFire.Shell
{
    // WIP - Migrate from statics and globals to IServiceProvider

    /// <summary>
    /// Manages several Presenters (ShellContentPresenters), which are typically windows
    /// (typically one full screen presenter per monitor, but perhaps small desktop windows
    /// for small utilities outside the app.)
    /// </summary>
    /// <remarks>
    /// TODO - Refactor bg and fg code to parameterized layers.
    /// </remarks>
    public abstract class WpfShell : ILionFireShell, INotifyClosing, INotifyPropertyChanged
    {
        #region (Static) Instance

        public static WpfShell Instance => DependencyContext.Default.GetService<WpfShell>();

        #endregion

        #region Dependencies

        public IHostApplicationLifetime HostApplicationLifetime { get; }

        public Dispatcher Dispatcher => Application.Dispatcher;

        #region EventAggregator

        public IEventAggregator EventAggregator
        {
            get => eventAggregator ??= new EventAggregator();
            set => eventAggregator = value;
        }
        private IEventAggregator eventAggregator;

        #endregion

        #region Options

        public LionFireShellOptions ShellOptions => shellOptionsMonitor.CurrentValue;
        public IOptionsMonitor<LionFireShellOptions> shellOptionsMonitor { get; }

        public RootInterfaceOptions InterfaceOptions => interfaceOptionsMonitor.CurrentValue;
        public IOptionsMonitor<RootInterfaceOptions> interfaceOptionsMonitor { get; }

        #endregion

        IUpdaterShell UpdaterShell => updaterShell;
        IUpdaterShell updaterShell;

        #endregion

        #region Configuration // TOPORT

        #region Debug

        SourceLevels DataBindingSourceLevel = System.Diagnostics.SourceLevels.Verbose;

#if DEBUG
        public static Type AutostartControl;
        public static bool IsAutostartControlMode { get { return AutostartControl != null; } }
#else
        public const bool IsAutostartControlMode = false;
#endif

        #endregion

        #region Parameters // TOPORT

        #region Derived from Args

        public bool StartInFullScreen
        {
            get
            {
                if (Args.Contains("fullscreen")) return true;
                if (Args.Contains("windowed")) return false;

                return ShellOptions.StartMaximizedToFullScreen;
            }
        }

        #endregion

        #endregion

        #region Parameters: Command line args // TOPORT

        public IList<string> Args
        {
            get { return args; }
            set
            {
                if (args == value) return;
                if (args != default(IList<string>)) throw new AlreadySetException();
                args = value;
            }
        }
        private IList<string> args;

        #endregion

        #endregion

        #region TEMP

        // Temp notes:
        object RootWindow { get; set; }
        object RootView { get; set; }
        object RootViewModel { get; set; }
        bool ShutdownOnRootWindowsClose { get; set; }
        List<Window> RootWindows { get; set; }

        #endregion

        #region Construction and Initialization

        #region (Static) Launch

        public const string LaunchTabName = "__Launch";

        /// <summary>
        /// Launch an app based on a Framework Element type.
        /// </summary>
        /// <typeparam name="StartupControlType"></typeparam>
        public static void Run(Type startupControlType)
        {
            if (!typeof(FrameworkElement).IsAssignableFrom(startupControlType)) { throw new ArgumentException("startupControlType must be a FrameworkElement"); }

            foreach (var mi in typeof(WpfShell).GetMethods().Where(x => x.Name == "Run" && x.GetGenericArguments().Length == 1))
            {
                var miG = mi.MakeGenericMethod(startupControlType);
                miG.Invoke(null, new object[] { });
                return;
            }
            throw new UnreachableCodeException();
        }

        #endregion

        public WpfShell(IHostApplicationLifetime hostApplicationLifetime)
        {
            HostApplicationLifetime = hostApplicationLifetime;

            HostApplicationLifetime.ApplicationStarted.Register(OnApplicationStarted);
            HostApplicationLifetime.ApplicationStarted.Register(OnApplicationStopping);

#if DEBUG
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = DataBindingSourceLevel;
#endif

            AttachWpf();

            System.Windows.Media.Animation.Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(System.Windows.Media.Animation.Timeline),
                new FrameworkPropertyMetadata { DefaultValue = 120 }
                );

            //if (instance != null) throw new AlreadyException("A LionFireShell has already been created.  Only one can exist per application.");
            //instance = this;

            // Doesn't look like this can be here for all derived classes, unless they inherit from this??
            Application.Resources.Source = new Uri("pack://application:,,,/LionFire.Avalon;component/Resources/default-lfa.xaml");

            InstanceStackRegistrar.Instance.Register<LionFire.Alerting.IAlerter>(new WpfAlerter()); // Fallback
            InstanceStackRegistrar.Instance.Register<LionFire.Alerting.IAlerter>(this);

            //Alerter.Alert("TEST!!!!", title: "Title1kljlkj", detail: "my detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabc", ex: new ArgumentNullException("arg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lk"));
        }

        private void AttachWpf()
        {
            LionFire.Events.CommandManager.AddRequerySuggested = h => System.Windows.Input.CommandManager.RequerySuggested += h;
            LionFire.Events.CommandManager.RemoveRequerySuggested = h => System.Windows.Input.CommandManager.RequerySuggested -= h;
        }

        #region IHostApplicationLifetime

        void OnApplicationStarted()
        {
            this.EventAggregator.PublicationThreadMarshaller = a => { if (!Dispatcher.CheckAccess()) { Dispatcher.BeginInvoke(a); return; } else { a(); } };

            Args = new List<string>((/*e?.Args ??*/ System.Environment.GetCommandLineArgs()).Select(x => x.ToLower())); // Make args lowercase?!

#if TOPORT // Show SplashScreen if enabled
            if (app.Capabilities.HasFlag(LionFireAppCapabilities.SplashScreen))
            {
                IsSplashVisible = true;
            }
#endif

            wpfHasStarted = true;

            // TODO: event
            // Hand over control to the app, to keep logic there instead of this GUI-specific 
            // shell code.
            // Do it in a separate thread so that the splash screen is alive
            //AppTask = Task.Factory.StartNew(LionFireApp.Current.Start); // Raises AppStarted

            Dispatcher.BeginInvoke(() =>
            {
                OnStarted();
                //Started?.Invoke();
            });
        }

        void OnApplicationStopping()
        {
            isAppFinished = true;
            Close();
        }

        #endregion

        #region WpfLionFireShell Events

        /// <summary>
        /// Invoked before WPF has started, and before LionFireApp has started.
        /// </summary>
        protected virtual void OnStarting()
        {
            #region FullScreen vs Windowed  DEADCODE - always start windowed (ShellWindow) and let that window maximize itself

            //bool isFullScreen = App.IsFullScreenDefault;

            //if (App.Args.Contains("fullscreen")) isFullScreen = false;
            //if (App.Args.Contains("windowed")) isFullScreen = true; // windowed wins!

            ////if (isFullScreen)
            ////{
            //    this.StartupUri = new Uri("pack://application:,,,/FullScreenShellWindow.xaml");
            ////}
            ////else
            ////{
            //this.StartupUri = new Uri("pack://application:,,,/ShellWindow.xaml", UriKind.RelativeOrAbsolute);
            //this.StartupUri = new Uri("pack://siteoforigin:,,,/ShellWindow.xaml", UriKind.RelativeOrAbsolute);
            //this.StartupUri = new Uri("/ShellWindow.xaml", UriKind.Relative);
            ////}
            //this.StartupUri = new Uri("ShellWindow.xaml", UriKind.Relative);

            #endregion
        }

        /// <summary>
        /// Invoked after WPF Application has started, and LionFireApp has started.
        /// </summary>
        protected virtual void OnStarted()
        {
            if (!Dispatcher.CheckAccess()) { Dispatcher.BeginInvoke(new Action(() => OnStarted())); return; }

            if (deferredAskUserToUpdate)
            {
                l.LogCritical("deferredAskUserToUpdate");
                AskUserToUpdate(); // Modal
            }

            if (!isAppFinished && !MainPresenter.HasTabs)
            {
                ShowRootInterface();
            }

            if (!isAppFinished)
            {
                MainPresenter.Show();
                this.MainWindow = MainPresenter.CurrentWindow;
            }
        }

        #endregion

        #region Close

        /// <summary>
        /// Invoked by Close()
        /// </summary>
        /// <returns>True if close should proceed, false if user requested cancel</returns>
        public virtual bool OnClosing()
        {
            return true;
        }

        /// <summary>
        /// Close the app, and close all of the presenters.  Finally, shut down the underlying native WPF Application.
        /// </summary>
        public void Close()
        {
            if (!Dispatcher.CheckAccess()) { Dispatcher.BeginInvoke(new Action(Close)); return; }

            if (!OnClosing())
            {
                l.Info("Close canceled");
                return;
            }

            IsDebugWindowVisible = false;

            var presenters = Presenters.Values.ToArray();
            Presenters.Clear();
            foreach (ShellContentPresenter scp in presenters)
            {
                try
                {
                    scp.Close();
                }
                catch (Exception ex)
                {
                    l.Error(ex.ToString());
                }
            }
            //MainPresenter.Close();

            Application?.Shutdown();

            HostApplicationLifetime.StopApplication();
        }

        #endregion

        #endregion

        #region State

        private bool wpfHasStarted;
        private bool isAppFinished = false;

        #endregion

        #region IHostedService

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ShowRootInterface();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            MainPresenter.Close();

            return Task.CompletedTask;
        }

        #endregion

        ////// REEVALUATE BELOW - Use more DI //////

        #region Shell Hierarchy

        // Application
        // ShellContentPresenter
        // MainWindow
        // Tabs

        #region Native WPF Application

        #region Application

        public Application Application
        {
            get => application ??= ApplicationProvider();
            set => application = value;
        }
        private Application application;

        protected virtual Func<Application> ApplicationProvider
        {
            get
            {
                return () =>
                {
                    if (Application.Current != null) { return Application.Current; }
                    else { return new Application(); }
                };
            }
        }

        #endregion

#if TOPORT // Some more extrinsic way of seeing what capabilities are available
        public LionFireAppCapabilities Capabilities
        {
            get
            {
                var c = LionFireAppCapabilities.Unspecified;
                if (SplashScreenType != null) { c |= LionFireAppCapabilities.SplashScreen; }
                return c;
            }
        }
#endif

#if OLD
        public int Run()
        {
            return Application.Run();
        }
        public void Shutdown()
        {
            Application.Shutdown();
        }
#endif

        #endregion

        #region Windowing

        public bool MinimizedAll = false;

        public Window MainWindow { get => Application.MainWindow; set => Application.MainWindow = value; }

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
                //this.MainWindow.Topmost = IsTopmost;
                if (MainPresenter.HasFullScreenShellWindow)
                {
                    MainPresenter.FullScreenShellWindow.Topmost = this.topmost;
                }
                if (MainPresenter.HasShellWindow)
                {
                    MainPresenter.ShellWindow.Topmost = this.topmost;
                }
            }
        }

        #endregion

        public void BringToFront()
        {
            if (!Dispatcher.CheckAccess()) Dispatcher.BeginInvoke(new Action(() => BringToFront()));
            else
            {
                if (MainPresenter.HasFullScreenShellWindow)
                {
                    MainPresenter.FullScreenShellWindow.BringIntoView();
                }
                if (MainPresenter.HasShellWindow)
                {
                    Window Window = MainPresenter.ShellWindow;
                    if (!Window.IsVisible)
                    {
                        Window.Show();
                    }

                    if (Window.WindowState == WindowState.Minimized)
                    {
                        Window.WindowState = WindowState.Normal;
                    }

                    Window.Activate();
                    Window.Topmost = true;  // important
                    Window.Topmost = false; // important
                    Window.Focus();         // important

                    MoveToForeground.DoOnProcess(Process.GetCurrentProcess().ProcessName);

                    MainPresenter.ShellWindow.WindowState = WindowState.Normal;
                    MainPresenter.ShellWindow.BringIntoView();
                }
            }
        }

        #endregion

        #region Tabs

        /// <summary>
        /// Used as the tab name when DefaultViewType is used.
        /// </summary>
        public const string DefaultTabName = "__Default";

        #endregion

        #endregion

        #region (Protected) RootInterface

        protected virtual void ShowRootInterface()
        {
            if (InterfaceOptions.RootViewAction != null)
            {
                InterfaceOptions.RootViewAction();
            }
            else if (InterfaceOptions.RootViewType != null)
            {
                ShowControl(InterfaceOptions.RootViewType, DefaultTabName);
            }
            else
            {
                MessageBox.Show("No default view or view action");
            }
        }

        #endregion

        #region IKeyboardShell

        public bool IsEditingText { get; set; }

        #endregion

        #region IUpdatingShell

        bool deferredAskUserToUpdate;

        #region AutoUpdate

        /// <summary>
        /// MODAL Updates are available, so ask the user if they wish to download and install them  now.
        /// </summary>
        /// <returns>false if application should stop what it's doing and update itself</returns>
        public bool AskUserToUpdate()
        {
            if (!wpfHasStarted)
            {
                deferredAskUserToUpdate = true; // Defer until WPF Application is started
                                                // FUTURE: could defer only if we need to ask the user something
                return true;
            }
            deferredAskUserToUpdate = false; // reset

            l.Info("[AUTOUPDATER] UPDATE AVAILABLE");

            // TODO: Hide the UpdateManager reference in the Application framework?
            var dr = MessageBox.Show(
                    string.Format("Updates are available to your software ({0} total). Do you want to download and prepare them now? You can always do this at a later time.",
                        updaterShell.UpdatesAvailable
                    //UpdateManager.Instance.UpdatesAvailable
                    ),
                    "Software updates available",
                     MessageBoxButton.YesNo);

            if (dr == MessageBoxResult.Yes)
            {
                return false; // Signifies the app should stop initializing and go forward with the update!
            }
            return true;
        }

        //private void OnAutoUpdateCheckFinished(bool needsUpdate)
        //{
        //    if (needsUpdate)
        //    {                
        //        AskUserToUpdate();
        //    }
        //    else
        //    {
        //        l.Info("App is up to date");
        //    }
        //}
        #endregion

        #endregion

        #region State

        #region State Flags

        #region Derived

        public bool IsActive
        {
            get
            {
                foreach (var presenter in Presenters.Values.ToArray())
                {
                    if (presenter.IsActive) return true;
                }
                return false;
            }
        }

        #endregion

        #endregion


        #endregion

        #region Exception Handling

        /// <summary>
        /// Platform-default user exception handler
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public bool AskUserToContinueOnException(Exception ex)
        {
            //if (!isStarted)
            //{
            //    deferredAskUserToUpdate = true; // Defer until WPF Application is started
            //    // FUTURE: could defer only if we need to ask the user something
            //    return true;
            //}
            //deferredAskUserToUpdate = false; // reset

            l.Info("[FATAL] Asking user whether to continue after unhandled exception: " + ex.ToString());

#if DEBUG
            System.Diagnostics.Debugger.Break();
#endif

            // TODO: Hide the UpdateManager reference in the Application framework?
            var dr = MessageBox.Show(
                    "An unhandled exception occurred.  Do you wish to continue?  If no, the application will be aborted.  Exception: " + Environment.NewLine + ex.ToString().Substring(0, Math.Min(500, ex.ToString().Length)),
                    "Unhandled exception",
                     MessageBoxButton.YesNo);

            if (dr == MessageBoxResult.Yes)
            {
                l.LogCritical("User chose to continue despite unhandled exception.");
                return true;
            }
            l.LogCritical("User chose to abort due to unhandled exception.");
            return false;
        }

        #endregion

        #region Presenter Framework

        #region Presenters Collection

        // REFACTOR - Think about DI


        #region PresenterTypes

        public Dictionary<string, Type> PresenterTypes = new Dictionary<string, Type>();

        #endregion

        #region Presenters

        public MultiBindableDictionary<string, ShellContentPresenter> Presenters = new MultiBindableDictionary<string, ShellContentPresenter>();

        #endregion
        // Tag of the the TabItem in a ShellContentPresent is the key.
        // ShellContentPresenters's contain tabItems
        // The App contains potentially several ShellContentPresenters, typically one per monitor

        // Now what about MDI windows?  Do they live on top?  I guess some could, and some could live in the tab.  
        // If they live in the Tab, I guess the tab has a canvas layer.  
        // Or there could be a dock manager!  Same deal.

        public string GetPresenterNameForControl(string controlKey)
        {
            foreach (KeyValuePair<string, ShellContentPresenter> kvp in Presenters)
            {
                ShellContentPresenter scp = kvp.Value;
                if (scp.Contains(controlKey))
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        #endregion

        #region MainPresenter

        private const string MainPresenterName = "_Main";

        IShellContentPresenter IPresenterShell.MainPresenter => this.MainPresenter;
        public ShellContentPresenter MainPresenter
        {
            get
            {
                if (mainPresenter == null)
                {
                    //Thread t = new Thread(new ThreadStart(() =>
                    //    {
                    mainPresenter = new ShellContentPresenter(this)
                    {
                        Name = MainPresenterName,
                    };
                    //    }
                    //));
                    //t.SetApartmentState(ApartmentState.STA);
                    //t.Start();
                    //t.Join();
                    Presenters.Add(mainPresenter.Name, mainPresenter);
                    mainPresenter.Closed += new Action<ShellContentPresenter>(mainPresenter_Closed);
                    //mainPresenter.Closing += new Action<ShellContentPresenter>(mainPresenter_Closing);
                }
                return mainPresenter;
            }
        }
        private ShellContentPresenter mainPresenter = null;

        //void mainPresenter_Closing(ShellContentPresenter obj)
        //{
        //    WindowsSettings.Save();
        //}

        void mainPresenter_Closed(ShellContentPresenter obj)
        {
            Presenters.Remove(obj.Name);

            //if (Presenters.Count > 1) l.Trace("UNTESTED - Closing with multiple presenters.");  -Close clears Presenters // This logic may have multiple shutdowns and could be cleaned up.

            if (CloseOnMainPresenterClose)
            {
                Close();
            }
        }
        public bool CloseOnMainPresenterClose = true;

        #endregion

        #region Tab Management

        public object ShowControl(Type type, string tabName = null
            //, FrameworkElement frameworkElement = null OLD
            )
        {
            MethodInfo mi = typeof(WpfShell).GetMethod("ShowControl", new Type[] { typeof(string) }).MakeGenericMethod(type);
            return mi.Invoke(WpfShell.Instance, new object[] { null
                //, frameworkElement 
            });
        }

        public T ShowControl<T>(string tabName = null
            //, T frameworkElement = null OLD
            ) where T : FrameworkElement
        {
            return MainPresenter.PushTab<T>(tabName);

            //return MainPresenter.GetControl<T>(tabName
            //    //, frameworkElement OLD
            //    , showControl: true);
        }

        //public T GetControl<T>(string tabName = null OLD
        //    //, T frameworkElement = null
        //    , bool showControl = false) where T : FrameworkElement
        //{
        //    var result = MainPresenter.GetControl<T>(tabName
        //        //, frameworkElement
        //        , showControl);
        //}

        //public void AddControl<T>(T controlInstance, string tabName = null) where T : FrameworkElement // UNUSED 
        //{
        //    mainPresenter.AddControl(controlInstance, tabName);
        //}

        #endregion

#if UNUSED
        public void LaunchPopupWindow(Type type) // Does this make sense?
        {
            var fe = (FrameworkElement)Activator.CreateInstance(type);

            var scp = new ShellContentPresenter(this);
            scp.Content = fe;

            scp.MinWidth = 20;
            scp.Width = double.NaN;
            scp.Height = double.NaN;
            scp.ShellWindow.SizeToContent = SizeToContent.WidthAndHeight;
            //scp.ShellWindow.WindowStyle = WindowStyle.None;

            scp.Show();
        }
#endif

        #endregion

        #region Debug Window

        // REFACTOR - DI

        protected virtual Func<FrameworkElement> DebugWindowContent { get { return () => null; } }

        public Window DebugWindow { get { return debugWindow; } }
        protected Window debugWindow;

        public bool IsDebugWindowVisible
        {
            get { return debugWindow != null && debugWindow.Visibility == Visibility.Visible; }
            set
            {
                if (value)
                {
                    var content = DebugWindowContent();
                    if (content == null)
                    {
                        l.Warn("IsDebugWindowVisible set to true, but LionFireShell.DebugWindowContent returned null.  (Override this to specify content.)");
                        return;
                    }

                    if (debugWindow == null)
                    {
                        debugWindow = new Window();
                        //debugWindow.Content = new ValorDebugPanel();
                        debugWindow.Content = content;
                        debugWindow.Width = 500;
                        debugWindow.Height = 500;
                        //debugWindow.SizeToContent = SizeToContent.WidthAndHeight;
                    }
                    debugWindow.Show();
                }
                else
                {
                    if (debugWindow != null)
                    {
                        debugWindow.Close();
                        debugWindow = null;
                    }
                }
            }
        }

        #endregion

        #region (Public) Window Methods

        #endregion

        #region Misc

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        private static readonly ILogger l = Log.Get();

        #endregion

    }
}
