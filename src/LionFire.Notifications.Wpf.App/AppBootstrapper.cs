using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using LionFire.Applications.Hosting;
using System.Threading.Tasks;
using LionFire.Threading.Tasks;

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
                .Add(new AppInfo("LionFire", "Notifier.WPF", "Notifications/Apps/Notifier.WPF"))
                
                .Add<WpfNotifierService>()
                .Add<LionFireSerialization>()
                .AddPackage<NewtonsoftJsonSerialization>()
                ;
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            runTask = app.Run().FireAndForget("App");
        }
    }

    public class CaliburnMicroAppBootstrapper : BootstrapperBase
    {
        SimpleContainer container;

        public CaliburnMicroAppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            container = new SimpleContainer();

            container.Singleton<IWindowManager, WindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();
            container.PerRequest<IShell, ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            DisplayRootViewFor<IShell>();
        }
    }
}