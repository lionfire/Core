using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Execution;
using System.ComponentModel;
using LionFire.Instantiating;
using LionFire.Notifications;
using LionFire.Assets;
using System.Threading;
using System.Diagnostics;

namespace LionFire.Supervisors
{
    public class TServiceStateSupervisorBase
    {
        public AssetReadHandle<TNotification> ServiceNotStarted { get; set; } = "ServiceNotStarted";
        public AssetReadHandle<TNotification> ServiceFaulted { get; set; } = "ServiceFaulted";
    }

    public class TServiceStateSupervisor : TServiceStateSupervisorBase, ITemplate<ServiceStateSupervisor>
    {
        //public AssetReadHandle<TNotification> ServiceNotStarted { get; set; } = "ServiceNotStarted";
        //public AssetReadHandle<TNotification> ServiceFaulted { get; set; } = "ServiceFaulted";

        public Type Type { get; set; }
    }

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
        ITemplate ITemplateInstance.Template { get => Template; set => Template = (TServiceStateSupervisor)value; }

        #endregion

        private Timer timer;

        public Task Start()
        {
            timer = new Timer(new TimerCallback((x) => { Debug.WriteLine("OnTimer - TODO");  }), null, 1000, 1000);
            return Task.CompletedTask;
        }
        public Task Stop()
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
