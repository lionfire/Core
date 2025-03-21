

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
using LionFire.Alerting;
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

namespace LionFire.Shell
{
    public class WpfLionFireShell<AppT, DefaultViewT> : WpfLionFireShell
        where AppT : ILionFireApp
        where DefaultViewT : FrameworkElement
    {
        public override Type AppType { get { return typeof(AppT); } }
        public override Type DefaultViewType { get { return typeof(DefaultViewT); } }
    }

    /// <summary>
    /// Manages several Presenters (ShellContentPresenters), which are typically windows
    /// (typically one full screen presenter per monitor, but perhaps small desktop windows
    /// for small utilities outside the app.)
    /// </summary>
    /// <remarks>
    /// TODO - Refactor bg and fg code to parameterized layers.
    /// </remarks>
    public abstract class WpfLionFireShell :
        //Application,
        IAlerter, ILionFireShell, INotifyClosing, INotifyPropertyChanged
    {
        public bool MinimizeAllOnFullScreen = true; // MOVE to settings
        public bool UndoMinimizeAllOnRestore = true; // MOVE to settings
        public bool MinimizedAll = false;

        #region Ontology

        #region (Static) Instance

        public static WpfLionFireShell Instance => instance;
        private static WpfLionFireShell instance;

        #endregion

        public ILionFireApp App => LionFireApp.Current;

        #endregion

        #region ILionFireShell Implementation

        public bool ProvidesRunLoop { get { return true; } }

        #endregion

        #region Configuration

        protected PerformanceMode PerformanceMode { get; set; } = PerformanceMode.HighPerformance;

        #region Required: App and View types

        public virtual Type AppType => null; // FUTURE: Allow a default?

        #region Default View

        /// <summary>
        /// Supercedes DefaultViewType.  This will be executed to show the initialdefault view.
        /// </summary>
        public virtual Action DefaultViewAction { get { return null; } }
        /// <summary>
        /// If DefaultViewAction is null, this will be instantiated to be the initial/default view.
        /// </summary>
        public abstract Type DefaultViewType { get; }

        /// <summary>
        /// Used as the tab name when DefaultViewType is used.
        /// </summary>
        public const string DefaultTabName = "__Default";

        protected virtual void ShowDefaultView()
        {
            if (DefaultViewAction != null)
            {
                DefaultViewAction();
            }
            else if (DefaultViewType != null)
            {
                ShowControl(DefaultViewType, DefaultTabName);
            }
            else
            {
                MessageBox.Show("No default view or view action");
            }
        }

        #endregion

        #endregion

        public virtual Type SplashScreenType => typeof(SplashWindow);

        /// <summary>
        /// Set to true to disable default Windows TitleBar and use the custom one.
        /// </summary>
        public virtual bool UseCustomTitleBar { get { return true; } }

        #region Default Configuration

        public static bool IsFullScreenDefault => !LionFire.Applications.LionFireApp.IsDevMode;

        // - window size  - OLD / TODO move to config, REVIEW
        public virtual Size DefaultWindowedSize => new Size(1368, 768);

        #endregion

        #region Debug

        SourceLevels DataBindingSourceLevel = System.Diagnostics.SourceLevels.Verbose;


#if DEBUG
        public static Type AutostartControl;
        public static bool IsAutostartControlMode { get { return AutostartControl != null; } }
#else
        public const bool IsAutostartControlMode = false;
#endif

        #endregion

        #endregion

        #region Parameters

        #region Derived from Args

        public bool StartInFullScreen
        {
            get
            {
                if (WpfLionFireShell.Instance.Args.Contains("fullscreen")) return true;
                if (WpfLionFireShell.Instance.Args.Contains("windowed")) return false;
                return WpfLionFireShell.IsFullScreenDefault;
            }
        }

        #endregion

        #endregion

        #region Construction and Initialization


        #region (Static) Launch

        public const string LaunchTabName = "__Launch";

        /// <summary>
        /// Launch an app based on a Framework Element type.
        /// DEPRECATED: Do not use this for major applications.  It is a quick way to avoid creating  Shell/App classes.
        /// </summary>
        /// <typeparam name="StartupControlType"></typeparam>
        public static void Run<StartupControlType>()
            where StartupControlType : FrameworkElement
        {
            var shell = new WpfLionFireShell<LionFireApp, StartupControlType>();

            //shell.Starting += () => { shell.ShowControl<StartupControlType>(LaunchTabName); 
            //    //shell.MainPresenter.Show(); 
            //};
            shell.Run();
        }

        /// <summary>
        /// Launch an app based on a Framework Element type.
        /// </summary>
        /// <typeparam name="StartupControlType"></typeparam>
        public static void Run(Type startupControlType)
        {
            if (!typeof(FrameworkElement).IsAssignableFrom(startupControlType)) { throw new ArgumentException("startupControlType must be a FrameworkElement"); }

            foreach (var mi in typeof(WpfLionFireShell).GetMethods().Where(x => x.Name == "Run" && x.GetGenericArguments().Length == 1))
            {
                var miG = mi.MakeGenericMethod(startupControlType);
                miG.Invoke(null, new object[] { });
                return;
            }
            throw new UnreachableCodeException();
        }

        #endregion

        private void AttachWpf()
        {

            LionFire.Events.CommandManager.AddRequerySuggested = h => System.Windows.Input.CommandManager.RequerySuggested += h;
            LionFire.Events.CommandManager.RemoveRequerySuggested = h => System.Windows.Input.CommandManager.RequerySuggested -= h;
        }

        public WpfLionFireShell()
        {
            LionFireShell.Instance = this;
#if DEBUG
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = DataBindingSourceLevel;
#endif

            AttachWpf();

            this.application = ApplicationFactory();
            this.application.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            System.Windows.Media.Animation.Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(System.Windows.Media.Animation.Timeline),
                new FrameworkPropertyMetadata { DefaultValue = 120 }
                );

            if (instance != null) throw new AlreadyException("A LionFireShell has already been created.  Only one can exist per application.");
            instance = this;

            Presenters.CollectionChanged += new NotifyCollectionChangedHandler<ShellContentPresenter>(Presenters_CollectionChanged);

            // Doesn't look like this can be here for all derived classes, unless they inherit from this??
            Application.Resources.Source = new Uri("pack://application:,,,/LionFire.Avalon;component/Resources/default-lfa.xaml");

            Application.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            InstanceStackRegistrar.Instance.Register<LionFire.Alerting.IAlerter>(new WpfAlerter()); // Fallback
            InstanceStackRegistrar.Instance.Register<LionFire.Alerting.IAlerter>(this);

            //Alerter.Alert("TEST!!!!", title: "Title1kljlkj", detail: "my detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabc", ex: new ArgumentNullException("arg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lk"));
        }


