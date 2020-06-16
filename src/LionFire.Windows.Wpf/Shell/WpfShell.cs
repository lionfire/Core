using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Reflection;
using LionFire.Collections;
using System.Windows.Documents;
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
using LionFire.Alerting;
using LionFire.Windows.Wpf;
using LionFire.Threading;
using LionFire.Shell.Wpf;

namespace LionFire.Shell
{
    public interface IWpfShell : IHostedService, IKeyboardShell { }

    /// <summary>
    /// Manages several Presenters (ShellContentPresenters), which are typically windows
    /// (typically one full screen presenter per monitor, but perhaps small desktop windows
    /// for small utilities outside the app.)
    /// </summary>
    /// <remarks>
    /// TODO - Refactor bg and fg code to parameterized layers.
    /// </remarks>
    public class WpfShell : IWpfShell, INotifyClosing, INotifyPropertyChanged
    {
        #region (Static) Instance

        public static WpfShell Instance => DependencyContext.Default.GetService<WpfShell>();

        #endregion

        #region Dependencies

        public IHostApplicationLifetime HostApplicationLifetime { get; }

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

        public Dispatcher Dispatcher => Application.Dispatcher;

        #region EventAggregator

        public IEventAggregator EventAggregator
        {
            get => eventAggregator ??= new EventAggregator();
            set => eventAggregator = value;
        }
        private IEventAggregator eventAggregator;

        public IViewLocator ViewLocator { get; }

        #endregion

        #region Options

        public LionFireShellOptions ShellOptions => shellOptionsMonitor.CurrentValue;
        public IOptionsMonitor<LionFireShellOptions> shellOptionsMonitor { get; }

        public RootInterfaceOptions InterfaceOptions => InterfaceOptionsMonitor.CurrentValue;
        protected IOptionsMonitor<RootInterfaceOptions> InterfaceOptionsMonitor { get; }

        #endregion

        //public IShellPresenter ShellPresenter { get; }

        #region ShellPresenter

        [SetOnce]
        public IShellPresenter ShellPresenter
        {
            get => shellPresenter;
            set
            {
                if (shellPresenter == value) return;
                if (shellPresenter != default) throw new AlreadySetException();
                shellPresenter = value;
                if(shellPresenter is WpfShellPresenter wsp)
                {
                    wsp.Shell = this;
                }
            }
        }
        private IShellPresenter shellPresenter;

        #endregion


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

        public bool StopOnMainPresenterClose { get; set; } = true; // TODO: Make this a settable option in UIReference.StopShellOnClose

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

        public WpfShell(IHostApplicationLifetime hostApplicationLifetime, IOptionsMonitor<RootInterfaceOptions> interfaceOptionsMonitor, IViewLocator viewLocator, IShellPresenter shellPresenter, IOptionsMonitor<LionFireShellOptions> shellOptionsMonitor)

        {
            HostApplicationLifetime = hostApplicationLifetime;
            InterfaceOptionsMonitor = interfaceOptionsMonitor;
            ViewLocator = viewLocator;
            ShellPresenter = shellPresenter;
            this.shellOptionsMonitor = shellOptionsMonitor;

            HostApplicationLifetime.ApplicationStarted.Register(OnApplicationStarted);
            HostApplicationLifetime.ApplicationStarted.Register(OnApplicationStopping);

            // TODO: Get Args from DI registered service somewhere?  
            // TODO: Should args be parsed into Shell Options at a higher level, in the App?
            Args = new List<string>((/*e?.Args ??*/ System.Environment.GetCommandLineArgs()).Select(x => x.ToLower())); // Make args lowercase?!

            EventAggregator.PublicationThreadMarshaller = a => { if (!Dispatcher.CheckAccess()) { Dispatcher.BeginInvoke(a); return; } else { a(); } };

#if DEBUG
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = DataBindingSourceLevel;
#endif

            AttachCommandManagerToWpf();

            System.Windows.Media.Animation.Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(System.Windows.Media.Animation.Timeline),
                new FrameworkPropertyMetadata { DefaultValue = 120 }
                );

