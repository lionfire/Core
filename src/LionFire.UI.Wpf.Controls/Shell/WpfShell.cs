using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Reflection;
using LionFire.Collections;
using LionFire.Applications;
//using AppUpdate;
//using AppUpdate.Common;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Diagnostics;
using Caliburn.Micro;
#if NOESIS
using Noesis;
#if UNITY
using Dispatcher = LionFire.Dispatching.UnityThreadDispatcherWrapper;
#endif
#else
#endif
#if WPF
//using System.Windows.Documents;
//using System.Windows.Threading;
//using TPresenter = LionFire.Shell.Wpf.WpfShellConductor;
#endif
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using LionFire.Dependencies;
using LionFire.UI;
using LionFire.Applications.Splash;
using LionFire.Alerting;
using LionFire.UI.Wpf;
using LionFire.Threading;
using LionFire.Shell.Wpf;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Vos.VosApp;
using LionFire.UI.Windowing;
using System.Net.Http.Headers;

namespace LionFire.Shell
{
    /// <summary>
    /// Base class for WpfShell and NoesisUnityShell
    /// 
    /// Manages several Presenters (ShellContentPresenters), which are typically windows
    /// (typically one full screen presenter per monitor, but perhaps small desktop windows
    /// for small utilities outside the app.)
    /// </summary>
    /// <remarks>
    /// 
    /// Dependencies:
    ///  - Application
    ///  
    /// </remarks>
    public abstract class WpfShell :
        //LionFireShell, IWpfShell,
        INotifyPropertyChanged
    {
#if TOPORT
        #region IMPORTED

        #region (Static) Instance

        public static WpfShell Instance => DependencyContext.Default.GetService<WpfShell>();

        #endregion
        
        public Application Application { get; }

        //public WpfShell(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime, Application application, IOptionsMonitor<StartupInterfaceOptions> interfaceOptionsMonitor, IViewLocator viewLocator, IOptionsMonitor<ShellOptions> shellOptionsMonitor)
        //    : base(serviceProvider, hostApplicationLifetime, interfaceOptionsMonitor, viewLocator, shellOptionsMonitor)
        //{
            
        //}

        //protected override void OnConstructed()
        //{
        //    //InstanceStackRegistrar.Instance.Register<LionFire.Alerting.IAlerter>(new WpfAlerter()); // Fallback
        //    //InstanceStackRegistrar.Instance.Register<LionFire.Alerting.IAlerter>(this);

        //    //Alerter.Alert("TEST!!!!", title: "Title1kljlkj", detail: "my detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabcmy detail abcabc", ex: new ArgumentNullException("arg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lkg null exc lk;j lkj; lkj ;lkjklj lkj lk jlkj lk"));
        //}

        protected void OnClosed()
        {
            Application?.Shutdown(); // Superfluous to StopApplication?
        }

        #region Derived

        public Window MainWindow => ShellPresenter?.MainPresenter?.CurrentWindow as Window;

        #endregion

        #endregion

        #region Dependencies

        public IServiceProvider ServiceProvider { get; }
        public IHostApplicationLifetime HostApplicationLifetime { get; }
        public IDispatcher Dispatcher { get; }

        #region EventAggregator

        public IEventAggregator EventAggregator { get; }
        

        public IViewLocator ViewLocator { get; }

        #endregion

        #region Options

        public ShellOptions ShellOptions => ShellOptionsMonitor.CurrentValue;
        public IOptionsMonitor<ShellOptions> ShellOptionsMonitor { get; }

        public UIStartupOptions InterfaceOptions => InterfaceOptionsMonitor.CurrentValue;
        protected IOptionsMonitor<UIStartupOptions> InterfaceOptionsMonitor { get; }

        #endregion

        #region ShellPresenter

        [SetOnce]
        public TPresenter ShellPresenter
        {
            get => shellPresenter;
            set
            {
                if (shellPresenter == value) return;
                if (shellPresenter != default) throw new AlreadySetException();
                shellPresenter = value;
            }
        }
        private TPresenter shellPresenter;

        #endregion

        #region Optional: SplashView

        public ISplashView SplashView { get; }

        #endregion

        #endregion

        #region Construction and Initialization

        public WpfShell(IServiceProvider serviceProvider, IEventAggregator eventAggregator, IDispatcher dispatcher, IHostApplicationLifetime hostApplicationLifetime, Application application, IOptionsMonitor<UIStartupOptions> interfaceOptionsMonitor, IViewLocator viewLocator, IOptionsMonitor<ShellOptions> shellOptionsMonitor)
        {
            Dispatcher = dispatcher;

        #region Derived

            Application = application;
            // Doesn't look like this can be here for all derived classes, unless they inherit from this??
            Application.Resources.Source = new Uri("pack://application:,,,/LionFire.UI.Wpf.Controls;component/Resources/default-lfa.xaml");
            //Application.Startup += Application_Startup;

            WpfDispatcherAdapter = new WpfDispatcherAdapter(Application);

        #endregion

            System.Windows.Media.Animation.Timeline.DesiredFrameRateProperty.OverrideMetadata( // MOVE?
                typeof(System.Windows.Media.Animation.Timeline),
                new FrameworkPropertyMetadata { DefaultValue = 120 }
                );

        #region Dependencies

            ServiceProvider = serviceProvider;
            EventAggregator = eventAggregator;
            HostApplicationLifetime = hostApplicationLifetime;
            InterfaceOptionsMonitor = interfaceOptionsMonitor;
            ViewLocator = viewLocator;
            ShellPresenter = (TPresenter)ActivatorUtilities.CreateInstance(serviceProvider, typeof(TPresenter), this);
            //ShellConductor.Shell = this;
            this.ShellOptionsMonitor = shellOptionsMonitor;

        #region Optional

            SplashView = ServiceProvider.GetService<ISplashView>();

        #endregion

        #endregion

            HostApplicationLifetime.ApplicationStarted.Register(OnApplicationStarted);
            HostApplicationLifetime.ApplicationStarted.Register(OnApplicationStopping);

            // TODO: Get Args from DI registered service somewhere?  
            // TODO: Should args be parsed into Shell Options at a higher level, in the App?


            //if (instance != null) throw new AlreadyException("A LionFireShell has already been created.  Only one can exist per application.");
            //instance = this;

            OnConstructed();
        }

        protected virtual void OnConstructed() { }

        public async Task Initialize()
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<object>();

            Dispatcher.Invoke(() =>
            {
                ShellPresenter.Initialize();
                tcs.SetResult(null);
            });

            await tcs.Task.ConfigureAwait(false);
        }


