using LionFire.Applications;
using LionFire.Applications.Modules;
using LionFire.Assets;
using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Notifications
{
    // TODO SOON
//    public class NotificationUtils
//    {
//        public static void QueueLocalNotification(Notification n)
//        {
//            var nm = App.Get<NotificationsModule>();
//            var ap = App.Get<IAssetProvider>();

//            var subpath = Guid.NewGuid().ToString();
//#warning NEXT: queuelocalnotification
//            //ap.Save(subpath,n, 
//        }
//    }

//    public class NotificationsModule : IModule
//    {

//        #region PersistenceContext 

//        public PersistenceContext PersistenceContext 
//        {
//            get { return persistenceContext; }
//            set { persistenceContext = value; }
//        }
//        private PersistenceContext  persistenceContext;

//        #endregion

        
//    }

}

namespace LionFire.Applications.Modules
{
    /// <summary>
    /// Modules examples:
    ///  - Notifications  /var/spool/mail
    ///  - Logs  /var/log
    /// -  Machine config for some area /etc/xyz/...
    /// - User SshConfig ~/.ssh
    /// </summary>
    public interface IModule
    {
        string RootPath { get; set; }
    }

    public class ModuleBase : IModule
    {
        public string RootPath { get; set; }
    }

    public class ModuleInfo
    {

    }
    public class ModuleLocator
    {
        public static ModuleLocator Default { get; private set; } = new ModuleLocator();

        public IModule Get(string key)
        {
            //LionFireEnvironment
            throw new NotImplementedException();
        }
    }
}
