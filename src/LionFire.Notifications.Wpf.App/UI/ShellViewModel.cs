using Caliburn.Micro;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using LionFire.Execution;
using LionFire.Messaging;
using LionFire.Messaging.Queues;
using LionFire.Messaging.Queues.IO;
using LionFire.Notifications.Twilio;
using LionFire.Structures;
#if Trading
using LionFire.Trading;
using LionFire.Trading.Spotware.Connect;
using LionFire.Trading.UI;
#endif
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using System;
using LionFire.Notifications.UI;
using System.Collections.ObjectModel;

namespace LionFire.Notifications.Wpf.App
{

    public class ShellViewModel : Conductor<ShellViewModel>.Collection.AllActive, IShell
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
        public CreateTestNotificationsViewModel CreateTestNotifications { get; set; } = new CreateTestNotificationsViewModel();

#if Trading
        public TradingNotificationsViewModel TradingNotificationsList { get; private set; } = new TradingNotificationsViewModel();
        public AccountsViewModel Accounts { get; set; } = new AccountsViewModel();
#endif

        #endregion

        public ShellViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
            this.Activated += ShellViewModel_Activated;
        }


        private void ShellViewModel_Activated(object sender, ActivationEventArgs e)
        {

        }


        public IEnumerable<object> ViewModels
        {
            get
            {
                //yield return TradingNotificationsList;
                yield return NotificationHistory;
                yield return CreateTestNotifications;
            }
        }
        private ObservableCollection<object> viewModels = new ObservableCollection<object>();

        protected override void OnActivate()
        {
            base.OnActivate();

            AutoUpdate = true;
            InitWriter();
            //InitAccount().FireAndForget();
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

        //IFeed feed;
        //Symbol gu;

        public void Update()
        {
            Debug.WriteLine("Update timer...");
        }
        //public async Task InitAccount()
        //{
        //    if (!isInit)
        //    {
        //        isInit = true;

        //        //feed = ManualSingleton<IAppHost>.Instance.Children.OfType<IFeed>().FirstOrDefault();
        //        //if (feed == null) return;

        //        //feed.AllowSubscribeToTicks = true;

        //        //if (feed is CTraderAccount ct)
        //        //{
        //        //    ct.IsTradeApiEnabled = true;
        //        //}

        //        //if (feed is IStartable startable)
        //        //{
        //        //    await startable.Start();
        //        //}

        //        //gu = feed.GetSymbol("GBPUSD");

        //        //gu.Ticked += OnTick;

        //    }
        //}


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
            //DependencyContext.Current.GetService<INotificationService>().Publish("DesktopAlerts", new
            //{
            //    Message = "hello "+msg
            //});
            writer.Enqueue(new MessageEnvelope
            {
                Payload = new Notifier
                {
                    Flags = NotificationFlags.MustAck,
                    Message = msg ?? "(no message)",
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
            StatusText = "TODO Voice";
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