        private bool isAppFinished = false;

        #region Native WPF App Events

        protected void _BeforeOnStartup(StartupEventArgs e)
        {
            l.Trace("LionFireShell.OnStartup.  Thread: " + Thread.CurrentThread.ManagedThreadId);

            this.EventAggregator.PublicationThreadMarshaller = a => { if (!Dispatcher.CheckAccess()) { Dispatcher.BeginInvoke(a); return; } else { a(); } };

            Args = new List<string>((e?.Args ?? System.Environment.GetCommandLineArgs()).Select(x => x.ToLower())); // Make args lowercase?!

            if (PerformanceMode == PerformanceMode.HighPerformance)
            {
                System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.SustainedLowLatency;
            }

            OnStarting();
            Starting?.Invoke();

            if (AppType != null)
            {
                LionFireApp app = (LionFireApp)Activator.CreateInstance(AppType);
                app.Shell = this;

                if (app.Capabilities.HasFlag(LionFireAppCapabilities.SplashScreen))
                {
                    IsSplashVisible = true;
                }
            }
        }

        protected void _AfterOnStartup(StartupEventArgs _)
        {
            wpfHasStarted = true;

            LionFireApp.Current.AppStarted += OnAppStarted;
            LionFireApp.Current.AppFinished += () =>
            {
                isAppFinished = true;
                Close();
            };

            // Hand over control to the app, to keep logic there instead of this GUI-specific 
            // shell code.
            // Do it in a separate thread so that the splash screen is alive
            AppTask = Task.Factory.StartNew(LionFireApp.Current.Start); // Raises AppStarted
        }

