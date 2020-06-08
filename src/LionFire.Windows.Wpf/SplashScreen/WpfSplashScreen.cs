#nullable enable
using LionFire.Applications.Splash;
using LionFire.Avalon;
using LionFire.Threading;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
//using AppUpdate;
//using AppUpdate.Common;

namespace LionFire.Shell
{
    public class WpfSplashScreen : ISplashView // TOPORT
    {

        #region Configuration

        public virtual Type SplashScreenType => typeof(SplashWindow);
        private bool PrecreateWindow { get; set; } = true;

        #endregion

        #region Dependencies

        public ISplashService SplashService
        {
            set
            {
                if (value != null)
                {

                }
                else
                {
                    IsSplashVisible = false;
                }
                splashService = value;
            }
        }
        private ISplashService? splashService;

        public IDispatcher Dispatcher { get; }
        public IServiceProvider ServiceProvider { get; }


        #endregion

        #region Construction

        public WpfSplashScreen(IDispatcher dispatcher, IServiceProvider serviceProvider)
        {
            Dispatcher = dispatcher;
            ServiceProvider = serviceProvider;
        }

        #endregion

        #region State

        private Window? splash;

        #endregion

        protected bool IsSplashVisible
        {
            get => splash != null && splash.Visibility == Visibility.Visible;
            set
            {
                if (!Dispatcher.CheckAccess()) { Dispatcher.BeginInvoke(() => IsSplashVisible = value); return; }

                if (value)
                {
                    IsWindowCreated = true;
                    splash?.Show();
                }
                else
                {
                    if (splash != null)
                    {
                        splash.Hide();
                        splash.Close();
                        splash = null;
                    }
                }
            }
        }

        protected bool IsWindowCreated
        {
            set
            {
                if (value && SplashScreenType != null)
                {
                    splash = ActivatorUtilities.CreateInstance(ServiceProvider, this.SplashScreenType) as Window;
                    if (splash == null) { throw new Exception($"{SplashScreenType.FullName} must be of type {typeof(Window).FullName}"); }
                    splash.DataContext = this;
                }
                else
                {
                    splash = null;
                }
            }
        }

        #region IHostedService

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (PrecreateWindow)
            {
                IsWindowCreated = true;
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            IsSplashVisible = false;
            return Task.CompletedTask;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            IsSplashVisible = false;
            IsWindowCreated = false;
            splashService = null;
        }

        #endregion
    }
}
