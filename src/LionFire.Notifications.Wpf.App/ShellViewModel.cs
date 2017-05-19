using Caliburn.Micro;
using LionFire.Notifications.Twilio;
using System.Timers;

namespace LionFire.Notifications.Wpf.App
{
    

    public class ShellViewModel : Screen, IShell {


        #region StatusText

        public string StatusText
        {
            get { return statusText; }
            set
            {
                if (statusText == value) return;
                statusText = value;
                NotifyOfPropertyChange(() => StatusText);
            }
        }
        private string statusText = "Constructed";

        #endregion


        public ShellViewModel()
        {
            
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            AutoUpdate = true;
        }


        #region TrueFxAutoUpdate

        public bool AutoUpdate
        {
            get { return autoUpdate; }
            set
            {
                if (autoUpdate == value) return;
                autoUpdate = value;
                
                NotifyOfPropertyChange(() => AutoUpdate);
            }
        }
        private bool autoUpdate;

        #endregion

        Timer autoUpdateTimer = new Timer();

        private void OnTrueFxAutoUpdateChanged()
        {
            if (autoUpdate)
            {
                autoUpdateTimer.Interval = UpdateInterval;
            }
            else
            {
            }
        }



        #region UpdateInterval

        public int UpdateInterval
        {
            get { return updateInterval; }
            set
            {
                if (updateInterval == value) return;
                updateInterval = value;
                NotifyOfPropertyChange(() => UpdateInterval);
            }
        }
        private int updateInterval = 3000;

        #endregion




        #region Twilio

        TwilioNotifier TwilioNotifier = new TwilioNotifier();
        public async void Voice()
        {
            StatusText="TODO Voice";
            await TwilioNotifier.SendVoiceAlert("test from wpf");
        }
        public async void SMS()
        {
            StatusText = "TODO SMS";
            await TwilioNotifier.SendSmsAlert("test from wpf");
        }
        
        #endregion

    }
}