        #endregion

        #region LionFire App Events

        Task AppTask; // UNUSED

        private void OnAppStarted()
        {
            l.Trace("LionFireShell.OnAppStarted");
            BeginInvoke(() =>
                {
                    l.Trace("LionFireShell.OnAppStarted#lambda");
                    LionFireApp.Current.AppStarted -= OnAppStarted;

                    OnStarted();
                    { var ev = Started; if (ev != null) ev(); }
                });
        }

        #endregion

        #region WpfLionFireShell Events

        /// <summary>
        /// Raised after OnStarting
        /// </summary>
        private event Action Starting; // UNUSED

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
        /// Raised after OnStarted
        /// </summary>
        private event Action Started; // UNUSED
        /// <summary>
        /// Invoked after WPF Application has started, and LionFireApp has started.
        /// </summary>
        protected virtual void OnStarted()
        {
            if (!Dispatcher.CheckAccess()) { Dispatcher.BeginInvoke(new Action(() => OnStarted())); return; }

            if (deferredAskUserToUpdate)
            {
                l.Fatal("deferredAskUserToUpdate");
                IsSplashVisible = false;
                AskUserToUpdate(); // Modal
            }

            if (!isAppFinished && !MainPresenter.HasTabs)
            {
                ShowDefaultView();
            }

            if (!isAppFinished)
            {
                MainPresenter.Show();
                this.MainWindow = MainPresenter.CurrentWindow;
            }
            IsSplashVisible = false;
        }

        #endregion

        #region Close

        /// <summary>
        /// Invoked by Close()
        /// </summary>
        /// <returns>True if close should proceed, false if user requested cancel</returns>
        public virtual bool OnClosing()
        {
            return LionFire.Applications.LionFireApp.Current.OnClosing();
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

            Application.Shutdown();
        }

        #endregion

        #endregion

        #region State

        #region Parameters: Command line args

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

        public DateTime StartTime { get { return App.StartTime; } }

        public bool IsEditingText
        {
            get; set;
        }

        #region State Flags

        bool wpfHasStarted;
        bool deferredAskUserToUpdate;




        #region IsTopmost

        public bool Topmost
        {
            get { return topmost; }
            set
            {
                if (topmost == value) return;
                topmost = value;
                OnTopmostChanged();
                var ev = TopmostChanged; if (ev != null) ev(topmost);
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

        #region Splash

        // REFACTOR - DI
        protected bool IsSplashVisible
        {
            get { return splash != null && splash.Visibility == Visibility.Visible; }
            set
            {
                if (!Dispatcher.CheckAccess()) { Dispatcher.BeginInvoke(new Action(() => IsSplashVisible = value)); return; }

                if (value && SplashScreenType != null)
                {
                    splash = Activator.CreateInstance(this.SplashScreenType) as Window;
                    if (splash != null)
                    {
                        splash.Show();
                        this.MainWindow = null;
                    }
                    // else SILENTFAIL
                }
                else
                {
                    if (splash != null)
                    {
                        //splash.Hide();
                        splash.Close();
                        splash = null;
                    }
                }
            }
        }
        private Window splash;

        #endregion

        #region Native WPF Application

        protected virtual Func<Application> ApplicationFactory
        {
            get
            {
                return () =>
                {
                    if (Application.Current != null)
                    {

                        return Application.Current;
                    }
                    else
                    {
                        return new LionFireWindowsApplication()
                        {
                            BeforeOnStartup = _BeforeOnStartup,
                            AfterOnStartup = _AfterOnStartup,
                        };
                    }
                };
            }
        }

        public Application Application
        {
            get { return application; }
        }
        private Application application;

        public LionFireAppCapabilities Capabilities
        {
            get
            {
                var c = LionFireAppCapabilities.Unspecified;
                if (SplashScreenType != null) { c |= LionFireAppCapabilities.SplashScreen; }
                return c;
            }
        }

        public Window MainWindow { get { return Application.MainWindow; } set { Application.MainWindow = value; } }
        public Dispatcher Dispatcher { get { return Application.Dispatcher; } }
        public int Run()
        {
            return Application.Run();
        }
        public void Shutdown()
        {
            Application.Shutdown();
        }

        #endregion

        #region EventAggregator

        public IEventAggregator EventAggregator
        {
            get
            {
                if (eventAggregator == null)
                {
                    eventAggregator = new EventAggregator();
                }
                return eventAggregator;
            }
            set { eventAggregator = value; }
        }
        private IEventAggregator eventAggregator;

        #endregion


        #region Invoke

        public void Invoke(Action action)
        {
            Dispatcher.Invoke(action);
        }
        object ILionFireShell.Invoke(Func<object> f)
        {
            if (Application == null) { return f(); }
            return Application.Dispatcher.Invoke(f);
        }

        public void BeginInvoke(Action action)
        {
            Dispatcher.BeginInvoke(action, DispatcherPriority.ApplicationIdle);
        }

        #endregion

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
                    LionFireApp.Current.UpdatesAvailable
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

        #region Exception Handling

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            l.Fatal("DispatcherUnhandledException: " + e.Exception.ToString());
            LionFireApp.Current.OnApplicationDispatcherException(sender, e);
        }


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
                l.Fatal("User chose to continue despite unhandled exception.");
                return true;
            }
            l.Fatal("User chose to abort due to unhandled exception.");
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

