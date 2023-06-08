#if OLD
using LionFire.Dependencies;
using LionFire.Data.Async.Gets;
using LionFire.UI;
using LionFire.UI.Windowing;
using LionFire.Settings;
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
using LionFire.Threading;
using Microsoft.Extensions.DependencyInjection;
using LionFire.ExtensionMethods;
using LionFire.Structures;

namespace LionFire.Shell.Wpf
{
    /// <summary>
    /// Top-level conductor
    /// </summary>
    public class ShellConductor : Conductor, IUIRoot
    {
        #region Dependencies

        public DesktopProfileManager WindowLayoutManager { get; }

        #region Options

        public ShellOptions Options => ShellOptionsMonitor.CurrentValue;
        IOptionsMonitor<ShellOptions> ShellOptionsMonitor;

        protected ShellPresenterOptions ShellPresenterOptions;
        public WindowSettings WindowSettings { get; private set; }

        #endregion

        #endregion

        #region Construction and Destruction

        public ShellConductor(IServiceProvider serviceProvider, IDispatcher dispatcher, DesktopProfileManager windowLayoutManager, ILazilyResolves<WindowSettings> windowSettings, IOptionsMonitor<ShellOptions> shellOptionsMonitor, IOptionsMonitor<ShellPresenterOptions> shellPresenterOptionsMonitor) : base(serviceProvider, dispatcher)
        {
            ShellOptionsMonitor = shellOptionsMonitor;
            ShellPresenterOptions = shellPresenterOptionsMonitor.CurrentValue;
            WindowLayoutManager = windowLayoutManager;
            WindowSettings = windowSettings.QueryNonDefaultValue(); // WindowSettings should already be resolved as a Hosted Participant that contributes CanStartShell 
        }

        #endregion

        public string DefaultPresenterName { get; set; }
        public IPresenter DefaultPresenter => Presenters.TryGetValue(DefaultPresenterName);


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

        #region Show

        protected IPresenter GetOrAddPresenter(string presenterName)
            => Presenters.GetOrAdd(presenterName, key =>
            {
                var presenter = ServiceProvider.GetRequiredService<IPresenter>();
                if (presenter is IKeyable<string> k) { k.Key = key; }
                return presenter;
            });

        public Task Show(UIInstantiation instantiation)
            => GetOrAddPresenter(instantiation.PresenterName ?? DefaultPresenterName).Show(instantiation);


        #endregion

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
            foreach (KeyValuePair<string, IPresenter> kvp in Presenters)
            {
                IPresenter scp = kvp.Value;
                if (scp.Contains(controlKey))
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        #endregion

        #region Event Handling

        protected override void OnPresenterDisposing(object obj)
        {
            base.OnPresenterDisposing(obj);

            if (Options.AutoClose && !Presenters.Values.SelectRecursive(p 
                => (p as IUICollection)?.Children.Values ?? Enumerable.Empty<IUIKeyed>()
            ).Where(p => p.Item.PreventAutoClose).Any())
            {
                OnSelfInitiatedClose();
            }
        }

        #endregion
    }
}

#region OLD - startup task
//Func<Task> startup; // StartAsync waits for ctor tasks to finish
//startup = () => Task.Run(async () =>
//{
//    WindowSettings = await windowSettings.GetNonDefaultValue().ConfigureAwait(false);
//    // ENH Make this a Participant that contributes to CanStartShell?
//});

//await startup().ConfigureAwait(false);
//startup = null;
#endregion

#endif