using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Services.Hosting
{
    public class ServiceState
    {

        #region Relationships

        public ServiceConfig Config { get; set; }

        #endregion

        #region Construction

        public ServiceState()
        {
        }
        public ServiceState(ServiceConfig config)
        {
            this.Config = config;
        }

        #endregion

        #region Service Object

        public object ServiceObject { get; set; }

        #region Derived

        public IService Service { get { return ServiceObject as IService; } }

        public IDisposable DisposableServiceObject { get { return ServiceObject as IDisposable; } }

        public bool IsDisposable {
            get { return DisposableServiceObject != null; }
        }

        #endregion

        #endregion

        #region Status

        public ServiceStatus Status {
            get { return status; }
            set { status = value; }
        }
        private ServiceStatus status;

        #region Derived

        /// <summary>
        /// Returns true if it is desired for this service to be running (starting/started/resuming)
        /// </summary>
        public bool IsActive {
            get {
                switch (Status)
                {
                    //case ServiceStatus.Uninitialized:
                    //    break;
                    //case ServiceStatus.Initialized:
                    //    break;
                    case ServiceStatus.Starting:
                    case ServiceStatus.Started:
                    case ServiceStatus.Resuming:
                        return true;
                    //case ServiceStatus.Pausing:
                    //    break;
                    //case ServiceStatus.Paused:
                    //    break;
                    //case ServiceStatus.Stopping:
                    //    break;
                    //case ServiceStatus.Stopped:
                    //    break;
                    //case ServiceStatus.Aborting:
                    //    break;
                    //case ServiceStatus.Aborted:
                    //    break;
                    default:
                        return false;
                }
            }
        }

        public bool IsRunning {
            get {
                switch (Status)
                {
                    //case ServiceStatus.Uninitialized:
                    //    break;
                    //case ServiceStatus.Initialized:
                    //    break;
                    case ServiceStatus.Starting:
                    case ServiceStatus.Started:
                    case ServiceStatus.Pausing:
                    case ServiceStatus.Aborting:
                    case ServiceStatus.Resuming:
                    case ServiceStatus.Stopping:
                        return true;
                    //case ServiceStatus.Paused:
                    //    break;
                    //case ServiceStatus.Stopped:
                    //    break;
                    //case ServiceStatus.Aborted:
                    //    break;
                    default:
                        return false;
                }
            }
        }

        #endregion

        #endregion


    }

}
