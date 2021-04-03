using Microsoft.Extensions.DependencyInjection;
using System.Windows.Media;
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
using LionFire.Validation;
using System.IO;
using System.Media;

namespace LionFire.Notifications.Wpf
{
    // TODO: Decouple from FSDirectoryQueueReader, switch to ObjectBus/Vos

    public class WpfNotifierService : ExecutableBase, IStartable, IStoppable, IInitializable
    {
        public NotificationConfiguration NotificationConfiguration { get; set; }

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

            await queue.StartAsync().ConfigureAwait(false);
        }

        public string FindPathForFile(string subpath, IEnumerable<string> candidatePaths)
        {
            foreach (var dir in candidatePaths)
            {
                var path = Path.Combine(dir, subpath);
                if (File.Exists(path)) return path;
            }
            return null;
        }

        public IDisposable StartSound(NotificationProfile notificationProfile)
        {
            var path = FindPathForFile(notificationProfile.Sound, NotificationConfiguration.SoundPaths);

            if (path == null)
            {
                Debug.WriteLine("Sound not found in soundpath: " + notificationProfile.Sound);
                return null;
            }
            var mp = new MediaPlayer();
            mp.Open(new Uri("file:///" + path));
            mp.Play();
            return null;
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
                        //Message = en
                        Properties = {
                            ["MessageEnvelope"] = env
                        }
                    };

                    var profileKey = "G3";

                    if (!NotificationConfiguration.Profiles.TryGetValue(profileKey, out var notificationProfile))
                    {
                        Debug.WriteLine("No notification profile found: " + profileKey);
                        return;
                    }

                    StartSound(notificationProfile);

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
                catch (Exception)
                {
                    taskbarIcon.ShowBalloonTip("Error", "Exception showing notification popup", BalloonIcon.Error);
                }
            });
            //MessageBox.Show("Got new env: " + env?.Payload?.ToString());
        }

        public Task Stop()
        {
            queue.MessageReceived -= OnMessageReceived;
            queue.Stop();
            return Task.CompletedTask;
        }

        public Task<bool> Initialize()
        {
            this.NotificationConfiguration = DependencyContext.Current.ServiceProvider.GetService<NotificationConfiguration>() ?? NotificationDefaults.DefaultConfiguration;

            //if(NotificationConfiguration==null)

            return Task.FromResult(true);
        }
    }
}