        //private void Application_Startup(object sender, StartupEventArgs e) => throw new NotImplementedException();

        #endregion

        #region State

        private bool IsStarted;
        private bool isAppFinished = false;
        private bool ShowDefaultUI = true;
        public bool MinimizedAll = false;

        #region IUpdaterShell  TOPORT

        bool deferredAskUserToUpdate;

        #endregion

        #region IKeyboardShell

        public bool IsEditingText { get; set; }
        public bool IsActive { get; set; } // TOPORT - what does this refer to?

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
        public void Close()
        {
            if (IsClosed) { return; }
            IsClosed = true;

#if !NOESIS
            if (!Dispatcher.CheckAccess()) { Dispatcher.Invoke(Close); return; }
#endif

            if (!OnClosing())
            {
                l.Info("Close canceled");
                return;
            }

            //IsDebugWindowVisible = false; // TOPORT
            ShellPresenter.Close();

            OnClosed();
        }

        protected virtual void OnClosed()
        {
            HostApplicationLifetime.StopApplication();
        }

        #endregion

        #endregion

        #region IHostApplicationLifetime

        void OnApplicationStarted()
        {
            // OLD
            //if (ShellOptions.AutoStart)
            //{
            //    StartAsync(default).FireAndForget();
            //}
            // else: Still need to wait for hosting application to tell us to StartAsync.
        }

        async void OnApplicationStopping() => await StopAsync(default).ConfigureAwait(false);

        #endregion

        #region IHostedService

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (IsStarted) throw new AlreadyException("Already started");
            IsStarted = true;

            // REVIEW this whole method once initial porting done

            // TODO: event
            // Hand over control to the app, to keep logic there instead of this GUI-specific 
            // shell code.
            // Do it in a separate thread so that the splash screen is alive
            //AppTask = Task.Factory.StartNew(LionFireApp.Current.Start); // Raises AppStarted

            // TODO: Inject this via constructor injection
            //ShellConductor.WindowSettings = (await VosAppSettings.UserLocal<WindowSettings>.H.GetOrInstantiateValue().ConfigureAwait(false)).Value;

            if (ShellPresenter is IHostedService hs)
            {
                await hs.StartAsync(cancellationToken).ConfigureAwait(false);
            }

#if NOESIS
            // TOPORT - no await
            Dispatcher.BeginInvoke(async () =>
#else
            Dispatcher.BeginInvoke(async () =>
#endif
            {
#if TOPORT
            if (deferredAskUserToUpdate)
            {
                l.LogCritical("deferredAskUserToUpdate");
                IsSplashVisible = false;
                
                AskUserToUpdate(); // Modal // TOPORT 
            }
#endif

#if TOPORT // Show SplashScreen if enabled
            if (app.Capabilities.HasFlag(LionFireAppCapabilities.SplashScreen))
            {
                IsSplashVisible = true;
            }
#endif

        #region OLD - delete - FullScreen vs Windowed  DEADCODE - always start windowed (ShellWindow) and let that window maximize itself

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

            await ShellPresenter.Initialize().ConfigureAwait(false);
                if (!isAppFinished && !ShellPresenter.MainPresenter.HasTabs && ShowDefaultUI)
                {
                    ShellPresenter.ShowStartupInterfaces();
                }

            //IsSplashVisible = false;

            //if (!isAppFinished) // OLD - redundant to ShowRootViews
            //{
            //    MainPresenter.Show();
            //}

            //OnStarted();
            //Started?.Invoke();
        }).FireAndForget();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Close();
            await HostApplicationLifetime.ApplicationStopped.WaitHandle.WaitOneAsync(default).ConfigureAwait(false);
            Debug.WriteLine("WPF ShellBase stopped. UNTESTED"); // TODO - is this ever reached?
        }

        #endregion

#endif
        #region Misc

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        private static readonly ILogger l = Log.Get();

        #endregion
    }
}
