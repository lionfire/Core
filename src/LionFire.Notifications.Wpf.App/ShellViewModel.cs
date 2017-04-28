using LionFire.Notifications.Twilio;

namespace LionFire.Notifications.Wpf.App
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell {


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

        TwilioNotifier TwilioNotifier = new TwilioNotifier();

        public ShellViewModel()
        {

        }

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

    }
}