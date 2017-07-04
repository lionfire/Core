using System.Threading.Tasks;
using LionFire.Execution;
using System.Timers;
using LionFire.Messaging.Queues.IO;
using LionFire.Messaging;
using System.IO;
using LionFire.Messaging.Queues;

namespace LionFire.Notifications.Wpf.App
{
    public class TestNotificationQueueFiller : IStartable
    {

        FSDirectoryQueueWriter writer = new FSDirectoryQueueWriter();

        Timer timer;
        public Task Start()
        {
            writer.QueueDir = Path.Combine(NotificationEnvironment.DesktopAlertQueueDir, DirectoryQueue.InSubDir);

            timer = new Timer(8000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Start();

            return Task.CompletedTask;
        }
        int counter = 0;

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            writer.Enqueue(new MessageEnvelope
            {
                Payload = new Notifier
                {
                    Flags = NotificationFlags.MustAck,
                    Message = "Test " + counter++,
                }
            });

            bool keepgoing = true;
            if (!keepgoing)
            {
                timer.Stop();
            }
        }
    }
}