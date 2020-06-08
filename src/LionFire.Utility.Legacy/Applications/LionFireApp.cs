// TODO: IOC
//#define SPLASH_TEST
#if !UNITY 
//#define NAppUpdater
#else
#endif

#if NBug
using NBug;
#endif

#if NAppUpdater
using AppUpdate;
using AppUpdate.Tasks;
using AppUpdate.Common;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Windows.Threading;
using System.Diagnostics;
using LionFire.Shell;
using System.Reflection;
using System.IO;
//using LionFire.Alerting;
using System.ComponentModel;

using LionFire.Serialization;
using System.Threading;
using LionFire.Logging;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using LionFire.Services;
using LionFire.Collections;
using Caliburn.Micro;
using Microsoft.Extensions.Logging;
using LionFire.Collections.Concurrent;
using System.Windows.Threading;
using LionFire.Persistence;
using LionFire.Vos;
using LionFire.Resolvables;
using LionFire.Resolves;
using LionFire.Vos.VosApp;

namespace LionFire.Applications
{
    public class LionFireApp : LionFireAppBase
    //, ILionFireApp, IService
    {
        #region (Static) Current

        // TODO: Use DependencyContext like LionFireShell?
        public static LionFireApp Instance => Current;  // Prefer this one over Current?
        public static LionFireApp Current
        {
            get => current;
            set
            {
                lock (currentLock)
                {
                    if (current != null) throw new InvalidOperationException("Can only set app once.");
                    current = value;
                }
            }
        }
        static LionFireApp current;

        private static object currentLock = new object();

        #endregion

     

#if TOPORT // maybe
        //public static int TraceStackDepth { get; set; } // UNUSED
        IService IHasService.Service => this;

    #region EventAggregator

        public IEventAggregator EventAggregator
        {
            get
            {
                if (eventAggregator != null) return eventAggregator;
                if (Shell != null) return Shell.EventAggregator;
                return null;
            }
            set => eventAggregator = value;
        }
        private IEventAggregator eventAggregator;

    #endregion

    #region Registration Codes

        public virtual string TryRegister(string key) => null;

    #endregion


    #region Dispatcher

        public static Dispatcher DefaultDispatcher
        {
            get
            {
                if (LionFireApp.Current != null) return LionFireApp.Current.Dispatcher;
                return Dispatcher.CurrentDispatcher;
            }
        }
        public Dispatcher Dispatcher
        {
            get
            {
                if (dispatcher != null) return dispatcher;
#if PresentationFramework
                if(System.Windows.Application.Current != null && System.Windows.Application.Current.Dispatcher != null) return System.Windows.Application.Current.Dispatcher;
#endif
                return Dispatcher.CurrentDispatcher;
            }
            set { dispatcher = value; }
        }
        private Dispatcher dispatcher;

    #endregion

    #region Configuration

        public virtual string DefaultAppName
        {
            get
            {
                var name = this.GetType().Name;
                string Suffix = "App";
                if (name.EndsWith(Suffix) && name.Length > Suffix.Length) name = name.Substring(0, name.Length - Suffix.Length);
                return name;
            }
        }

        /// <summary>
        /// Defaults to the first part of the namespace.
        /// </summary>
        /// <remarks>
        /// If you use Java-style namespaces, e.g. ca.lionfire.www, you may want to tweak this.
        /// </remarks>
        public virtual string DefaultCompanyName
        {
            get
            {
                var ass = Assembly.GetEntryAssembly();
                if (ass != null)
                {
                    var ns = ass.EntryPoint.DeclaringType.Namespace;
                    var index = ns.IndexOf('.');
                    if (index >= 0)
                    {
                        return ns.Substring(0, index);
                    }
                    else
                    {
                        return ns;
                    };
                }
                return "DefaultCompany";
            }
        }

    #endregion

    #region AutoUpdate

    #region Configuration

        // TODO: Get from user config, allow user to change somehow
        public string CurrentReleaseChannel = "Prealpha";

        public static bool AutoUpdateAsync = false;
        public static bool AutoUpdateAlwaysRestart = true;

        /// <summary>
        /// Override this to allow auto updates
        /// </summary>
        public virtual bool AllowAutoUpdates
        {
            get { return false; }
        }

        public bool IsAutoUpdateEnabled
        {
            get
            {
                if (!AllowAutoUpdates) { return false; }
                if (!isAutoUpdateEnabled.HasValue)
                {
                    if (IsDevMode)
                    {
                        l.Info("[AUTOUPDATE] skipped due to DevMode");
                        isAutoUpdateEnabled = false;
                    }
                    else
                    {
                        isAutoUpdateEnabled = IsAutoUpdateAvailable;
                    }
                }
                return isAutoUpdateEnabled.Value;
            }
        }
        private bool? isAutoUpdateEnabled;

        public bool IsAutoUpdateCompiled
        {
            get
            {
#if NAppUpdater
                return true;
#else
                return false;
#endif
            }
        }
        public bool IsAutoUpdateAvailable
        {
            get
            {
                if (!UpdateFeedUrls.Any()) return false;
#if NAppUpdater
                return true;
#else
                return false;
#endif
            }
        }

    #region URLs

        //public string UpdateUrl = "http://lionfire.ca/files/test/feed.xml";

        //https://files.lionfire.ca/files/V/ForCom/PC/Prealpha/feed.xml

        public string[] UpdateHosts = new string[]
        {
            //"http://lionfire.ca/files/V/",
            "https://lionfire.ca/files/V/",
            //"http://files.lionfire.ca/files/V/",
        };

    #endregion

    #endregion

    #endregion

#endif
    #region Configuration Extras

    public virtual bool IsVosEnabled => true;
        public virtual bool IsPackagesEnabled => true;
        public virtual bool IsBugReporterEnabled => true;

        public virtual bool IsNetworkEnabled => false;

        #region UseLogSettings

        public bool UseLogSettings { get; set; } = true;

        #endregion

        #region TEMP - ForCom

        /// <summary>
        /// False for more futuristic/complicated/experimental stuff.  True for simple/shippable stuff
        /// TEMP MOVE
        /// </summary>
        public static bool IsForCom => true;

        #endregion

        #endregion


        #region Derived

        #region Derived Configuration

        #region Make DEBUG / TRACE define constants visible to application

        [Obsolete("MOVE to LionFireEnvironment.Compilation")]
        public const bool IsDebug =
#if DEBUG
 true;
#else
 false;
#endif

        [Obsolete("MOVE to LionFireEnvironment.Compilation")]
        public const bool IsTrace =
#if TRACE
 true;
#else
 false;
#endif

        #endregion

        #endregion

        #endregion

#if TOPORT // maybe

        #region Configuration Extras (2)

        /// <summary>
        /// A summary of app configuration in one property
        /// </summary>
        public virtual LionFireAppCapabilities Capabilities
        {
            get
            {
                LionFireAppCapabilities c = LionFireAppCapabilities.Unspecified;
                if (IsVosEnabled) { c |= LionFireAppCapabilities.Vos; }
                if (IsAutoUpdateEnabled) { c |= LionFireAppCapabilities.AutoUpdate; }
                if (IsPackagesEnabled) { c |= LionFireAppCapabilities.Packages; }
                if (IsBugReporterEnabled) { c |= LionFireAppCapabilities.BugReporter; }
                if (Shell != null)
                {
                    c |= LionFireAppCapabilities.Shell;
                    c |= Shell.Capabilities;
                }
                return c;
            }
        }

        //public Type SplashScreenType
        //{
        //    get
        //    {
        //        return (Shell == null) ? null : Shell.SplashScreenType;
        //    }
        //}

        #endregion

        #region Serialization Assemblies

        public virtual IEnumerable<Assembly> SerializationAssemblies { get { yield return this.GetType().Assembly; } }

        private void RegisterSerializationAssemblies()
        {
            foreach (var a in SerializationAssemblies) { LionJsonSerializer.EnableAliases(a); }
        }
        #endregion

//#endregion

        #region Bug Reporting

        #region Configuration

        // Only for .NET 4+
        public virtual bool LogFirstChanceExceptions => true;

        #endregion

        /// <summary>
        /// Call once at startup
        /// </summary>
        private void TryEnableBugReporter()
        {
            if (!IsBugReporterEnabled) return;
            try
            {
                SplashMessage = "Enabling BugReporter";
                if (_bugReporter == null)
                {
                    if (IsDevMode)
                    {
                        _bugReporter = new NullBugReporter();
                    }
                    else
                    {
                        _bugReporter
#if NBug
 = new NBugReporter();
#else
 = new NullBugReporter();
#endif
                    }
                }

                BugReporter.IsEnabled = true;
            }
            finally
            {
                SplashMessage = "BugReporter enabled";
            }
        }
        private IBugReporter BugReporter
        {
            get
            {
                return _bugReporter;
            }
        }
        private IBugReporter _bugReporter;

#if !UNITY

        public bool OnApplicationDispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            bool result = false;

            if (BugReporter != null) { result = BugReporter.OnApplicationDispatcherException(sender, e); }

            // REVIEW - RECENTCHANGE
            if (!result)
            {
                result = Shell.AskUserToContinueOnException(e.Exception);
            }
            e.Handled = result;
            return result;
        }

#endif

