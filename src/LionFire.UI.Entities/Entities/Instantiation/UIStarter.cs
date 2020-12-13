using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LionFire.Alerting;
using LionFire.Execution;
using LionFire.Threading;
using LionFire.DependencyMachines;
using LionFire.Collections;
using LionFire.Persistence.Persisters;

namespace LionFire.UI.Entities
{
    public interface IUIStarter : IStartable
    {
        IUIRoot UIRoot { get; }
    }

    /// <summary>
    /// Run by UIEntitiesService
    /// </summary>
    public class UIStarter : IUIStarter
    {
        #region Dependencies

        IOptionsMonitor<UIStartupOptions> UIStartupOptions;

        public IDispatcher Dispatcher { get; }
        public IUIRoot UIRoot { get; }
        IUIFactory UIFactory { get; }

        #endregion

        #region Construction

        public UIStarter(IDispatcher dispatcher, IOptionsMonitor<UIStartupOptions> shellStartupOptions, IUIRoot uiRoot, IUIFactory uiFactory)
        {
            Dispatcher = dispatcher;
            UIStartupOptions = shellStartupOptions;
            UIRoot = uiRoot;
            UIFactory = uiFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken = default) => ShowStartupInterfaces();

        #endregion

        /// <summary>
        /// Invoked once at startup to bring up primary views
        /// </summary>
        public async Task ShowStartupInterfaces()
        {
            if (!UIStartupOptions.CurrentValue.StartupEntities.Any()) return;

            await Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    //TempStartup();

                    UIFactory.CreateMany(UIStartupOptions.CurrentValue.StartupEntities);
                }
                catch (Exception ex)
                {
                    Alerter.Alert("Failed to show startup interface", ex);
                }
            }).ConfigureAwait(false);
        }

        //// TODO: Move code to UIStartupOptions as UIInstantiations
        private void TempStartup()
        {

            //    //var mainWindow = Show<IWindow>();
            //    //mainWindow.Key = ViewNameConventions.MainWindow;
            //    var mainWindow = ShowForNode<IWindow>(Root, "MainWindow");


            //    var mainWindowLayers = SetChildForNode<ILayers>(mainWindow);
            //    mainWindowLayers.Key = ViewNameConventions.MainWindow;

            //    var mainWindowBG = mainWindowLayers.GetLayer(LayerConventions.Background);

            //    var content = mainWindowLayers.GetLayer(LayerConventions.Content);

            //    var contentTabs = Show<ITabs>(content);

            //    // Old comments:
            //    // MainWindow: TabPresenter
            //    // Register TabPresenter as MainPresenter (or MainWindow?)
            //    // Show startup interface in TabPresenter

            //UIStartupOptions.CurrentValue.StartupViews.Select(sv => UIRoot.Show(sv));
            //await Task.WhenAll(UIStartupOptions.CurrentValue.StartupViews.Select(sv => Root.Show(sv))).ConfigureAwait(false);


            //RootPresenter.MainPresenter.Visible = true;
            //this.Shell.Application.MainWindow = MainPresenter.CurrentWindow;
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
}

