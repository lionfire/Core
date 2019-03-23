using System;
using System.Collections.Generic;
using System.Reflection;
using Caliburn.Micro;
using System.Linq;
using LionFire.Applications.Hosting;
using LionFire.Serialization.Json.Newtonsoft;
using LionFire.Serialization.UI;
using LionFire.UI;

namespace LionFire.Serialization.Wpf.App
{

    

    public class AppBootstrapper : BootstrapperBase
    {
        SimpleContainer container;

        IAppHost app;

        public AppBootstrapper()
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
            app = new AppHost()
                .Add(new AppInfo
                {
                    CompanyName = "LionFire",
                    ProgramName = "Serialization.Wpf",
                })
                .Add<SerializationPackage>()
                .Add<NewtonsoftJsonSerialization>()
                ;
            
            app.Run();
            DisplayRootViewFor<IShell>();
        }
        protected override IEnumerable<Assembly> SelectAssemblies()
        {

            foreach (var a in base.SelectAssemblies()) yield return a;
            yield return  typeof(FsObjectCollectionView).Assembly;
            yield return  typeof(PropertyGridView).Assembly;
        }
    }
}