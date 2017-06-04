using Caliburn.Micro;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using LionFire.Execution;
using LionFire.Messaging;
using LionFire.Messaging.Queues;
using LionFire.Messaging.Queues.IO;
using LionFire.Notifications.Twilio;
using LionFire.Structures;
using LionFire.Trading;
using LionFire.Trading.Spotware.Connect;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using System;
using LionFire.Trading.UI;

namespace LionFire.Notifications.Wpf.App
{

    //public enum 



    public class GenerateNotificationViewModel : Screen
    {
        #region Message

        public string Message
        {
            get { return message; }
            set
            {
                if (message == value) return;
                message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }
        private string message;

        #endregion


        #region Profile

        public string Profile
        {
            get { return profile; }
            set
            {
                if (profile == value) return;
                profile = value;
                NotifyOfPropertyChange(() => Profile);
            }
        }
        private string profile = "G3";

        #endregion

    }

    public class NotificationHistoryViewModel : Screen
    {
    }

    public class ShellViewModel : Screen, IShell
    {

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

        #region Dependencies

        IWindowManager windowManager;

        #endregion

        #region Children

        public NotificationHistoryViewModel NotificationHistory { get; private set; } = new NotificationHistoryViewModel();
        public GenerateNotificationViewModel GenerateNotification { get; private set; } = new GenerateNotificationViewModel();
        public TradingNotificationsViewModel NotificationsList { get; private set; } = new TradingNotificationsViewModel();

        public AccountsViewModel Accounts { get; set; } = new AccountsViewModel();

        #endregion

        public ShellViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            AutoUpdate = true;
            InitWriter();
            InitAccount().FireAndForget();
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

        public void Update()
        {
            Debug.WriteLine("Update timer...");
        }
        public async Task InitAccount()
        {
            if (!isInit)
            {
                isInit = true;

                feed = ManualSingleton<IAppHost>.Instance.Components.OfType<IFeed>().FirstOrDefault();
                if (feed == null) return;

                feed.AllowSubscribeToTicks = true;

                if (feed is CTraderAccount ct)
                {
                    ct.IsTradeApiEnabled = true;
                }

                if (feed is IStartable startable)
                {
                    await startable.Start();
                }

                //gu = feed.GetSymbol("GBPUSD");

                //gu.Ticked += OnTick;
                
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

        public void Notify1() { Notify("1"); }
        public void Notify2() { Notify("2"); }

        public void Notify(string msg)
        {
            //InjectionContext.Current.GetService<INotificationService>().Publish("DesktopAlerts", new
            //{
            //    Message = "hello "+msg
            //});
            writer.Enqueue(new MessageEnvelope
            {
                Payload = new TNotification
                {
                    Flags = NotificationFlags.MustAck,
                    Message = Message??"(no message)",
                    Profile = "G3",
                    
                }
            });
        }
        int counter = 0;

        FSDirectoryQueueWriter writer = new FSDirectoryQueueWriter();

        public void InitWriter()
        {
            writer.QueueDir = Path.Combine(NotificationEnvironment.DesktopAlertQueueDir, DirectoryQueue.InSubDir);
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