        void Presenters_CollectionChanged(NotifyCollectionChangedEventArgs<ShellContentPresenter> e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {

                }
            }
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                }
            }
        }

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

        IShellContentPresenter ILionFireShell.MainPresenter
        {
            get { return this.MainPresenter; }
        }
        public ShellContentPresenter MainPresenter
        {
            get
            {
                if (mainPresenter == null)
                {
                    //Thread t = new Thread(new ThreadStart(() =>
                    //    {
                    mainPresenter = new ShellContentPresenter()
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
            MethodInfo mi = typeof(WpfLionFireShell).GetMethod("ShowControl", new Type[] { typeof(string) }).MakeGenericMethod(type);
            return mi.Invoke(WpfLionFireShell.Instance, new object[] { null
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

        public void LaunchPopupWindow(Type type)
        {
            var fe = (FrameworkElement)Activator.CreateInstance(type);

            var scp = new ShellContentPresenter();
            scp.Content = fe;

            scp.MinWidth = 20;
            scp.Width = double.NaN;
            scp.Height = double.NaN;
            scp.ShellWindow.SizeToContent = SizeToContent.WidthAndHeight;
            //scp.ShellWindow.WindowStyle = WindowStyle.None;

            scp.Show();
        }

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

        #region Alert

        // REFACTOR- DI

        #region IsAlertOpen

        public bool IsAlertOpen
        {
            get { return isAlertOpen; }
            set
            {
                if (isAlertOpen == value) return;
                isAlertOpen = value;

                var ev = IsAlertOpenChanged;
                if (ev != null) ev();
            }
        }
        private bool isAlertOpen;

        public event Action IsAlertOpenChanged;

        #endregion

        public virtual void Alert(Alert alert)
        {
            if (!Dispatcher.CheckAccess()) { Dispatcher.BeginInvoke(new Action(() => Alert(alert))); return; }
            var layer = MainPresenter.TopControl;

            if (layer == null)
            {
                l.Fatal("Failed to get adorner layer for alert: " + alert.Message + " " + alert.Exception);
                // TODO: Throw exception and use a fallback alerter
                return;
            }

            var a = new AlertAdorner() { DataContext = alert, Layer = layer };
            a.SetValue(Grid.ZIndexProperty, 99);
            layer.Children.Add(a);
            IsAlertOpen = true;
        }

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

    }

    public enum PerformanceMode
    {
        Unspecified = 0,
        HighPerformance

    }
}
