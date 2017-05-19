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

namespace LionFire.Notifications.Wpf
{

    public class WpfNotifierService : ExecutableBase, IStartable, IStoppable
    {

        FSDirectoryQueueReader queue;

        public Task Start()
        {
            queue = new FSDirectoryQueueReader
            {
                QueueDir = NotificationEnvironment.DesktopAlertQueueDir
            };
            queue.MessageReceived += OnMessageReceived;
            queue.Start();

            return Task.CompletedTask;
        }

        private void OnMessageReceived(MessageEnvelope env)
        {
            env.OnHandled();
            MessageBox.Show("Got new env: " + env?.Payload?.ToString());
        }

        public Task Stop(StopMode mode = StopMode.GracefulShutdown, StopOptions options = StopOptions.StopChildren)
        {
            queue.MessageReceived -= OnMessageReceived;
            queue.Stop();
            return Task.CompletedTask;
        }
    }
}
