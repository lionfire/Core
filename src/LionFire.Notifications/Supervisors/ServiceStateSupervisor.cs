using System;
using System.Threading.Tasks;
using LionFire.Execution;
using System.ComponentModel;
using LionFire.Instantiating;
using LionFire.Notifications;
using System.Threading;
using System.Diagnostics;
using LionFire.Assets;

namespace LionFire.Supervisors
{
    public class TServiceStateSupervisorBase
    {
        public RAsset<Notifier> ServiceNotStarted { get; set; } = "ServiceNotStarted";
        public RAsset<Notifier> ServiceFaulted { get; set; } = "ServiceFaulted";
    }

    public class TServiceStateSupervisor : TServiceStateSupervisorBase, ITemplate<ServiceStateSupervisor>
    {
        //public AssetReadHandle<TNotification> ServiceNotStarted { get; set; } = "ServiceNotStarted";
        //public AssetReadHandle<TNotification> ServiceFaulted { get; set; } = "ServiceFaulted";

        public Type Type { get; set; }
    }

    // MOVE
    // What is this for?
    public class ServiceStateSupervisor : IStartable, INotifyPropertyChanged, ITemplateInstance<TServiceStateSupervisor>, IStoppable
    {

        #region ServiceObject

        public object ServiceObject
        {
            get { return serviceObject; }
            set
            {
                if (serviceObject == value) return;
                serviceObject = value;
                OnPropertyChanged(nameof(ServiceObject));
            }
        }
        private object serviceObject;

        public TServiceStateSupervisor Template { get; set; }

        #endregion

        private Timer timer;
        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            timer = new Timer(new TimerCallback((x) => { Debug.WriteLine("OnTimer - TODO");  }), null, 1000, 1000);
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            timer?.Dispose();
            timer = null;
            return Task.CompletedTask;
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
