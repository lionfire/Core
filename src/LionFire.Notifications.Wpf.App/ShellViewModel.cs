using Caliburn.Micro;
using LionFire.Applications.Hosting;
using LionFire.DependencyInjection;
using LionFire.Execution;
using LionFire.Notifications.Twilio;
using LionFire.Structures;
using LionFire.Trading;
using LionFire.Trading.Spotware.Connect;
using System.Diagnostics;
using System.Linq;
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

        IWindowManager windowManager;

        public ShellViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            AutoUpdate = true;
        }


        #region AutoUpdate

        public bool AutoUpdate
        {
            get { return autoUpdate; }
            set
            {
                if (autoUpdate == value) return;
                autoUpdate = value;
                autoUpdateTimer.Enabled = value;
                NotifyOfPropertyChange(() => AutoUpdate);
            }
        }
        private bool autoUpdate;
        
        Timer autoUpdateTimer = new Timer();
        
        #endregion

        bool isInit = false;

        IFeed feed;
        Symbol gu;

        public async void Update()
        {
            Debug.WriteLine("Update timer...");

            if (!isInit)
            {
                isInit = true;

                feed = ManualSingleton<IAppHost>.Instance.Components.OfType<IFeed>().FirstOrDefault();
                if (feed == null) return;

                var st = feed as IStartable;
                await st?.Start();

                CTraderAccount ct = feed as CTraderAccount;
                if (ct != null)
                {
                    ct.IsTradeApiEnabled = true;
                }

                gu = feed.GetSymbol("GBPUSD");

                gu.Ticked += OnTick;
                
            }
        }

        private void OnTick(SymbolTick tick)
        {
            Debug.WriteLine(tick.ToString());
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

        public void Notify1() { Notify("1"); }
        public void Notify2() { Notify("2"); }

        public void Notify(string msg)
        {
            InjectionContext.Current.GetService<INotificationService>().Publish("DesktopAlerts", new
            {
                Message = "hello "+msg
            });
        }




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