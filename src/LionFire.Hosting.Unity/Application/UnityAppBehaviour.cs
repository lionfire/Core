using Microsoft.Extensions.Configuration;
using LionFire.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using System;
using UnityEngine;

namespace LionFire.Hosting.Unity
{

    public class UnityAppBehaviour : MonoBehaviour
    {

        ApplicationLifetime ApplicationLifetime;

        //public UnityAppBehavior()
        //{
        //    ApplicationLifetime = new ApplicationLifetime(null); // TODO: Logger
        //} 

        void Awake()
        {
            Debug.Log("UnityLifetimeBehavior: Awake()");
        }
        void OnApplicationQuit()
        {
            Debug.Log("UnityLifetimeBehavior: OnApplicationQuit()");
        }

        //public IUnityHostBuilderProvider Provider;
        //public Type ProviderType;
        //public Func<MonoBehaviour, IHostBuilder> CreateHost;
        //public IUnityHostBuilderProvider Provider2 { get; set; }
        //public Type ProviderType2 { get; set; }
        //public Func<MonoBehaviour, IHostBuilder> CreateHost2 { get; set; }
        //public string Field;
        //public string Prop { get; set; }

        public virtual IConfigurationBuilder CreateConfigurationBuilder()
        {
            var config = new ConfigurationBuilder()
                 .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                 return config;
        }

        protected IConfiguration Config { get; private set; }

        /// <summary>
        /// Recommend to override this as desired
        /// </summary>
        /// <returns></returns>
        protected virtual IHostBuilder CreateHostBuilder()
            => new HostBuilder()
                .ConfigureServices(services =>
                    services
                        .AddUnityEngine()
                        .AddUnityRuntime(this, logStartStop: true))
                    ;

        private IHost host;

        protected virtual void OnEnable()
        {
            Debug.Log("UnityLifetimeBehavior: OnEnable()...");

            try
            {
                if (ApplicationLifetime != null) throw new System.Exception("UnityAppBehavior.Application is already set.  Duplicate OnEnable() invokation?");

                Config = CreateConfigurationBuilder().Build();

                var hostBuilder = CreateHostBuilder()
                    .ConfigureServices((ctx, services) =>
                    {
                        services
                            .AddUnityRuntime(this)
                            .AddSingleton<IHostApplicationLifetime>(serviceProvider => ApplicationLifetime = new ApplicationLifetime(serviceProvider.GetService<ILogger<ApplicationLifetime>>()))
                            .AddHostedService<UnityStartStopLogger>()
                            ;
                    });

                host = hostBuilder.Build();
                    
                ApplicationLifetime.ApplicationStarted.Register(() => Debug.Log("ApplicationLifetime.ApplicationStarted"));
                ApplicationLifetime.ApplicationStopped.Register(() => Debug.Log("ApplicationLifetime.ApplicationStopped"));

                var startTask = host.StartAsync();
                ApplicationLifetime.NotifyStarted();
                Debug.Log("UnityLifetimeBehavior: OnEnable()...done.  Start task complete: " + startTask.IsCompleted); // REVIEW - block until start is complete?
            }
            catch (Exception )
            {
                Debug.LogError("UnityLifetimeBehavior: OnEnable()...threw exception.");
                throw;
            }
        }

        protected virtual void OnDisable()
        {
            Debug.Log($"[{ typeof(UnityAppBehaviour).Name}] : OnDisable()...");

            if (ApplicationLifetime == null)
            {
                Debug.LogWarning($"[{typeof(UnityAppBehaviour).Name}] OnDisable: ApplicationLifetime is null.  Skipping shutdown.");
            }
            else
            {
                Debug.Log($"[{typeof(UnityAppBehaviour).Name}] OnDisable(): host.StopAsync()...");
                host.StopAsync(TimeSpan.FromSeconds(30));
                Debug.Log($"[{typeof(UnityAppBehaviour).Name}] OnDisable(): host.StopAsync()...done.");

                //Debug.Log($"[{typeof(UnityAppBehaviour).Name}] OnDisable(): StopApplication()...");
                //ApplicationLifetime.StopApplication();
                //Debug.Log($"[{typeof(UnityAppBehaviour).Name}] OnDisable(): NotifyStopped()...");
                //ApplicationLifetime.NotifyStopped();
            }
            Debug.Log($"[{typeof(UnityAppBehaviour).Name}] OnDisable()...done.");
            this.ApplicationLifetime = null;
        }


    }

}
