using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using LionFire.Applications.Hosting;
using System.Threading.Tasks;
using LionFire.Threading.Tasks;
using LionFire.Serialization;
using LionFire.Serialization.Json.Newtonsoft;
using LionFire.Trading.Applications;
using LionFire.Trading;
using LionFire.Composables;
using LionFire.Trading.TrueFx;
using LionFire.Trading.Spotware.Connect;
using LionFire.Assets;

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
                .Initialize()
                .Add<SerializationPackage>()
                .Add<NewtonsoftJsonSerialization>()
                //.Add<VosPackage>()
                .Add<WpfNotifierService>()
                .Add<TestNotificationQueueFiller>()
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
                ;
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            runTask = app.Run();
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