        #endregion

      

        #region Settings

        public override IEnumerable<IReadWriteHandle> SettingsObjects
        {
            get
            {
                //if (UseLogSettings)
                //{
                //    yield return LogSettings.VLogSettings;
                //}
                yield break;
            }
        }

        #region Initialization

        private async Task InitSettingsObjects()
        {
            foreach (var vh in SettingsObjects)
            {
                await InitSettings(vh).ConfigureAwait(false);
            }
        }


        protected static async Task InitSettings(IReadWriteHandle vs)
        {
            try
            {
                // TOPORT: ResolveOrCreate
                await vs.Resolve().ConfigureAwait(false);
                //        vs.RetrieveOrCreate();
            }
            catch (Exception ex)
            {
                l.Error("Failed to init settings: " + vs.Reference.ToString() + " - " + ex.ToString());
            }
            //    vs.IsObjectReferenceFrozen = true;
            // TOPORT - AutoLoad and AutoSave these Vobs
            //    vs.AutoLoad = true;
            //    vs.AutoSave = true;
        }

        #endregion

        #endregion

        #region Construction / Destruction

        public AssemblyInitializer AssemblyInitializer => assemblyInitializer;
        protected AssemblyInitializer assemblyInitializer = new AssemblyInitializer();

        public LionFireApp()
            : this(null, null, null, null)
        {
        }

