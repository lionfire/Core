using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using LionFire.Applications.Hosting;
using System.Threading.Tasks;
using LionFire.Threading.Tasks;
using LionFire.Serialization;
using LionFire.Serialization.Json.Newtonsoft;
#if Trading
using LionFire.Trading.Applications;
using LionFire.Trading;
using LionFire.Trading.TrueFx;
using LionFire.Trading.Spotware.Connect;
using LionFire.Trading.Notifications;
#endif
using System.Reflection;
using LionFire.Notifications.UI;

namespace LionFire.Notifications.Wpf.App
{
    public class AppBootstrapper : LionFireAppBootstrapper
    {
    }


    public class LionFireAppBootstrapper : CaliburnMicroAppBootstrapper
    {
        IAppHost app;
        Task runTask;

        protected override void Configure()
        {
            base.Configure();
            app = new AppHost()
                //.Add(new AppInfo("LionFire", "Notifier.WPF", "Notifications/Apps/Notifier.WPF"))
                .Add(new AppInfo("LionFire", "Trading Dash", "Trading"))
                .AddJsonAssetProvider()
                .Add<SerializationPackage>()
                .Add<NewtonsoftJsonSerialization>()
                .Initialize()
                //.Add<VosPackage>()
                .Add<WpfNotifierService>()
                //.Add<TestNotificationQueueFiller>()
#if Trading
                .Add<TradingNotifierHost>()
                .Add(new TradingNotificationsService
                {
                    //AccountNames = new List<string> { "IC Markets.Demo" },
                    AccountNames = new List<string> { "IC Markets^301078" },
                })
                .AddTrading(new TradingOptions
                {
                    AccountModes = AccountMode.Demo,
                })
                //.Add(new TTrueFxFeed
                //{
                //    //AccountId = "",
                //    //AccessToken = "",
                //})
                .AddSpotwareConnectClient("LionFire.Trading.App")
                .AddAsset<TCTraderAccount, IAppHost>("IC Markets.Demo")
#endif
                ;
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            runTask = app.Run();
            base.OnStartup(sender, e);

        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {

#if Trading
            yield return typeof(Trading.UI.TradingNotificationsViewModel).Assembly; // LionFire.Trading.UI
            yield return typeof(Trading.UI.TradingNotificationsView).Assembly; // LionFire.Trading.UI.WPF
#endif
            yield return typeof(NotificationHistoryView).Assembly; // LionFire.Notifications.WPF

            foreach (var assembly in base.SelectAssemblies())
            {
                yield return assembly;
            }
        }
    }

    public class CaliburnMicroAppBootstrapper : BootstrapperBase
    {
        protected SimpleContainer caliburnContainer;

        public CaliburnMicroAppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            caliburnContainer = new SimpleContainer();

            caliburnContainer.Singleton<IWindowManager, WindowManager>();
            caliburnContainer.Singleton<IEventAggregator, EventAggregator>();
            caliburnContainer.PerRequest<IShell, ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return caliburnContainer.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return caliburnContainer.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            caliburnContainer.BuildUp(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            DisplayRootViewFor<IShell>();
        }
    }
}