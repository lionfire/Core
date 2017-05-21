using LionFire.DependencyInjection;
using LionFire.Execution;
using LionFire.Execution.Executables;
using LionFire.Messaging;
using LionFire.Messaging.Queues.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Hardcodet.Wpf;
using Hardcodet.Wpf.TaskbarNotification;
using Caliburn.Micro;

namespace LionFire.Notifications.Wpf
{

    public class WpfNotifierService : ExecutableBase, IStartable, IStoppable
    {

        TaskbarIcon taskbarIcon;
        FSDirectoryQueueReader queue;

        public async Task Start()
        {
            queue = new FSDirectoryQueueReader
            {
                QueueDir = NotificationEnvironment.DesktopAlertQueueDir
            };
            queue.MessageReceived += OnMessageReceived;

            taskbarIcon = new TaskbarIcon()
            {
                //IconSource = 
            };
            taskbarIcon.ShowBalloonTip("Lino", "Active", BalloonIcon.Info);

            await queue.Start().ConfigureAwait(false);
        }

        private void OnMessageReceived(MessageEnvelope env)
        {
            if (env == null) return;
            Execute.BeginOnUIThread(() =>
            {
                env.OnHandled();
                try
                {
                    //taskbarIcon.ShowBalloonTip("TODO", "TODO", BalloonIcon.Info);

                    var wm = IoC.Get<IWindowManager>();
                    var vm = new PopupAlertViewModel()
                    {
                        Detail = env?.Payload?.ToString(),
                        Properties = {
                            ["MessageEnvelope"] = env
                        }
                    };

                    //dynamic settings = new ExpandoObject();
                    //settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    //settings.ResizeMode = ResizeMode.NoResize;
                    //settings.MinWidth = 450;
                    //settings.Title = "My New Window";
                    //settings.Icon = new BitmapImage(new Uri("pack://application:,,,/MyApplication;component/Assets/myicon.ico"));

                    //IWindowManager manager = new WindowManager();
                    //manager.ShowDialog(myViewModel, null, settings);
                    //wm.ShowDialog(vm, null);
                }
                catch (Exception )
                {
                    taskbarIcon.ShowBalloonTip("Error", "Exception showing notification popup", BalloonIcon.Error);
                }
            });
            //MessageBox.Show("Got new env: " + env?.Payload?.ToString());
        }

        public Task Stop(StopMode mode = StopMode.GracefulShutdown, StopOptions options = StopOptions.StopChildren)
        {
            queue.MessageReceived -= OnMessageReceived;
            queue.Stop();
            return Task.CompletedTask;
        }
    }
}