        protected virtual void EarlyCtor()
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        }

        void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
#if !UNITY
            var a = args.LoadedAssembly;

            foreach (var attr in a.CustomAttributes)
            {
                try
                {
#if !MONO // TODO OPTIMIZE TOMONO
                    if (attr.AttributeType == typeof(UseAliasesForSerializationAttribute))
#endif
                    {
                        //l.Debug("Delay registering assembly for aliased serialization: " + a.FullName);
                        LionJsonSerializer.EnableAliases(a);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    l.Debug(ex.ToString());
                }
            }
#endif
        }



        public bool IsInitialized { get { return isInitialized && this.Shell != null; } }
        private bool isInitialized = false;

        // REFACTOR: Eliminate appName/companyName from here, override properties instead
        public LionFireApp(string appName = null, string companyName = null, string commonAppDirDirectoryName = null, ILionFireShell shell = null)
        {
            LionFireApp.Current = this; // Do this before anything

            EarlyCtor();
            RegisterSerializationAssemblies();

            //Log.InitTraceListeners(); // TOPORT

            this.Shell = shell;

        #region Environment

            if (!LionFireEnvironment.HasLionAppName)
            {
                LionFireEnvironment.LionAppName = appName ?? DefaultAppName ?? LionFireEnvironment.LionAppName; // Use parameter, or forces it to be permanently set at ctor time.
            }

            LionFireEnvironment.CommonAppDirDirectoryName = commonAppDirDirectoryName; // Will use LionAppName if null

            LionFireEnvironment.CompanyName = companyName ?? DefaultCompanyName;

            if (String.IsNullOrWhiteSpace(LionFireEnvironment.CompanyName))
            {
                LionFireEnvironment.CompanyName = "UnknownCompany";
            }

        #endregion
            isInitialized = true;

        }

        #endregion
#endif
        #region State

        #region Splash

        #region SplashMessage

        public string SplashMessage
        {
            get { return splashMessage; }
            set
            {
                if (splashMessage == value) return;
                splashMessage = value;
                lSplash.Debug("[splash] " + splashMessage);
                OnPropertyChanged("SplashMessage");
            }
        }
        private string splashMessage;

        #endregion

        #region SplashProgress

        public double SplashProgress
        {
            get { return splashProgress; }
            set
            {
                if (splashProgress == value) return;
                splashProgress = value;
                //lSplash.Trace("[splash %] " + splashProgress.ToString());
                OnPropertyChanged("SplashProgress");
            }
        }
        private double splashProgress;

        #endregion

        #endregion

        #endregion

