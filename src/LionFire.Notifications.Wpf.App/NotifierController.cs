using LionFire.DependencyInjection;
using LionFire.Execution;
using LionFire.Execution.Executables;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Notifications.Wpf
{

    public class NotificationEnvironment
    {
        public const string NotificationsSubPath = "Notifications";

        public static string Dir
        {
            get
            {
                return Path.GetFullPath(Path.Combine(
                    LionFireEnvironment.CompanyProgramDataDir,
                    NotificationsSubPath,
                    "Machine",
                    "In"
                    ));

            }
        }
    }

    public class WpfNotifierService : ExecutableBase, IStartable, IStoppable
    {

        public Task Start()
        {

            var q = new AssetFolderQueue { Path = NotificationEnvironment.Dir };
            q.
            q.Start();

            RunLoop().FireAndForget();
        }




        public Task Stop(StopMode mode = StopMode.GracefulShutdown, StopOptions options = StopOptions.StopChildren)
        {
            throw new NotImplementedException();
        }
    }
}
