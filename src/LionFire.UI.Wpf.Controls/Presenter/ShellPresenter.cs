using LionFire.Collections;
using LionFire.Dependencies;
using LionFire.Resolves;
using LionFire.UI;
using LionFire.UI.Windowing;
using LionFire.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LionFire.Alerting;
using LionFire.UI.Wpf;

namespace LionFire.Shell.Wpf
{
#if !NOESIS
    public class WpfShellPresenter : ShellPresenterBase<WpfShell>
    {
        public WpfShellPresenter(IServiceProvider serviceProvider, WpfShell shell, IOptionsMonitor<StartupInterfaceOptions> rootInterface, DesktopProfileManager windowLayoutManager
         , IUserLocalSettings<WindowSettings> windowSettings, IOptionsMonitor<ShellOptions> shellOptionsMonitor
         ) : base(serviceProvider, shell, rootInterface, windowLayoutManager, windowSettings, shellOptionsMonitor)
        {

        }
    }
#else
    public class NoesisUnityShellPresenter : ShellPresenterBase<NoesisUnityShell>
    {
        public NoesisUnityShellPresenter(IServiceProvider serviceProvider, NoesisUnityShell shell,  IOptionsMonitor<StartupInterfaceOptions> rootInterface, DesktopProfileManager windowLayoutManager, IUserLocalSettings<WindowSettings> windowSettings, IOptionsMonitor<ShellOptions> shellOptionsMonitor
         ) : base(serviceProvider, shell, rootInterface, windowLayoutManager, windowSettings, shellOptionsMonitor)
        {

        }
    }
#endif

    

    /// <summary>
    /// For WPF and Noesis
    /// </summary>
    /// <typeparam name="TShell"></typeparam>
    public class ShellPresenterBase<TShell> : IShellPresenter, IHostedService
        where TShell : class, ILionFireShell
#if WPF
            , IWpfShell
#endif
    {

#region Dependencies

        public IServiceProvider ServiceProvider { get; }
        public IOptionsMonitor<StartupInterfaceOptions> RootInterface { get; }
        public DesktopProfileManager WindowLayoutManager { get; }

#region Shell

        //[SetOnce]
        //public TShell Shell
        //{
        //    get => shell;
        //    set
        //    {
        //        if (shell == value) return;
        //        if (shell != default) throw new AlreadySetException();
        //        shell = value;
        //    }
        //}
        //private TShell shell;
        public TShell Shell { get; }
        ILionFireShell IShellPresenter.Shell { get => Shell; }

        public ShellOptions Options => ShellOptionsMonitor.CurrentValue;
        IOptionsMonitor<ShellOptions> ShellOptionsMonitor;

#endregion

#endregion

        public WindowSettings WindowSettings { get; set; }

        Func<Task> startup;

#region Construction

        public ShellPresenterBase(IServiceProvider serviceProvider, TShell shell, IOptionsMonitor<StartupInterfaceOptions> rootInterface, DesktopProfileManager windowLayoutManager, IUserLocalSettings<WindowSettings> windowSettings, IOptionsMonitor<ShellOptions> shellOptionsMonitor)
        {
            ShellOptionsMonitor = shellOptionsMonitor;
            Shell = shell;
            ServiceProvider = serviceProvider;
            RootInterface = rootInterface;
            WindowLayoutManager = windowLayoutManager;
            startup = () => Task.Run(async () =>
            {
                WindowSettings = (await windowSettings.GetValueAsync().ConfigureAwait(false));
            });

        }

#endregion

#region IHostedService

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await startup().ConfigureAwait(false);
            startup = null;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

#endregion

#region State

#region Presenters

        public MultiBindableDictionary<string, ShellContentPresenter> Presenters = new MultiBindableDictionary<string, ShellContentPresenter>();

#endregion

#endregion

#region Properties

#region Derived

        public bool AnyActive
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

#region (Public) Methods


        public void BringToFront()
        {
            throw new NotImplementedException();
#if TOPORT
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
#endif
        }


        public void Close()
        {

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
                    l.Error("Exception when closing child presenter", ex);
                }
            }
            //MainPresenter.Close();
        }

#region Root Views

        /// <summary>
        /// Invoked once at startup to bring up primary views
        /// </summary>
        public void ShowStartupInterfaces()
        {
            try
            {
                foreach (var r in RootInterface.CurrentValue.StartupInterfaces)
                {
                    Show(r);
                }
            }
            catch (Exception ex)
            {
                Alerter.Alert("Failed to show startup interface", ex);
            }

#if Windowing && WPF
            MainPresenter.Show(); // Show the window
            this.Shell.Application.MainWindow = MainPresenter.CurrentWindow;
#endif
        }

#endregion

#region UIReference

        public void Show(UIReference reference)
        {
            string tabName = reference.TabName ?? reference.EffectiveName ?? WpfShellTabNames.DefaultTabName;

            if (reference.ViewAction != null)
            {
                reference.ViewAction();
            }
            else if (reference.View != null)
            {
                throw new NotImplementedException(nameof(reference.View));
            }
            else if (reference.ViewType != null)
            {
                ShowControl(reference.ViewType, tabName, reference.DataContext);
            }
            else if (reference.ViewModelType != null)
            {
                var viewType = DependencyContext.Current.GetRequiredService<IViewLocator>().LocateTypeForModelType(reference.ViewModelType, null, null);
                if (viewType == null) { throw new Exception("Could not locate type for model type:" + reference.ViewModelType.FullName); }
                ShowControl(viewType, tabName, reference.DataContext);
            }
            else
            {
                throw new Exception("No default view or view action");
            }
        }

#endregion

#region Views

        public object ShowControl(Type type, string tabName = null, object dataContext = null)
        {

            ShowControlGenericMethodInfo ??= this.GetType().GetMethods().Where(mi => mi.Name == "ShowControl" && mi.ContainsGenericParameters).First();

            MethodInfo mi = ShowControlGenericMethodInfo.MakeGenericMethod(type ?? throw new ArgumentNullException(nameof(type)));
            return mi.Invoke(this, new object[] { tabName });
        }
        private static MethodInfo ShowControlGenericMethodInfo;

        public T ShowControl<T>(string tabName = null) where T : class => MainPresenter.PushTab<T>(tabName);
        public object ShowControl(Type type, string tabName = null) => MainPresenter.PushTab(type, tabName);

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

#endregion

#region MainPresenter

        // ENH - eliminate the concept of MainPresenter, and instead use options for special behavior for "main" presenters, or let application name a particular presenter as "main"

        private const string MainPresenterName = WindowProfile.MainName;

        IShellContentPresenter IShellPresenter.MainPresenter => this.MainPresenter;
        public ShellContentPresenter MainPresenter
        {
            get
            {
                if (mainPresenter == null)
                {
                    mainPresenter = new ShellContentPresenter(this, MainPresenterName);
                    //t.SetApartmentState(ApartmentState.STA); // OLD - is this needed for some reason?

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

        async void mainPresenter_Closed(ShellContentPresenter obj)
        {
            Presenters.Remove(obj.Name);
            
            if (Options.StopOnMainPresenterClose) // TODO - move
            {
                await Shell.StopAsync(default).ConfigureAwait(false);
            }
        }

#endregion

        // REVIEW
        public Dictionary<string, Type> PresenterTypes = new Dictionary<string, Type>();

#region Presenters Collection

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

        private static ILogger l = Log.Get();

    }

}
