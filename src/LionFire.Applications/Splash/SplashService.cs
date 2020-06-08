using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Applications.Splash
{

    public class SplashService : ISplashService, IHostedService
    {
        Stack<SplashDisposable> stack = new Stack<SplashDisposable>();

        public SplashService(IEnumerable<ISplashView> splashViews)
        {
            SplashViews = splashViews;
        }

        #region ISplashService

        public IDisposable SetMessage(string message)
        {
            var d = new SplashDisposable(this) { Message = message };
            stack.Push(d);
            UpdateSplashMessage();
            return d;
        }

        #endregion

        #region Message

        public string Message
        {
            get => message;
            set
            {
                if (message == value) return;
                message = value;
                OnPropertyChanged(nameof(Message));
                MessageChanged?.Invoke(message);
            }
        }
        private string message;
        public event Action<string> MessageChanged;

        #endregion

        internal void OnDeactivated(SplashDisposable _) => UpdateSplashMessage();
        private void UpdateSplashMessage()
        {
            var oldMessage = Message;
            while (stack.Count > 0 && !stack.Peek().IsActive) stack.Pop();

            if (stack.Count > 0)
            {
                Message = stack.Peek().Message;
                //was: LionFireApp.Current.SplashMessage = 
            }
            else
            {
                Message = "";
            }
            if (oldMessage != Message) MessageChanged?.Invoke(Message);
        }

        public Task StartAsync(CancellationToken cancellationToken)
            => Task.WhenAll(SplashViews.Select(async view =>
            {
                view.SplashService = this;
                await view.StartAsync(cancellationToken).ConfigureAwait(false);
            }));

        public IEnumerable<ISplashView> SplashViews { get; }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach(var view in SplashViews)
            {
                view.SplashService = null;
                view.Dispose();
            }
            return Task.CompletedTask;
        }

        #region Internal class

        internal class SplashDisposable : IDisposable
        {
            public bool IsActive { get; private set; } = true;

            public string Message { get; set; }

            SplashService SplashService;
            public SplashDisposable(SplashService splashService)
            {
                SplashService = splashService;
            }

            public void Dispose()
            {
                IsActive = false;
                this.SplashService.OnDeactivated(this);
            }
        }

        #endregion

        // TOPORT
        //#region Splash

        //#region SplashMessage

        //public string SplashMessage
        //{
        //    get { return splashMessage; }
        //    set
        //    {
        //        if (splashMessage == value) return;
        //        splashMessage = value;
        //        lSplash.Debug("[splash] " + splashMessage);
        //        OnPropertyChanged("SplashMessage");
        //    }
        //}
        //private string splashMessage;

        //#endregion

        //#region SplashProgress

        //public double SplashProgress
        //{
        //    get { return splashProgress; }
        //    set
        //    {
        //        if (splashProgress == value) return;
        //        splashProgress = value;
        //        //lSplash.Trace("[splash %] " + splashProgress.ToString());
        //        OnPropertyChanged("SplashProgress");
        //    }
        //}
        //private double splashProgress;

        //#endregion

        //private static ILogger lSplash { get; } = Log.Get("LionFire.Applications.LionFireApp.Splash");

        #region Misc


        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #endregion
    }



}