#if TOPORT // maybe

        #region Command Line Utils

        #region ReadLine with Timeout
        // MOVE

        // http://stackoverflow.com/questions/57615/how-to-add-a-timeout-to-console-readline
        class Reader
        {
            private static Thread inputThread;
            private static AutoResetEvent getInput, gotInput;
            private static string input;

            static Reader()
            {
                getInput = new AutoResetEvent(false);
                gotInput = new AutoResetEvent(false);
                inputThread = new Thread(reader);
                inputThread.IsBackground = true;
                inputThread.Start();
            }

            private static void reader()
            {
                while (true)
                {
                    getInput.WaitOne();
                    input = Console.ReadLine();
                    gotInput.Set();
                }
            }

            public static string ReadLine(int timeOutMillisecs)
            {
                getInput.Set();
                bool success = gotInput.WaitOne(timeOutMillisecs);
                if (success)
                    return input;
                else
                    return null;
            }
        }

        #endregion


        /// <summary>
        /// Run a service from the command line.  This will block at the console until the user enters 'exit' and enter on a line.
        /// To daemonize on mono, pass the -d parameter.
        /// </summary>
        public void RunCommandLineService()
        {
            RunMethod = CommandLineServiceRunMethod;
            Run();
        }

        private void CommandLineServiceRunMethod()
        {
            //Under mono if you deamonize a process a Console.ReadLine with cause an EOF 
            //so we need to block another way
            var args = Environment.GetCommandLineArgs();
            if (args.Any(s => s.Equals("-d", StringComparison.CurrentCultureIgnoreCase)))
            {
                Console.WriteLine("Service started.  Will continue running until LionFireApp is stopped.");
                isStoppedEvent.Wait();
                //Thread.Sleep(Timeout.Infinite); OLD
            }
            else
            {
                string text;
                while ((text = Reader.ReadLine(1000)) != "exit") // REVIEW - what is a good delay here?
                {
                    if (isStoppedEvent.IsSet) { break; }
                    if (text != null)
                    {
                        Console.WriteLine("Type 'exit' to exit.");
                    }
                }
            }
            Console.WriteLine("Service run method completed.");
        }

        #endregion


        #region Lifecycle - Start, Closing

        public virtual void Pause()
        {
            throw new NotSupportedException();
        }
        public virtual void Continue()
        {
            throw new NotSupportedException();
        }

        #region AppState

        public AppState AppState
        {
            get => appState;
            set
            {
                if (appState == value) return;
                var oldState = value;
                appState = value;

                AppStateChangedFromTo?.Invoke(oldState, appState);
            }
        }
        private AppState appState;

        public event Action<AppState, AppState> AppStateChangedFromTo;

        #endregion

        #region Start

        #region Configuration: Start

        public virtual IEnumerable<Assembly> StartupAssemblies { get { yield break; } }
        public virtual IEnumerable<Assembly> DelayLoadedAssemblies { get { yield break; } }

        #endregion

        volatile bool startInvoked = false;

        public event Action AppStarted;
        public event Action AppFinished;

        #region IsStarted

        public bool IsStarted => isStarted;

        #region isStarted

        /// <summary>
        /// Services with no RunMethod should set this to false in the event the service stops.
        /// </summary>
        protected bool isStarted
        {
            get => _isStarted;
            set
            {
                if (_isStarted == value) return;
                _isStarted = value;
                if (_isStarted)
                {
                    isStartedEvent.Set();
                    isStoppedEvent.Reset();
                }
                else
                {
                    isStartedEvent.Reset();
                    isStoppedEvent.Set();
                }
            }
        }
        private bool _isStarted;
        readonly ManualResetEventSlim isStartedEvent = new ManualResetEventSlim(false);
        readonly ManualResetEventSlim isStoppedEvent = new ManualResetEventSlim(true);

        #endregion

        #endregion

        /// <summary>
        /// The shell should call this after construction-time initialization is done.
        /// </summary>
        public void Start()
        {
            if (startInvoked) throw new AlreadyException();
            startInvoked = true;
            this.StartTime = DateTime.UtcNow;

            AppState = AppState.Starting;

        #region Init Phase

            foreach (var a in StartupAssemblies)
            {
                l.Trace("Loading startup assembly: " + a.FullName);
            }

            if (InitMethod != null) InitMethod();

#if TRACE_FILENAMES
            //l.Debug("Process.GetCurrentProcess().MainModule.ModuleName - " + p.MainModule.ModuleName);
            //l.Debug("Process.GetCurrentProcess().MainModule.FileName - " + p.MainModule.FileName);
            //l.Debug("Process.GetCurrentProcess().StartInfo.FileName - " + p.StartInfo.FileName);
            //l.Debug("Environment.CommandLine - " + Environment.CommandLine);
#endif

        #region BugReporter

            TryEnableBugReporter();

        #endregion


            UtilityAssemblyInitializer.Initialize();

            l.Info(
                //LionFireEnvironment.LionAppName 
                LionFireEnvironment.VersionString
                + " starting from " + LionFireEnvironment.AppBinDir + " with AppDir " + LionFireEnvironment.AppDir);
            l.Info("Capabilities: " + Capabilities.ToString());

#if TRACE_BuildTimes
            var ass = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in ass)
            {
                if (!a.FullName.Contains("LionFire")) continue;
                var d = AssemblyFileMetaData.RetrieveLinkerTimestamp(a.Location);
                l.Trace(d.ToString("yy-MM-dd HH:mm:ss") + " " + a.Location);
            }