            //if (instance != null) throw new AlreadyException("A LionFireShell has already been created.  Only one can exist per application.");
            //instance = this;

            // Doesn't look like this can be here for all derived classes, unless they inherit from this??
            Application.Resources.Source = new Uri("pack://application:,,,/LionFire.Windows.wpf;component/Resources/default-lfa.xaml");

            InstanceStackRegistrar.Instance.Register<LionFire.Alerting.IAlerter>(new WpfAlerter()); // Fallback
            //InstanceStackRegistrar.Instance.Register<LionFire.Alerting.IAlerter>(this);

            //Alerter.Alert("TEST!!!!", title: "Title1kljlkj", detail: "my detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabc", ex: new ArgumentNullException("arg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lk"));
        }

        #region WPF connections

        private void AttachCommandManagerToWpf()
        {
            LionFire.Events.CommandManager.AddRequerySuggested = h => System.Windows.Input.CommandManager.RequerySuggested += h;
            LionFire.Events.CommandManager.RemoveRequerySuggested = h => System.Windows.Input.CommandManager.RequerySuggested -= h;
        }

        #endregion

        #endregion

        #region State

        private bool IsStarted;
        private bool isAppFinished = false;
        private bool ShowDefaultUI = true;
        public bool MinimizedAll = false;

        #region IKeyboardShell

        public bool IsEditingText { get; set; }

        #endregion

        #endregion

        #region (Public) Methods

        #region Close

        /// <summary>
        /// Invoked by Close()
        /// </summary>
        /// <returns>True if close should proceed, false if user requested cancel</returns>
        public virtual bool OnClosing()
        {
            return true;
        }

        private bool IsClosed = false;

        /// <summary>
        /// Close the app, and close all of the presenters.  Finally, shut down the underlying native WPF Application.
        /// </summary>
        public async Task Close()
        {
            if (IsClosed) { return; }
            IsClosed = true;

            if (!Dispatcher.CheckAccess()) { await Dispatcher.InvokeAsync(new Func<Task>(Close)); return; }

            if (!OnClosing())
            {
                l.Info("Close canceled");
                return;
            }

            //IsDebugWindowVisible = false; // TOPORT
            ShellPresenter.Close();


            Application?.Shutdown();

            HostApplicationLifetime.StopApplication();
        }

        #endregion

        #endregion

        #region IHostApplicationLifetime

        void OnApplicationStarted()
        {
            if (ShellOptions.AutoStart)
            {
                StartAsync(default).FireAndForget();
            }
            // else: Still need to wait for hosting application to tell us to StartAsync.
        }

        void OnApplicationStopping() => StopAsync(default);

        #endregion

        #region IHostedService

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (IsStarted) throw new AlreadyException("Already started");
            IsStarted = true;

#if TOPORT // Show SplashScreen if enabled
            if (app.Capabilities.HasFlag(LionFireAppCapabilities.SplashScreen))
            {
                IsSplashVisible = true;
            }
#endif

            // TODO: event
            // Hand over control to the app, to keep logic there instead of this GUI-specific 
            // shell code.
            // Do it in a separate thread so that the splash screen is alive
            //AppTask = Task.Factory.StartNew(LionFireApp.Current.Start); // Raises AppStarted

            await Dispatcher.BeginInvoke(() =>
            {
                //OnStarting();

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

                // TODO TOPORT - REVIEW - move this blocking/modal user update out of Shell into parent App
                //if (deferredAskUserToUpdate)
                //{
                //    l.LogCritical("deferredAskUserToUpdate");
                //    AskUserToUpdate(); // Modal
                //}

                if (!isAppFinished && !ShellPresenter.MainPresenter.HasTabs && ShowDefaultUI)
                {
                    ShellPresenter.ShowRootViews();
                }

                //if (!isAppFinished) // OLD - redundant to ShowRootViews
                //{
                //    MainPresenter.Show();
                //}

                //OnStarted();
                //Started?.Invoke();
            });
        }

        public Task StopAsync(CancellationToken cancellationToken) => Close();

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
