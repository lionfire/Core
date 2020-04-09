using System;
using System.Collections.Generic;

namespace LionFire.Applications.Splash
{
    public class SplashService : ISplashService
    {
        Stack<SplashDisposable> stack = new Stack<SplashDisposable>();

        #region ISplashService

        public IDisposable SetMessage(string message)
        {
            var d = new SplashDisposable(this) { Message = message };
            stack.Push(d);
            UpdateSplashMessage();
            return d;
        }

        public string Message { get; private set; }
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
    }


}