#endif

        #region Application Events

            if (LogFirstChanceExceptions)
            {
#if NET4 && !MONO
                AppDomain.CurrentDomain.FirstChanceException += new EventHandler<System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs>(CurrentDomain_FirstChanceException);
#endif
            }

            //AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

        #endregion


        #region Assembly Initializers

            AssemblyInitializer.Initialize();

        #endregion

        #region Vos
            
            if (IsVosEnabled)
            {
                VosApp.InitVosApp();
            }
            Timing.RecordFromStart("TimeToVosInit");

        #endregion

        #region Settings

            InitSettingsObjects();

        #endregion

        #region AutoUpdate

            if (!TryDoAutoUpdate())
            {
                DoAutoUpdate();
                return; // ----------->>>> SKIPS Starting the app!  Will restart.
            }

        #endregion

        #endregion

        #region Start Phase

        #region Derived Classes Custom Startup

            OnStarting();

#if SPLASH_TEST
            //ThreadPool.QueueUserWorkItem(new Action(() =>
            {
                for (int i = 100; i > 0; i--)
                {
                    Shell.BeginInvoke(() =>
                        {
                            LionFireApp.Current.SplashProgress+=0.03;
                            if (LionFireApp.Current.SplashProgress > 1.0) LionFireApp.Current.SplashProgress = 1.0;

                            LionFireApp.Current.SplashMessage =
                                String.Format("Loading: {0:P}", LionFireApp.Current.SplashProgress);
                                //"Loading " + LionFireApp.Current.SplashProgress.ToString() + "%";
                        });
                    Thread.Sleep(30);

                    if (LionFireApp.Current.SplashProgress >= 1.0) break;

                }
                //finished = true;
                //bool finished;
            }
            //));
#endif
        #endregion

            if (StartMethod != null)
            {
                try
                {
                    StartMethod();
                }
                catch (Exception ex)
                {
                    l.LogCritical("Application failed to Start. " + ex);
                    throw new LionFireException("Application failed to Start. ", ex);
                }
            }
            isStarted = true; // --- Boom. Started. --- 
            AppState = Applications.AppState.Started;

        #region Started Event

            var ev = AppStarted;
            if (ev != null) ev();

        #endregion

            OnStarted();

        #endregion

            Task.Run(() =>
            {
                Thread.Sleep(500);
                foreach (var a in DelayLoadedAssemblies)
                {
                    l.Trace("Loading delay-loaded assembly: " + a.FullName);
                }
            });

            if (RunMethod != null)
            {
                runCancellationTokenSource = new CancellationTokenSource();
                runTask = Task.Factory.StartNew(() =>
                    {
                        runTaskThreads.Add(Thread.CurrentThread);
                        l.Trace("Executing RunMethod");
                        RunMethod();
                        AppState = Applications.AppState.Stopping;
                        l.Trace("RunMethod finished.  Firing OnClosing event");
                        OnClosing();
                        l.Trace("OnClosing event finished.");
                        OnAppStopped();
                    }, runCancellationTokenSource.Token);
            }
            else
            {
                if (StartMethod == null)
                {
                    OnNoRunOrStartMethod();
                }
            }
        }

        public bool ShellProvidesRun
        {
            get
            {
                if (this.Shell != null && this.Shell.ProvidesRunLoop) return true;
                return false;
            }
        }
        protected virtual void OnNoRunOrStartMethod()
        {
            if (ShellProvidesRun) return;
            l.Warn("No RunMethod and no StartMethod.  Stopping.");
            throw new Exception("No RunMethod and no StartMethod.");
            OnAppStopped();
        }

        void OnAppStopped()
        {
            AppState = Applications.AppState.Stopped;
            isStarted = false;
            var ev = AppFinished; if (ev != null) { ev(); }
        }


        public void Run()
        {
            Start();
            WaitForStop();
        }

        public Action InitMethod { get; set; }

        #region RunMethod

        private CancellationTokenSource runCancellationTokenSource;
        private Task runTask;
        /// <summary>
        /// Add threads here to be killed in the event of a force stop.
        /// </summary>
        //#if UNITY
        private ConcurrentList<Thread> runTaskThreads = new ConcurrentList<Thread>();
        //#else
        //        private ConcurrentBag<Thread> runTaskThreads = new ConcurrentBag<Thread>();
        //#endif
        protected void AddRunTaskThread(Thread thread)
        {
            runTaskThreads.Add(thread);
        }

        /// <summary>
        /// If this app is invoked as a serviec, this method won't be invoked.
        /// </summary>
        public Action RunMethod
        {
            get { return runMethod; }
            set
            {
                if (runMethod == value) return;
                if (value != null && runMethod != default) throw new NotSupportedException("RunMethod can only be set once or back to null.");
                runMethod = value;
            }
        }
        private Action runMethod;

        #endregion


        #region StartMethod

        /// <summary>
        /// This method will be invoked at startup, before the RunMethod (if one exists)
        /// </summary>
        public Action StartMethod
        {
            get { return startMethod; }
            set
            {
                if (startMethod == value) return;
                if (value != null && startMethod != default(Action)) throw new NotSupportedException("StartMethod can only be set once or back to null.");
                startMethod = value;
            }
        }
        private Action startMethod;

        #endregion

        #region StopMethod

        /// <summary>
        /// This method will be invoked to change the app from started to false, before the OnClosing (if one exists)
        /// </summary>
        public Action StopMethod
        {
            get { return stopMethod; }
            set
            {
                if (stopMethod == value) return;
                if (value != null && stopMethod != default(Action)) throw new NotSupportedException("StopMethod can only be set once or back to null.");
                stopMethod = value;
            }
        }
        private Action stopMethod;

        #endregion

        #region Stop

        #region State: Stop

        #region IsStopRequested
        // REVIEW - see http://stackoverflow.com/questions/4783865/how-do-i-abort-cancel-tpl-tasks

        public bool IsStopRequested
        {
            get
            {
                return isStopRequested
                    || runCancellationTokenSource.IsCancellationRequested
                    ;
            }
            set
            {
                if (isStopRequested == value) return;
                isStopRequested = value;
                if (value)
                {
                    bool throwOnFirstException = false; // REVIEW
                    runCancellationTokenSource.Cancel(throwOnFirstException);
                }
                var ev = IsStopRequestedChanged;
                if (ev != null) ev();
                OnPropertyChanged("IsStopRequested");
            }
        }
        private bool isStopRequested;

        public event Action IsStopRequestedChanged;

        #endregion

        #region IsStopping

        public bool IsStopping
        {
            get { return isStopping; }
            set
            {
                if (isStopping == value) return;
                isStopping = value;
                OnPropertyChanged("IsStopping");
            }
        }
        private bool isStopping;

        #endregion

        #endregion

        #region Configuration: Stop

        public virtual bool RequestsStop { get { return StopMethod == null; } }

        public virtual bool StopsOnStopMethodException { get { return true; } }

        #endregion

        #region Methods: Stop

        public void WaitForStop()
        {
            isStoppedEvent.Wait();
        }

        public void Stop()
        {
            AppState = AppState.Stopping;
            if (RequestsStop)
            {
                IsStopRequested = true;
            }
            if (StopMethod != null)
            {
                try
                {
                    StopMethod();

                    OnAppStopped();
                }
                catch (Exception ex)
                {
                    if (StopsOnStopMethodException)
                    {
                        l.Error("[STOP Exception] " + ex);
                        isStarted = false;
                    }
                    else
                    {
                        l.LogCritical("[STOP Exception] " + ex);
                    }
                }
            }
        }

        public void ForceStopAfter(int millisecondsDelay = 5000)
        {
            Stop();
            isStoppedEvent.Wait(millisecondsDelay);
            if (!isStoppedEvent.IsSet)
            {
                ForceStop();
            }
        }

        public void ForceStop(bool tryCleanForceStop = false)
        {
            IsStopRequested = true;

            var threads = runTaskThreads;
            runTaskThreads = new ConcurrentList<Thread>();

            if (threads != null)
            {
                foreach (var thread in threads)
                {
                    l.LogCritical("[FORCE STOP] Aborting thread.  (This is a dangerous operation.)");
                    thread.Abort(); // Danger!
                }
            }
            isStarted = false;
            AppState = Applications.AppState.Stopped;
        }

        #endregion

        #endregion

        #endregion

        #region Event Handling - Close

        #region Closing

        /// <summary>
        /// Called by the shell's Close() (or specifically OnClosing()) to notify the App it should shut down
        /// </summary>
        /// <returns></returns>
        public override bool OnClosing() // Rename to stopping
        {
            Timing.RecordFromStart("TimeToClose");
            TimingManager.Instance.Flush();

            var args = new CancelableEventArgs();
            var ev = Closing;
            if (ev != null) { ev.InvokeCancelable(args); }
            return !args.CancelRequested;
        }

        public event Action<CancelableEventArgs> Closing;

        #endregion

        #endregion

        #endregion

        #region AutoUpdate


        #region AutoUpdate

        #region Update Paths

        public IEnumerable<string> ReleaseChannels
        {
            get
            {
                yield return "Prealpha";
                yield return "Alpha";
                yield return "Beta";
                yield return "Gold";
            }
        }

        public IEnumerable<string> UpdateUrlBases
        {
            get
            {
                foreach (var host in UpdateHosts)
                {
                    yield return host + LionFireEnvironment.LionAppName + "/" + LionFireEnvironment.PlatformKind;
                }
            }
        }
        public IEnumerable<string> UpdateUrls
        {
            get
            {
                foreach (var x in UpdateUrlBases) yield return x + "/" + CurrentReleaseChannel + "/Files";
            }
        }
        public IEnumerable<string> UpdateFeedUrls
        {
            get
            {
                foreach (var x in UpdateUrls) yield return x + "/" + "feed.xml";
            }
        }

        #endregion

        /// <summary>
        /// Returns true if startup should continue.
        /// (MODAL if !AutoUpdateAsync)
        /// </summary>
        /// <returns>false if the updater needs to restart.  In this case, the app initialization should cease.</returns>
        protected bool TryDoAutoUpdate()
        {
            // TODO: Check last update time, only check once per day/hour or whatever
            if (IsAutoUpdateEnabled)
            {
                try
                {
                    LionFireApp.Current.InitAutoUpdater();
                    if (AutoUpdateAsync)
                    {
                        LionFireApp.Current.CheckForUpdateAsync();
                    }
                    else
                    {
                        bool result;
                        using (Timing.StartNow("CheckForUpdate"))
                        {
                            result = LionFireApp.Current.CheckForUpdate();
                        }
                        if (result)
                        {
                            return Shell.AskUserToUpdate(); // MODAL
                        }
                        else
                        {
                            l.Info("App is up to date");
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Alerter.Alert(title: "Auto Update failed", message: "Try again later or download the latest installer.",
                        level: LogLevel.Error, exception: ex);
                }
            }
            else
            {
                //l.Trace("Autoupdate not enabled");
            }
            return true;
        }

        /// <summary>
        /// Go forward with an autoupdate: download and restart.
        /// </summary>
        public void DoAutoUpdate()
        {
#if NAppUpdater
            UpdateManager.Instance.ReportProgress += ReportProgressDelegate;
            //UpdateManager.Instance.PrepareUpdates();

            var iar = UpdateManager.Instance.BeginPrepareUpdates(OnPrepareUpdatesCompleted, null);
#endif
        }

#if NAppUpdater
        private void ReportProgressDelegate(UpdateProgressInfo currentStatus)
        {
            l.Debug("ReportProgressDelegate: " + currentStatus.ToString());
            l.Debug("ReportProgressDelegate: " + Dump(currentStatus));
            if (!currentStatus.StillWorking)
            {
                l.Debug("!currentStatus.StillWorking");
                //UpdateManager.Instance.ReportProgress -= ReportProgressDelegate;
            }
        }
        private static string Dump(UpdateProgressInfo info)
        {
            if (info == null) return "(null)";
            return info.Message + " [" + info.Percentage + "%] " + (info.StillWorking ? "" : "STOPPED");
        }



        private void OnPrepareUpdatesCompleted(IAsyncResult result
            //bool completed
            )
        {
            var upd = result as UpdateProcessAsyncResult;

            bool completed = result.IsCompleted;

            if (completed)
            {
                UpdateManager.Instance.ReportProgress -= ReportProgressDelegate;

                bool restart = LionFireApp.AutoUpdateAlwaysRestart;

                if (restart)
                {
                    l.Info("[AUTOUPDATER] Download of updates completed.  RESTARTING...");
                    Shell.Invoke(() =>
                        UpdateManager.Instance.ApplyUpdates(restart, true, true)
                    ); // Must be on main thread if it will shutdown
                }
                else
                {
                    l.Info("[AUTOUPDATER] Download of updates completed.  No restart required.  Applying updates...");
                    UpdateManager.Instance.ApplyUpdates(restart, true, true);
                    LionFireApp.Current.Start();
                }
            }
#if SanityChecks
            else
            {
                l.Warn("Download of updates NOT completed.");
            }
#endif
        }
#endif



        #endregion

        public string UpdateUrl
        {
            get
            {
                string updateUrl = null;
                foreach (var u in UpdateFeedUrls) { updateUrl = u; break; }

                return updateUrl;
            }
        }
#if NAppUpdater

        public bool CheckForUpdate()
        {
            if (UpdateManager.Instance.State == UpdateManager.UpdateProcessState.AfterRestart)
            {
                l.Info("Just updated and restarted.  Not checking for updates again.");
            }
            else
            {
                l.Info("Checking for updates from " + UpdateUrl);
                UpdateManager.Instance.CheckForUpdates();
                if (UpdateManager.Instance.UpdatesAvailable > 0)
                {
                    l.Info("--- Files needing updates  ---");
                    foreach (var t in UpdateManager.Instance.Tasks)
                    {
                        var fut = t as FileUpdateTask;
                        if (fut != null)
                        {
                            l.Info(" - " + fut.LocalPath);
                        }
                    }
                    l.Info("------------------------------ ");

                    return true;
                }
            }
            return false;
        }

        private void OnCheckForUpdateAsyncFinished(bool result)
        {
            var ev = OnUpdateAvailable;
            if (ev != null) ev(result);
        }
#else
        public bool CheckForUpdate()
        {
            return false;
        }

#endif
        public void CheckForUpdateAsync(Action<bool> onGotUpdate = null)
        {
            //using (Timing.StartNow("CheckForUpdateAsync")) // FUTURE
            l.Info("(async) Checking for updates from " + UpdateUrl);
            throw new NotImplementedException();
            //UpdateManager.Instance.CheckForUpdateAsync(onGotUpdate ?? OnCheckForUpdateAsyncFinished);            
        }

        public event Action<bool> OnUpdateAvailable;

        public void InitAutoUpdater()
        {
#if NAppUpdater

            string updateUrl = UpdateUrl;

            //UpdateManager.Instance.UpdateSource = new NAppUpdate.Framework.Sources.SimpleWebSource(updateUrl); // provided is the URL for the updates feed
            UpdateManager.Instance.UpdateSource = new AppUpdate.Sources.SimpleWebSource(updateUrl); // provided is the URL for the updates feed
            UpdateManager.Instance.ReinstateIfRestarted(); // required to be able to restore state after app restart

            UpdateManager.Instance.Logger.Logged += OnUpdateManager;
            //return UpdateManager.Instance;
#else
            //return null;
#endif
        }
#if NAppUpdater
        private void OnUpdateManager(AppUpdate.Common.Logger.LogItem li)
        {
            if (li.Severity == AppUpdate.Common.Logger.SeverityLevel.Debug)
            {
                lAutoUpdate.Debug(li.ToString());
            }
            else if (li.Severity == AppUpdate.Common.Logger.SeverityLevel.Warning)
            {
                lAutoUpdate.Warn(li.ToString());
            }
            else // if (li.Severity == AppUpdate.Common.Logger.SeverityLevel.Error)
            {
                lAutoUpdate.Error(li.ToString());
            }
        }
#endif

        #endregion

        #region Settings

        //public virtual void LoadSettings()
        //{

        //}

        //protected virtual void OnLoadedSettings()
        //{

        //}

        #endregion

        #region (Private) Exception Event Handling

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            l.LogCritical((e.IsTerminating ? "Crashing due to u" : "U") + "nhandled exception: " + e.ExceptionObject);
        }

        //void CurrentDomain_ProcessExit(object sender, EventArgs e)
        //{
        //    l.Fatal("Process exiting.");
        //}

#if NET4 && !MONO
        void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            lFirstChanceExceptions.Debug("First chance exception: " + e.Exception);

            if (e.Exception.Message.Contains("The Undo operation encountered a context"))
            {
                // This preceeded a crash.  Count: 1
                Debugger.Break();
            }
        }
#endif

        #endregion

        #region Debug

        #region DebugUIVisible

        public bool DebugUIVisible
        {
            get { return debugUIVisible; }
            set
            {
                if (debugUIVisible == value) return;
                debugUIVisible = value;

                var ev = DebugUIVisibleChanged;
                if (ev != null) ev();
            }
        }
        private bool debugUIVisible;

        public event Action DebugUIVisibleChanged;

        #endregion

        #endregion
#endif
        #region Misc

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        private static ILogger l = Log.Get();
        private static ILogger lSplash = Log.Get("LionFire.Applications.LionFireApp.Splash");
        private static ILogger lAutoUpdate = Log.Get("LionFire.Applications.LionFireApp.AutoUpdater");
        private static ILogger lFirstChanceExceptions = Log.Get("LionFire.Applications.LionFireApp.FirstChanceExceptions");

        #endregion


        //public Vob Documents { get { return V.Stores["Documents"]; } } // TOPORT

        #region IService  // TEMP?

        public virtual bool CanPauseAndContinue => false;

        public ServiceState State
        {
            get
            {
                throw new NotImplementedException();
                //if (IsStarted) return ServiceState.Started;
                //else return ServiceState.Stopped;
            }
        }//private ServiceState state;

        public event Action<IService> StateChanged;

        #endregion


    }
}
